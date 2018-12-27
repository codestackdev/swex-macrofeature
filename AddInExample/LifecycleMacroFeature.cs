using CodeStack.SwEx.MacroFeature.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using System.Runtime.InteropServices;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Example.Properties;

namespace CodeStack.SwEx.MacroFeature.Example
{
    public class LifecycleMacroFeatureParams
    {
    }

    public class LifecycleMacroFeatureHandler : IMacroFeatureHandler
    {
        private ISldWorks m_App;
        IModelDoc2 m_Model;
        IFeature m_Feat;

        public void Init(ISldWorks app, IModelDoc2 model, IFeature feat)
        {
            m_App = app;
            m_Model = model;
            m_Feat = feat;

            m_App.SendMsgToUser($"{m_Model.GetTitle()}\\{m_Feat.Name} loaded");
        }

        public void Unload()
        {
            m_App.SendMsgToUser($"{m_Model.GetTitle()}\\{m_Feat.Name} unloaded");
        }

        public void Rebuild()
        {
            m_App.SendMsgToUser($"{m_Model.GetTitle()}\\{m_Feat.Name} rebuilt");
        }
    }

    [ComVisible(true)]
    [FeatureIcon(typeof(Resources), nameof(Resources.codestack), "CodeStack\\MacroFeatureExample\\Icons")]
    public class LifecycleMacroFeature : MacroFeatureEx<LifecycleMacroFeatureParams, LifecycleMacroFeatureHandler>
    {
        protected override MacroFeatureRebuildResult OnRebuild(LifecycleMacroFeatureHandler handler,
            LifecycleMacroFeatureParams parameters)
        {
            handler.Rebuild();

            return MacroFeatureRebuildResult.FromStatus(true);
        }
    }
}
