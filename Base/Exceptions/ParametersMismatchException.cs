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
