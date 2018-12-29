//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Data;
using CodeStack.SwEx.MacroFeature.Exceptions;
using CodeStack.SwEx.MacroFeature.Icons;
using CodeStack.SwEx.MacroFeature.Mocks;
using CodeStack.SwEx.MacroFeature.Placeholders;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Helpers
{
    internal class MacroFeatureParametersParser
    {
        private const string VERSION_PARAMETERS_NAME = "__paramsVersion";
        private const string VERSION_DIMENSIONS_NAME = "__dimsVersion";

        private string[] m_CurrentIcons;

        internal MacroFeatureParametersParser()
        {
        }

        internal MacroFeatureParametersParser(Type macroFeatType)
        {
            var app = Context.CurrentApp;
            m_CurrentIcons = MacroFeatureIconInfo.GetIcons(macroFeatType, app.SupportsHighResIcons());
        }

        internal TParams GetParameters<TParams>(IFeature feat, IMacroFeatureData featData, IModelDoc2 model, 
            out IDisplayDimension[] dispDims, out IBody2[] editBodies, out MacroFeatureOutdateState_e state)
            where TParams : class, new()
        {
            return GetParameters(feat, featData, model, typeof(TParams), out dispDims, out editBodies, out state) as TParams;
        }

        internal object GetParameters(IFeature feat, IMacroFeatureData featData, IModelDoc2 model, Type paramsType,
            out IDisplayDimension[] dispDims, out IBody2[] editBodies, out MacroFeatureOutdateState_e state)
        {
            object retParamNames = null;
            object retParamValues = null;
            object paramTypes = null;
            object retSelObj;
            object selObjType;
            object selMarks;
            object selDrViews;
            object compXforms;

            featData.GetParameters(out retParamNames, out paramTypes, out retParamValues);
            featData.GetSelections3(out retSelObj, out selObjType, out selMarks, out selDrViews, out compXforms);

            IDisplayDimension[] localDispDims = null;
            
            try
            {
                var dispDimsObj = featData.GetDisplayDimensions() as object[];

                if (dispDimsObj != null)
                {
                    localDispDims = new IDisplayDimension[dispDimsObj.Length];

                    for (int i = 0; i < localDispDims.Length; i++)
                    {
                        localDispDims[i] = dispDimsObj[i] as IDisplayDimension;
                        dispDimsObj[i] = null;
                    }
                }
                else
                {
                    localDispDims = null;
                }

                object[] editBodiesObj = featData.EditBodies as object[];

                IBody2[] localEditBodies = null;

                if (editBodiesObj != null)
                {
                    localEditBodies = editBodiesObj.Cast<IBody2>().ToArray();
                }

                var paramNames = retParamNames as string[];
                var paramValues = retParamValues as string[];
                var selObjects = retSelObj as object[];

                var parameters = new Dictionary<string, string>();

                var paramsVersion = new Version();
                var dimsVersion = new Version();

                if (paramNames != null && paramValues != null)
                {
                    for (int i = 0; i < paramNames.Length; i++)
                    {
                        if (paramNames[i] == VERSION_PARAMETERS_NAME)
                        {
                            paramsVersion = new Version(paramValues[i]);
                        }
                        else if (paramNames[i] == VERSION_DIMENSIONS_NAME)
                        {
                            paramsVersion = new Version(paramValues[i]);
                        }
                        else
                        {
                            parameters.Add(paramNames[i], paramValues[i]);
                        }
                    }
                }

                ConvertParameters(paramsType, paramsVersion, conv =>
                {
                    parameters = conv.ConvertParameters(model, feat, parameters);
                    localEditBodies = conv.ConvertEditBodies(model, feat, localEditBodies);
                    selObjects = conv.ConvertSelections(model, feat, selObjects);
                    localDispDims = conv.ConvertDisplayDimensions(model, feat, localDispDims);
                });
                
                var resParams = Activator.CreateInstance(paramsType);

                TraverseParametersDefinition(resParams.GetType(),
                    (prp) =>
                    {
                        AssignObjectsToProperty(resParams, selObjects, prp, parameters);
                    },
                    (dimType, prp) =>
                    {
                        var dimIndices = GetObjectIndices(prp, parameters);

                        if (dimIndices.Length != 1)
                        {
                            throw new InvalidOperationException(
                                "It could only be one index associated with dimension");
                        }

                        var dimInd = dimIndices.First();

                        if (localDispDims.Length > dimInd)
                        {
                            var dispDim = localDispDims[dimInd];

                            if (!(dispDim is DisplayDimensionPlaceholder))
                            {
                                var dim = dispDim.GetDimension2(0);
                                var val = (dim.GetSystemValue3(
                                    (int)swInConfigurationOpts_e.swSpecifyConfiguration,
                                    new string[] { featData.CurrentConfiguration.Name }) as double[])[0];
                                prp.SetValue(resParams, val, null);
                            }
                        }
                        else
                        {
                            throw new IndexOutOfRangeException(
                                $"Dimension at index {dimInd} id not present in the macro feature");
                        }
                    },
                    (prp) =>
                    {
                        AssignObjectsToProperty(resParams, localEditBodies, prp, parameters);
                    },
                    prp =>
                    {
                        var paramVal = GetParameterValue(parameters, prp.Name);
                        var val = Convert.ChangeType(paramVal, prp.PropertyType);
                        prp.SetValue(resParams, val, null);
                    });

                dispDims = localDispDims;
                editBodies = localEditBodies;
                state = GetState(featData, localDispDims);
                return resParams;
            }
            catch
            {
                ReleaseDisplayDimensions(localDispDims);

                throw;
            }
        }

        private void AssignObjectsToProperty(object resParams, Array availableObjects,
            PropertyInfo prp, Dictionary<string, string> parameters)
        {
            var indices = GetObjectIndices(prp, parameters);

            if (indices != null && indices.Any())
            {
                var isEmpty = availableObjects == null && indices.All(i => i == -1);

                object val = null;

                if (!isEmpty)
                {
                    if ((availableObjects != null && indices.All(i => availableObjects.Length > i)))
                    {
                        if (typeof(IList).IsAssignableFrom(prp.PropertyType))
                        {
                            var lst = prp.GetValue(resParams, null) as IList;

                            if (lst != null)
                            {
                                lst.Clear();
                            }
                            else
                            {
                                lst = Activator.CreateInstance(prp.PropertyType) as IList;
                            }

                            val = lst;

                            foreach (var obj in indices.Select(i =>
                            {
                                if (i < 0 || availableObjects == null)
                                {
                                    throw new IndexOutOfRangeException($"Index {i} is out of range for arrays");
                                }

                                return availableObjects.GetValue(i);
                            }))
                            {
                                lst.Add(obj);
                            }
                        }
                        else
                        {
                            if (indices.Length > 1)
                            {
                                throw new InvalidOperationException($"Multiple selection indices at {prp.Name} could only be associated with the List");
                            }

                            val = availableObjects.GetValue(indices.First());
                        }
                    }
                    else
                    {
                        throw new NullReferenceException(
                            $"Referenced entity is missing for {prp.Name}");
                    }

                }

                prp.SetValue(resParams, val, null);
            }
            else
            {
                throw new NullReferenceException($"Indices are not set for {prp.PropertyType.Name}");
            }
        }

        private int[] GetObjectIndices(PropertyInfo prp, Dictionary<string, string> parameters)
        {
            int[] indices = null;

            string indValues;

            if (parameters.TryGetValue(prp.Name, out indValues))
            {
                indices = indValues.Split(',').Select(i => int.Parse(i)).ToArray();
            }
            else
            {
                //throw new ParametersMismatchException("Object parameter (selection, edit body or dimension) indices have note been updated");

                //TODO: remove - legacy
                #region Legacy - remove

                var selAtt = prp.TryGetAttribute<ParameterSelectionAttribute>();
                var dimAtt = prp.TryGetAttribute<ParameterDimensionAttribute>();
                var editBodyAtt = prp.TryGetAttribute<ParameterEditBodyAttribute>();
#pragma warning disable CS0618
                if (selAtt != null && selAtt.SelectionIndex != -1)
                {
                    indices = new int[] { selAtt.SelectionIndex };
                }
                else if (dimAtt != null && dimAtt.DimensionIndex != -1)
                {
                    indices = new int[] { dimAtt.DimensionIndex };
                }
                else if (editBodyAtt != null && editBodyAtt.BodyIndex != -1)
                {
                    indices = new int[] { editBodyAtt.BodyIndex };
                }
                else
                {
                    throw new ParametersMismatchException("Object parameter (selection, edit body or dimension) indices have note been updated");
                }
#pragma warning restore
                #endregion
            }

            return indices;
        }

        internal static void ReleaseDisplayDimensions(IDisplayDimension[] dispDims)
        {
            if (dispDims != null)
            {
                for (int i = 0; i < dispDims.Length; i++)
                {
                    var dispDim = dispDims[i];
                    dispDims[i] = null;
                    ReleaseDimension(dispDim);
                }
            }
        }

        private void ConvertParameters(Type paramsType, Version paramVersion,
            Action<IParameterConverter> converter)
        {
            IParametersVersionConverter versConv = null;
            var curParamVersion = new Version();

            paramsType.TryGetAttribute<ParametersVersionAttribute>(a =>
            {
                versConv = a.VersionConverter;
                curParamVersion = a.Version;
            });

            if (curParamVersion != paramVersion)
            {
                if (curParamVersion > paramVersion)
                {
                    if (versConv != null)
                    {
                        if (versConv.ContainsKey(curParamVersion))
                        {
                            foreach (var conv in versConv.Where(
                                v => v.Key > paramVersion && v.Key <= curParamVersion)
                                .OrderBy(v => v.Key))
                            {
                                converter.Invoke(conv.Value);
                            }
                        }
                        else
                        {
                            throw new NullReferenceException($"{curParamVersion} version of parameters {paramsType.FullName} is not registered");
                        }
                    }
                    else
                    {
                        throw new NullReferenceException("Version converter is not set");
                    }
                }
                else
                {
                    throw new FutureVersionParametersException(paramsType, curParamVersion, paramVersion);
                }
            }
        }

        internal void SetParameters(IModelDoc2 model, IFeature feat,
            IMacroFeatureData featData, object parameters, out MacroFeatureOutdateState_e state)
        {
            string[] paramNames;
            int[] paramTypes;
            string[] paramValues;
            object[] selection;
            int[] dimTypes;
            double[] dimValues;
            IBody2[] bodies;

            Parse(parameters,
                out paramNames, out paramTypes, out paramValues,
                out selection, out dimTypes, out dimValues, out bodies);

            if (selection.Any())
            {
                var dispWraps = selection.Select(s => new DispatchWrapper(s)).ToArray();

                featData.SetSelections2(dispWraps, new int[selection.Length], new IView[selection.Length]);
            }

            featData.EditBodies = bodies;

            var dispDimsObj = featData.GetDisplayDimensions() as object[];

            IDisplayDimension[] dispDims = null;

            if (dispDimsObj != null)
            {
                dispDims = new IDisplayDimension[dispDimsObj.Length];

                for (int i = 0; i < dispDimsObj.Length; i++)
                {
                    dispDims[i] = dispDimsObj[i] as IDisplayDimension;
                    dispDimsObj[i] = null;
                }
            }

            var dimsVersion = GetDimensionsVersion(featData);

            ConvertParameters(parameters.GetType(), dimsVersion, conv =>
            {
                dispDims = conv.ConvertDisplayDimensions(model, feat, dispDims);
            });
            
            if (dispDims != null)
            {
                try
                {
                    if (dispDims.Length != dimValues.Length)
                    {
                        throw new ParametersMismatchException("Dimensions mismatch");
                    }

                    for (int i = 0; i < dispDims.Length; i++)
                    {
                        var dispDim = dispDims[i];

                        if (!(dispDim is DisplayDimensionPlaceholder))
                        {
                            SetAndReleaseDimension(dispDim, i, dimValues[i],
                                featData.CurrentConfiguration.Name);
                        }
                    }
                }
                catch
                {
                    ReleaseDisplayDimensions(dispDims);
                    throw;
                }
            }

            state = GetState(featData, dispDims);

            if (paramNames.Any())
            {
                //macro feature dimensions cannot be changed in the existing feature
                //reverting the dimensions version
                if (state.HasFlag(MacroFeatureOutdateState_e.Dimensions))
                {
                    var index = Array.IndexOf(paramNames, VERSION_DIMENSIONS_NAME);
                    paramValues[index] = dimsVersion.ToString();
                }

                featData.SetParameters(paramNames, paramTypes, paramValues);
            }
        }

        private Version GetDimensionsVersion(IMacroFeatureData featData)
        {
            return GetVersion(featData, VERSION_DIMENSIONS_NAME);
        }

        private MacroFeatureOutdateState_e GetState(IMacroFeatureData featData, IDisplayDimension[] dispDims)
        {
            var state = MacroFeatureOutdateState_e.UpToDate;

            if (dispDims != null && dispDims.Any(d => d is DisplayDimensionPlaceholder))
            {
                state |= MacroFeatureOutdateState_e.Dimensions;
            }

            if (m_CurrentIcons != null)
            {
                var curIcons = featData.IconFiles as string[];

                if (curIcons == null || !m_CurrentIcons.SequenceEqual(curIcons, 
                    StringComparer.CurrentCultureIgnoreCase))
                {
                    state |= MacroFeatureOutdateState_e.Icons;
                }
            }
            
            return state;
        }

        private Version GetVersion(IMacroFeatureData featData, string name)
        {
            Version dimsVersion;
            string versVal;
            featData.GetStringByName(name, out versVal);

            if (!Version.TryParse(versVal, out dimsVersion))
            {
                dimsVersion = new Version();
            }

            return dimsVersion;
        }

        private void SetAndReleaseDimension(IDisplayDimension dispDim,
            int index, double val, string confName)
        {
            var dim = dispDim.GetDimension2(0);

            dim.SetSystemValue3(val,
                (int)swSetValueInConfiguration_e.swSetValue_InSpecificConfigurations,
                new string[] { confName });

            ReleaseDimension(dispDim, dim);
        }

        private static void ReleaseDimension(IDisplayDimension dispDim, Dimension dim = null)
        {
            //NOTE: releasing the pointers as unreleased pointer might cause crash

            if (dim != null && Marshal.IsComObject(dim))
            {
                Marshal.ReleaseComObject(dim);
                dim = null;
            }

            if (dispDim != null && Marshal.IsComObject(dispDim))
            {
                Marshal.ReleaseComObject(dispDim);
                dispDim = null;
            }

            GC.Collect();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        internal void Parse(object parameters,
            out string[] paramNames, out int[] paramTypes,
            out string[] paramValues, out object[] selection,
            out int[] dimTypes, out double[] dimValues, out IBody2[] editBodies, bool removeDanglingRefs = false)
        {
            var paramNamesList = new List<string>();
            var paramTypesList = new List<int>();
            var paramValuesList = new List<string>();

            var selectionList = new List<Tuple<string, object>>();
            var dimsList = new List<Tuple<string, Tuple<swDimensionType_e, double>>>();
            var editBodiesList = new List<Tuple<string, IBody2>>();

            TraverseParametersDefinition(parameters.GetType(),
                (prp) =>
                {
                    ReadObjectsValueFromProperty(parameters, prp, selectionList, removeDanglingRefs);
                },
                (dimType, prp) =>
                {
                    var val = Convert.ToDouble(prp.GetValue(parameters, null));
                    dimsList.Add(new Tuple<string, Tuple<swDimensionType_e, double>>(
                        prp.Name, new Tuple<swDimensionType_e, double>(dimType, val)));
                },
                (prp) => 
                {
                    ReadObjectsValueFromProperty(parameters, prp, editBodiesList, removeDanglingRefs);
                },
                prp =>
                {
                    swMacroFeatureParamType_e paramType;

                    if (prp.PropertyType == typeof(int))
                    {
                        paramType = swMacroFeatureParamType_e.swMacroFeatureParamTypeInteger;
                    }
                    else if (prp.PropertyType == typeof(double))
                    {
                        paramType = swMacroFeatureParamType_e.swMacroFeatureParamTypeDouble;
                    }
                    else
                    {
                        paramType = swMacroFeatureParamType_e.swMacroFeatureParamTypeString;
                    }

                    var val = prp.GetValue(parameters, null);

                    paramNamesList.Add(prp.Name);
                    paramTypesList.Add((int)paramType);
                    paramValuesList.Add(Convert.ToString(val));
                });
            
            parameters.GetType().TryGetAttribute<ParametersVersionAttribute>(a => 
            {
                var setVersionFunc = new Action<string, Version>((n, v) => 
                {
                    var versParamIndex = paramNamesList.IndexOf(n);

                    if (versParamIndex == -1)
                    {
                        paramNamesList.Add(n);
                        paramValuesList.Add(v.ToString());
                        paramTypesList.Add((int)swMacroFeatureParamType_e.swMacroFeatureParamTypeString);
                    }
                    else
                    {
                        paramValuesList[versParamIndex] = v.ToString();
                    }
                });

                setVersionFunc.Invoke(VERSION_PARAMETERS_NAME, a.Version);
                setVersionFunc.Invoke(VERSION_DIMENSIONS_NAME, a.Version);
            });

            AddParametersForObjects(selectionList, paramNamesList, paramTypesList, paramValuesList);
            AddParametersForObjects(dimsList, paramNamesList, paramTypesList, paramValuesList);
            AddParametersForObjects(editBodiesList, paramNamesList, paramTypesList, paramValuesList);

            paramNames = paramNamesList.ToArray();
            paramTypes = paramTypesList.ToArray();
            paramValues = paramValuesList.ToArray();

            selection = selectionList.Select(s => s.Item2).Where(s => s != null).ToArray();

            dimTypes = dimsList.Select(d => (int)d.Item2.Item1).ToArray();
            dimValues = dimsList.Select(d => d.Item2.Item2).ToArray();

            editBodies = editBodiesList.Select(b => b.Item2).ToArray();
        }

        private void AddParametersForObjects<T>(List<Tuple<string, T>> objects,
            List<string> paramNamesList, List<int> paramTypesList,
            List<string> paramValuesList)
            where T : class
        {
            if (objects != null && objects.Any())
            {
                var paramsGroup = objects.GroupBy(o => o.Item1).ToDictionary(g => g.Key,
                    g => string.Join(",", g.Select(e => e.Item2 != null ? objects.IndexOf(e) : -1).ToArray()));

                paramNamesList.AddRange(paramsGroup.Keys);
                paramValuesList.AddRange(paramsGroup.Values);
                paramTypesList.AddRange(Enumerable.Repeat((int)swMacroFeatureParamType_e.swMacroFeatureParamTypeString, paramsGroup.Count));
            }
        }

        private void ReadObjectsValueFromProperty<T>(object parameters,
            PropertyInfo prp, List<Tuple<string, T>> list, bool removeNulls)
            where T : class
        {
            var val = prp.GetValue(parameters, null);

            if (val is IList)
            {
                foreach (T lstElem in val as IList)
                {
                    if (!removeNulls || lstElem != null)
                    {
                        list.Add(new Tuple<string, T>(prp.Name, lstElem));
                    }
                }
            }
            else
            {
                list.Add(new Tuple<string, T>(prp.Name, val as T));
            }
        }

        private void TraverseParametersDefinition(Type paramsType,
            Action<PropertyInfo> selParamHandler,
            Action<swDimensionType_e, PropertyInfo> dimParamHandler,
            Action<PropertyInfo> editBodyHandler,
            Action<PropertyInfo> dataParamHandler)
        {
            foreach (var prp in paramsType.GetProperties())
            {
                var prpType = prp.PropertyType;

                var selAtt = prp.TryGetAttribute<ParameterSelectionAttribute>();
                var dimAtt = prp.TryGetAttribute<ParameterDimensionAttribute>();
                var editBodyAtt = prp.TryGetAttribute<ParameterEditBodyAttribute>();

                if (selAtt != null)
                {
                    selParamHandler.Invoke(prp);
                }
                else if (dimAtt != null)
                {
                    var dimType = dimAtt.DimensionType;
                    dimParamHandler.Invoke(dimType, prp);
                }
                else if (editBodyAtt != null)
                {
                    editBodyHandler.Invoke(prp);
                }
                else
                {
                    if (prpType.IsPrimitive || prpType == typeof(string))
                    {
                        dataParamHandler.Invoke(prp);
                    }
                    else
                    {
                        throw new NotSupportedException(
                            $"{prp.Name} is not supported as the parameter of macro feature. Currently only primitive types and string are supported");
                    }
                }
            }
        }

        private string GetParameterValue(Dictionary<string, string> parameters, string name)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            string value;

            if (!parameters.TryGetValue(name, out value))
            {
                throw new IndexOutOfRangeException($"Failed to read parameter {name}");
            }

            return value;
        }
    }
}
