using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Base
{
    public interface IMacroFeatureHandler
    {
        void Init(ISldWorks app, IModelDoc2 model, IFeature feat);
        void Unload();
    }
}
