//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Base
{
    /// <summary>
    /// Represents the status result from macro feature and optional error message
    /// </summary>
    public class MacroFeatureRebuldStatusResult : MacroFeatureRebuildResult
    {
        private static object GetResult(bool status, string error = "")
        {
            if (status)
            {
                return status;
            }
            else
            {
                if (!string.IsNullOrEmpty(error))
                {
                    return error;
                }
                else
                {
                    return status;
                }
            }
        }

        internal protected MacroFeatureRebuldStatusResult(bool status, string error = "")
            : base(GetResult(status, error))
        {
        }
    }
}
