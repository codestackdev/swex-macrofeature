//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2018 www.codestack.net
//License: https://github.com/codestack-net-dev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Base
{
    /// <summary>
    /// Represents the result of the macro feature regeneration method
    /// which should be returned from <see cref="MacroFeatureEx.OnRebuild(ISldWorks, IModelDoc2, IFeature)"/>
    /// </summary>
    public abstract class MacroFeatureRebuildResult
    {
        /// <summary>
        /// Creates the result from temp body
        /// </summary>
        /// <param name="body">Body to return</param>
        /// <returns>Result</returns>
        /// <remarks>Use this result if you need to macro feature to generate a single body (solid or surface)</remarks>
        public static MacroFeatureRebuildResult FromBody(IBody2 body)
        {
            return new MacroFeatureRebuildBodyResult(body);
        }

        /// <inheritdoc cref="FromBody(IBody2)"/>
        /// <param name="featData">Pointer to feature data</param>
        /// <param name="updateEntityIds">Indicates if it is required to automatically assign the user ids to body entities</param>
        public static MacroFeatureRebuildResult FromBody(IBody2 body, IMacroFeatureData featData, bool updateEntityIds = true)
        {
            return new MacroFeatureRebuildBodyResult(featData, updateEntityIds, body);
        }

        /// <inheritdoc cref="FromBody(IBody2, IMacroFeatureData, bool)"/>
        /// <summary>
        /// Create the result from array of bodies
        /// </summary>
        /// <param name="bodies">Array of temp bodies</param>
        /// <remarks>Use this method if it is required to have multiple bodies in the macro feature</remarks>
        public static MacroFeatureRebuildResult FromBodies(IBody2[] bodies, IMacroFeatureData featData, bool updateEntityIds = true)
        {
            return new MacroFeatureRebuildBodyResult(featData, updateEntityIds, bodies);
        }

        [Obsolete("Deprecated. Use FromBodies method instead")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public static MacroFeatureRebuildResult FromPattern(IBody2[] bodiesPattern)
        {
            return new MacroFeatureRebuildPatternResult(bodiesPattern);
        }

        /// <summary>
        /// Returns the status of the rebuild operation
        /// </summary>
        /// <param name="status">True if regeneration successful, False if not</param>
        /// <param name="error">Error message to be displayed in the What's Wrong dialog if <paramref name="status"/> equals to false</param>
        /// <returns>Result</returns>
        /// <remarks>Use this method if macro feature doesn't modify or create new bodies</remarks>
        public static MacroFeatureRebuildResult FromStatus(bool status, string error = "")
        {
            return new MacroFeatureRebuldStatusResult(status, error);
        }

        private readonly object m_Result;

        internal MacroFeatureRebuildResult(object result)
        {
            m_Result = result;
        }

        internal object GetResult()
        {
            return m_Result;
        }
    }
}
