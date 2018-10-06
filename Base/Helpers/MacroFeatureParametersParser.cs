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

            Parse(parameters,
                out paramNames, out paramTypes, out paramValues,
                out selection, out dimTypes, out dimValues);

            if (paramNames.Any())
            {
                featData.SetParameters(paramNames, paramTypes, paramValues);
            }

            if (selection.Any())
            {
                var dispWraps = selection.Select(s => new DispatchWrapper(s)).ToArray();

                featData.SetSelections2(dispWraps, new int[selection.Length], new IView[selection.Length]);
            }

            //TODO: set dimensions
        }

        internal void Parse(object parameters,
            out string[] paramNames, out int[] paramTypes,
            out string[] paramValues, out object[] selection,
            out int[] dimTypes, out double[] dimValues)
        {
            var paramNamesList = new List<string>();
            var paramTypesList = new List<int>();
            var paramValuesList = new List<string>();
            var selectionList = new Dictionary<int, object>();
            var dimTypesList = new List<int>();
            var dimValuesList = new List<double>();

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

            var isConsecutive = !selectionList.OrderBy(e => e.Key)
                .Select(e => e.Key)
                .Select((i, j) => i - j)
                .Distinct().Skip(1).Any();

            if (!isConsecutive)
            {
                throw new InvalidOperationException("Selection elements indices are not consecutive");
            }

            var disps = selectionList.OrderBy(e => e.Key).Select(e => e.Value);

            paramNames = paramNamesList.ToArray();
            paramTypes = paramTypesList.ToArray();
            paramValues = paramValuesList.ToArray();
            selection = disps.ToArray();
            dimTypes = dimTypesList.ToArray();
            dimValues = dimValuesList.ToArray();
        }

        private void TraverseParametersDefinition(Type paramsType,
            Action<int, PropertyInfo> selParamHandler, Action<PropertyInfo> dataParamHandler)
        {
            foreach (var prp in paramsType.GetProperties())
            {
                var prpType = prp.PropertyType;

                var selAtt = prp.TryGetAttribute<ParameterSelectionAttribute>();

                if (selAtt != null)
                {
                    var selIndex = selAtt.SelectionIndex;

                    selParamHandler.Invoke(selIndex, prp);
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
