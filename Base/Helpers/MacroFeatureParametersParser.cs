//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex-macrofeature
//**********************

using CodeStack.SwEx.Common.Reflection;
using CodeStack.SwEx.MacroFeature.Attributes;
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
        internal TParams GetParameters<TParams>(IMacroFeatureData featData)
            where TParams : class, new()
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

            var dispDimsObj = featData.GetDisplayDimensions() as object[];

            IDisplayDimension[] dispDims = null;

            if (dispDimsObj != null)
            {
                dispDims = dispDimsObj.Cast<IDisplayDimension>().ToArray();
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

            var resParams = new TParams();

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
                    if (dispDims.Length > dimInd)
                    {
                        var dispDim = dispDims[dimInd];
                        var dim = dispDim.GetDimension2(0);
                        var val = (dim.GetSystemValue3(
                            (int)swInConfigurationOpts_e.swSpecifyConfiguration,
                            new string[] { featData.CurrentConfiguration.Name }) as double[])[0];
                        prp.SetValue(resParams, val, null);
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
                    var paramVal = GetParameterValue(paramNames, paramValues, prp.Name);
                    var val = Convert.ChangeType(paramVal, prp.PropertyType);
                    prp.SetValue(resParams, val, null);
                });

            return resParams;
        }

        internal void SetParameters(IMacroFeatureData featData, object parameters)
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

            if (paramNames.Any())
            {
                featData.SetParameters(paramNames, paramTypes, paramValues);
            }

            if (selection.Any())
            {
                var dispWraps = selection.Select(s => new DispatchWrapper(s)).ToArray();

                featData.SetSelections2(dispWraps, new int[selection.Length], new IView[selection.Length]);
            }

            var dispDimsObj = featData.GetDisplayDimensions() as object[];

            if (dispDimsObj != null)
            {
                for (int i = 0; i < dispDimsObj.Length; i++)
                {
                    var dispDim = dispDimsObj[i] as IDisplayDimension;
                    SetAndReleaseDimension(dispDim, i, dimValues[i],
                        featData.CurrentConfiguration.Name);
                }
            }

            featData.EditBodies = bodies;
        }

        private void SetAndReleaseDimension(IDisplayDimension dispDim,
            int index, double val, string confName)
        {
            var dim = dispDim.GetDimension2(0);
            
            dim.SetSystemValue3(val,
                (int)swSetValueInConfiguration_e.swSetValue_InSpecificConfigurations,
                new string[] { confName });
            
            //NOTE: releasing the pointers as unreleased pointer might cause crash
            Marshal.ReleaseComObject(dim);
            Marshal.ReleaseComObject(dispDim);
            dim = null;
            dispDim = null;
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

        private string GetParameterValue(string[] paramNames, string[] paramValues, string name)
        {
            if (!(paramNames is string[]))
            {
                throw new ArgumentNullException(nameof(paramNames));
            }

            if (!(paramValues is string[]))
            {
                throw new ArgumentNullException(nameof(paramValues));
            }

            var paramNamesList = paramNames.ToList();

            var index = paramNamesList.IndexOf(name);

            if (index != -1)
            {
                if (paramValues.Length > index)
                {
                    return paramValues[index];
                }
                else
                {
                    throw new IndexOutOfRangeException($"Parameter {name} doesn't have a value");
                }
            }
            else
            {
                throw new IndexOutOfRangeException($"Failed to read parameter {name}");
            }
        }
    }
}
