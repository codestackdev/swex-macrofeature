using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeStack.SwEx.MacroFeature.Base;
using SolidWorks.Interop.sldworks;
using System.ComponentModel;
using SolidWorks.Interop.swconst;

namespace CodeStack.SwEx.MacroFeature.Core
{
    public class ObsoleteMacroFeatureEx : MacroFeatureEx
    {
        protected virtual string ObsoleteFeatureRebuildError
        {
            get
            {
                return "This feature is obsolete";
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return MacroFeatureRebuildResult.FromStatus(false, ObsoleteFeatureRebuildError);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            return false;
        }
    }
    
    public class ObsoleteMacroFeatureEx<TReplacementMacroFeature> : ObsoleteMacroFeatureEx
        where TReplacementMacroFeature : MacroFeatureEx
    {
        protected override string ObsoleteFeatureRebuildError
        {
            get
            {
                return $"{base.ObsoleteFeatureRebuildError} and has been superseded. Click 'Edit Feature' to replace feature";
            }
        }

        protected virtual string ReplaceFeatureMessage
        {
            get
            {
                return "This features is obsolete. It is required to replace it. Do you want to replace this feature?";
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            if (app.SendMsgToUser2(ReplaceFeatureMessage,
                (int)swMessageBoxIcon_e.swMbWarning,
                (int)swMessageBoxBtn_e.swMbYesNo) == (int)swMessageBoxResult_e.swMbHitYes)
            {
                var newFeat = model.FeatureManager
                    .ReplaceComFeature<TReplacementMacroFeature>(feature);

                return newFeat != null;
            }

            return true;
        }
    }
}
