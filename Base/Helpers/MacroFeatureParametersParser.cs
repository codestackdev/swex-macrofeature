//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Base;
using CodeStack.SwEx.MacroFeature.Data;
using CodeStack.SwEx.MacroFeature.Exceptions;
using CodeStack.SwEx.MacroFeature.Mocks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
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

        internal TParams GetParameters<TParams>(IFeature feat, IModelDoc2 model, out IDisplayDimension[] dispDims)
            where TParams : class, new()
        {
            return GetParameters(feat, model, typeof(TParams), out dispDims) as TParams;
        }

        internal object GetParameters(IFeature feat, IModelDoc2 model, Type paramsType, out IDisplayDimension[] dispDims)
        {
            var featData = feat.GetDefinition() as IMacroFeatureData;

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

                IBody2[] editBodies = null;

                if (editBodiesObj != null)
                {
                    editBodies = editBodiesObj.Cast<IBody2>().ToArray();
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
                    editBodies = conv.ConvertEditBodies(model, feat, editBodies);
                    selObjects = conv.ConvertSelections(model, feat, selObjects);
                    localDispDims = conv.ConvertDisplayDimensions(model, feat, localDispDims);
                });
                
                var resParams = Activator.CreateInstance(paramsType);

                TraverseParametersDefinition(resParams.GetType(),
                    (selIndex, prp) =>
                    {
                        if (selObjects != null && selObjects.Length > selIndex)
                        {
                            var val = selObjects[selIndex];
                            prp.SetValue(resParams, val, null);
                        }
                        else
                        {
                            throw new NullReferenceException(
                                $"Referenced entity is missing at index {selIndex} for {prp.PropertyType.Name}");
                        }
                    },
                    (dimInd, dimType, prp) =>
                    {
                        if (localDispDims.Length > dimInd)
                        {
                            var dispDim = localDispDims[dimInd];

                            if (!(dispDim is DisplayDimensionEmpty))
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
                    (bodyInd, prp) =>
                    {
                        if (editBodies.Length > bodyInd)
                        {
                            prp.SetValue(resParams, editBodies[bodyInd], null);
                        }
                        else
                        {
                            throw new IndexOutOfRangeException(
                                $"Edit body at index {bodyInd} id not present in the macro feature");
                        }
                    },
                    prp =>
                    {
                        var paramVal = GetParameterValue(parameters, prp.Name);
                        var val = Convert.ChangeType(paramVal, prp.PropertyType);
                        prp.SetValue(resParams, val, null);
                    });

                dispDims = localDispDims;
                return resParams;
            }
            catch
            {
                ReleaseDisplayDimensions(localDispDims);

                throw;
            }
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

        internal void SetParameters(IModelDoc2 model, IFeature feat, object parameters, out bool isOutdated)
        {
            var featData = feat.GetDefinition() as IMacroFeatureData;

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

            var dimsOutdated = false;

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

                        if (!(dispDim is DisplayDimensionEmpty))
                        {
                            SetAndReleaseDimension(dispDim, i, dimValues[i],
                                featData.CurrentConfiguration.Name);
                        }
                        else
                        {
                            dimsOutdated = true;
                        }
                    }
                }
                catch
                {
                    ReleaseDisplayDimensions(dispDims);
                    throw;
                }
            }

            if (paramNames.Any())
            {
                //macro feature dimensions cannot be changed in the existing feature
                //reverting the dimensions version
                if (dimsOutdated)
                {
                    var index = Array.IndexOf(paramNames, VERSION_DIMENSIONS_NAME);
                    paramValues[index] = dimsVersion.ToString();
                }

                featData.SetParameters(paramNames, paramTypes, paramValues);
            }

            isOutdated = dimsOutdated;
        }

        private Version GetDimensionsVersion(IMacroFeatureData featData)
        {
            return GetVersion(featData, VERSION_DIMENSIONS_NAME);
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
            out int[] dimTypes, out double[] dimValues, out IBody2[] editBodies)
        {
            var paramNamesList = new List<string>();
            var paramTypesList = new List<int>();
            var paramValuesList = new List<string>();
            var selectionList = new Dictionary<int, object>();
            var dimsList = new Dictionary<int, Tuple<swDimensionType_e, double>>();

            var editBodiesList = new Dictionary<int, IBody2>();

            TraverseParametersDefinition(parameters.GetType(),
                (selIndex, prp) =>
                {
                    if (selectionList.ContainsKey(selIndex))
                    {
                        throw new InvalidOperationException(
                            $"Duplicate declaration of the selection index {selIndex} for parameter {prp.Name}");
                    }

                    var val = prp.GetValue(parameters, null);

                    if (val != null)
                    {
                        selectionList.Add(selIndex, val);
                    }
                    else
                    {
                        throw new NullReferenceException(
                            $"Selection entity for {prp.PropertyType.Name} at selection index {selIndex} is null");
                    }
                },
                (dimInd, dimType, prp) =>
                {
                    var val = Convert.ToDouble(prp.GetValue(parameters, null));
                    dimsList.Add(dimInd, new Tuple<swDimensionType_e, double>(dimType, val));
                },
                (bodyInd, prp) => 
                {
                    editBodiesList.Add(bodyInd, prp.GetValue(parameters, null) as IBody2);
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

            if (!IsConsecutive(selectionList.Keys))
            {
                throw new InvalidOperationException("Selection elements indices are not consecutive");
            }

            var disps = selectionList.OrderBy(e => e.Key).Select(e => e.Value);

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

            paramNames = paramNamesList.ToArray();
            paramTypes = paramTypesList.ToArray();
            paramValues = paramValuesList.ToArray();
            selection = disps.ToArray();

            if (!IsConsecutive(dimsList.Keys))
            {
                throw new InvalidOperationException("Dimensions elements indices are not consecutive");
            }

            dimTypes = dimsList.OrderBy(d => d.Key).Select(d => (int)d.Value.Item1).ToArray();
            dimValues = dimsList.OrderBy(d => d.Key).Select(d => d.Value.Item2).ToArray();

            if (!IsConsecutive(editBodiesList.Keys))
            {
                throw new InvalidOperationException("Edit bodies elements indices are not consecutive");
            }

            editBodies = editBodiesList.OrderBy(d => d.Key).Select(d => d.Value).ToArray();
        }

        private static bool IsConsecutive(IEnumerable<int> selectionList)
        {
            return !selectionList.OrderBy(k => k)
                            .Select((i, j) => i - j)
                            .Distinct().Skip(1).Any();
        }

        private void TraverseParametersDefinition(Type paramsType,
            Action<int, PropertyInfo> selParamHandler,
            Action<int, swDimensionType_e, PropertyInfo> dimParamHandler,
            Action<int, PropertyInfo> editBodyHandler,
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
                    var selIndex = selAtt.SelectionIndex;

                    selParamHandler.Invoke(selIndex, prp);
                }
                else if (dimAtt != null)
                {
                    var dimType = dimAtt.DimensionType;
                    var dimInd = dimAtt.DimensionIndex;
                    dimParamHandler.Invoke(dimInd, dimType, prp);
                }
                else if (editBodyAtt != null)
                {
                    var bodyInd = editBodyAtt.BodyIndex;
                    editBodyHandler.Invoke(bodyInd, prp);
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
