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
        internal DimensionDataCollection(IDisplayDimension[] dispDims)
            : base(new List<DimensionData>())
        {
            if (dispDims != null)
            {
                for (int i = 0; i < dispDims.Length; i++)
                {
                    this.Items.Add(new DimensionData(dispDims[i] as IDisplayDimension));
                }
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
