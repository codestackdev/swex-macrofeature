//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Exceptions
{
    public class ParametersMismatchException : Exception
    {
        public ParametersMismatchException(string reason) 
            : base($"{reason}. Please reinsert the feature as changing the dimensions in parameters is not supported")
        {
        }
    }
}
