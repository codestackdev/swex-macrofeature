using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Exceptions
{
    public class FutureVersionParametersException : Exception
    {
        public FutureVersionParametersException(Type paramType, Version curParamVersion, Version paramVersion)
            : base($"Future version of parameters '{paramType.FullName}' {paramVersion} are stored in macro feature. Current version: {curParamVersion}")
        {
        }
    }
}
