//**********************
//SwEx - development tools for SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Base
{
    public class MacroFeatureRebuildBodyResult : MacroFeatureRebuildResult
    {
        internal MacroFeatureRebuildBodyResult(IBody2 body) : this(body, null, false)
        {
        }

        internal MacroFeatureRebuildBodyResult(IBody2 body, IMacroFeatureData featData, bool updateEntityIds) : base(body)
        {
            if (updateEntityIds)
            {
                if (featData == null)
                {
                    throw new ArgumentNullException(nameof(featData));
                }

                object faces;
                object edges;
                featData.GetEntitiesNeedUserId(body, out faces, out edges);

                if (faces is object[])
                {
                    int nextId = 0;

                    foreach (Face2 face in faces as object[])
                    {
                        featData.SetFaceUserId(face, nextId++, 0);
                    }
                }

                if (edges is object[])
                {
                    int nextId = 0;

                    foreach (Edge edge in edges as object[])
                    {
                        featData.SetEdgeUserId(edge, nextId++, 0);
                    }
                }
            }
        }
    }
}
