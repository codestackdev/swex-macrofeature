using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolidWorks.Interop.sldworks
{
    internal static class SldWorksEx
    {
        internal static bool SupportsHighResIcons(this ISldWorks app)
        {
            const int SW_2017_REV = 25;

            var majorRev = int.Parse(app.RevisionNumber().Split('.')[0]);

            return majorRev >= SW_2017_REV;
        }
    }
}
