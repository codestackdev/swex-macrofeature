//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.Common.Enums;
using CodeStack.SwEx.MacroFeature.Helpers;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Base
{
    /// <summary>
    /// Represents the result of macro feature where macro feature holds the body or pattern of bodies
    /// </summary>
    public class MacroFeatureRebuildBodyResult : MacroFeatureRebuildResult
    {
        [Obsolete, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        internal protected MacroFeatureRebuildBodyResult(params IBody2[] bodies) : this(null, false, bodies)
        {
        }

        private static object GetBodyResult(IBody2[] bodies)
        {
            if (bodies != null)
            {
                if (bodies.Length == 1)
                {
                    return bodies.First();
                }
                else
                {
                    return bodies;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(bodies));
            }
        }

        internal protected MacroFeatureRebuildBodyResult(IMacroFeatureData featData,
            bool updateEntityIds, params IBody2[] bodies) : base(GetBodyResult(bodies))
        {
            if (Context.CurrentApp.IsVersionNewerOrEqual(SwVersion_e.Sw2013, 5))
            {
                featData.EnableMultiBodyConsume = true;
            }

            if (updateEntityIds)
            {
                if (featData == null)
                {
                    throw new ArgumentNullException(nameof(featData));
                }

                for (int i = 0; i < bodies.Length; i++)
                {
                    var body = bodies[i];

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
}
