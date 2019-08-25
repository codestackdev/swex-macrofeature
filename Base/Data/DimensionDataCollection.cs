//**********************
//SwEx.MacroFeature - framework for developing macro features in SOLIDWORKS
//Copyright(C) 2019 www.codestack.net
//License: https://github.com/codestackdev/swex-macrofeature/blob/master/LICENSE
//Product URL: https://www.codestack.net/labs/solidworks/swex/macro-feature
//**********************

using CodeStack.SwEx.MacroFeature.Data;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using CodeStack.SwEx.MacroFeature;

namespace CodeStack.SwEx.MacroFeature.Data
{
    /// <summary>
    /// Collection of dimensions associated with this macro feature
    /// </summary>
    public class DimensionDataCollection : ReadOnlyCollection<DimensionData>, IDisposable
    {
        internal DimensionDataCollection(IDisplayDimension[] dispDims, string[] dispDimParams)
            : base(new List<DimensionData>())
        {
            if (dispDims != null && dispDimParams != null)
            {
                if (dispDims.Length == dispDimParams.Length)
                {
                    for (int i = 0; i < dispDims.Length; i++)
                    {
                        this.Items.Add(new DimensionData(dispDims[i] as IDisplayDimension, dispDimParams[i]));
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException("Dimensions and parameters lentgth do not match");
                }
            }
        }

        public DimensionData this[string name]
        {
            get
            {
                return this.First(d => d.Name == name);
            }
        }

        /// <summary>
        /// Disposing object
        /// </summary>
        public void Dispose()
        {
            if (Count > 0)
            {
                foreach (var item in this.Items)
                {
                    item.Dispose();
                }

                GC.Collect();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}
