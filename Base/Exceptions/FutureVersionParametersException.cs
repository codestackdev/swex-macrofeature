//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Exceptions
{
    /// <summary>
    /// Exception indicates that the version of the parameters of macro feature
    /// belongs of a never version of the add-in
    /// </summary>
    /// <remarks>Suggest users to upgrade the add-in version to support the feature</remarks>
    public class FutureVersionParametersException : Exception
    {
        internal FutureVersionParametersException(Type paramType, Version curParamVersion, Version paramVersion)
            : base($"Future version of parameters '{paramType.FullName}' {paramVersion} are stored in macro feature. Current version: {curParamVersion}")
        {
        }
    }
}
