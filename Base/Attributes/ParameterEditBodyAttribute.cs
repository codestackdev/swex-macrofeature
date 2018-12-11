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

namespace CodeStack.SwEx.MacroFeature.Attributes
{
    public class ParameterEditBodyAttribute : Attribute
    {
        [Obsolete("This property is deprecated")]
        internal int BodyIndex { get; private set; } = -1;

        public ParameterEditBodyAttribute()
        {
        }

        [Obsolete("This constructor is deprecated")]
        public ParameterEditBodyAttribute(int bodyIndex)
        {
            BodyIndex = bodyIndex;
        }
    }
}
