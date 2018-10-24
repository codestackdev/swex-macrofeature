using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Helpers
{
    internal class MacroFeatureLifecycleManager
    {
        internal event Action<IModelDoc2> ModelDisposed;
        internal event Action<IModelDoc2, IFeature> FeatureDisposed;

        private IModelDoc2 m_Model;
        private readonly string m_MacroFeatBaseName;

        private Dictionary<string, IFeature> m_UnloadQueue;

        internal MacroFeatureLifecycleManager(IModelDoc2 model, string macroFeatBaseName)
        {
            m_Model = model;
            m_MacroFeatBaseName = macroFeatBaseName;

            m_UnloadQueue = new Dictionary<string, IFeature>(
                StringComparer.CurrentCultureIgnoreCase);

            AttachEvents();
        }

        private void AttachEvents()
        {
            if (m_Model is IPartDoc)
            {
                (m_Model as PartDoc).DeleteItemPreNotify += OnDeleteItemPreNotify;
                (m_Model as PartDoc).DeleteItemNotify += OnDeleteItemNotify;
                (m_Model as PartDoc).DestroyNotify2 += OnDestroyNotify2;
            }
            else if (m_Model is IAssemblyDoc)
            {
                (m_Model as AssemblyDoc).DeleteItemPreNotify += OnDeleteItemPreNotify;
                (m_Model as AssemblyDoc).DeleteItemNotify += OnDeleteItemNotify;
                (m_Model as AssemblyDoc).DestroyNotify2 += OnDestroyNotify2;
            }
            else if (m_Model is IDrawingDoc)
            {
                (m_Model as DrawingDoc).DeleteItemPreNotify += OnDeleteItemPreNotify;
                (m_Model as DrawingDoc).DeleteItemNotify += OnDeleteItemNotify;
                (m_Model as DrawingDoc).DestroyNotify2 += OnDestroyNotify2;
            }
        }

        //TODO: remove this and unload element in the pre-notification
        private int OnDeleteItemNotify(int EntityType, string itemName)
        {
            if (EntityType == (int)swNotifyEntityType_e.swNotifyFeature)
            {
                IFeature feat;
                if (m_UnloadQueue.TryGetValue(itemName, out feat))
                {
                    FeatureDisposed?.Invoke(m_Model, feat);
                    m_UnloadQueue.Remove(itemName);
                    
                    feat = null;
                    GC.Collect();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
            }

            return 0;
        }

        private int OnDeleteItemPreNotify(int EntityType, string itemName)
        {
            if (EntityType == (int)swNotifyEntityType_e.swNotifyFeature)
            {
                var feat = GetFeatureByName(itemName);

                if (feat != null)
                {
                    if (feat.GetTypeName2() == "MacroFeature")
                    {
                        if ((feat.GetDefinition() as IMacroFeatureData).GetBaseName() == m_MacroFeatBaseName)
                        {
                            if (!m_UnloadQueue.ContainsKey(itemName))
                            {
                                m_UnloadQueue.Add(itemName, feat);
                            }
                            else
                            {
                                Debug.Assert(false, "DeleteItemNotify is not called after DeleteItemPreNotify");
                            }
                        }
                    }
                }
                else
                {
                    Debug.Assert(false, "Feature name supplied by DeleteItemPreNotify doesn't exist");
                }
            }

            return 0;
        }

        private bool TryGetMacroFeature(int entType, string name, out IFeature macroFeat)
        {
            macroFeat = null;

            if (entType == (int)swNotifyEntityType_e.swNotifyFeature)
            {
                var feat = GetFeatureByName(name);

                if (feat != null)
                {
                    if (feat.GetTypeName2() == "MacroFeature")
                    {
                        if ((feat.GetDefinition() as IMacroFeatureData).GetBaseName() == m_MacroFeatBaseName)
                        {
                            macroFeat = feat;
                            return true;
                        }
                    }
                }
                else
                {
                    Debug.Assert(false, "Feature name supplied by DeleteItemPreNotify doesn't exist");
                }
            }

            return false;
        }

        private IFeature GetFeatureByName(string name)
        {
            if (m_Model is IPartDoc)
            {
                return (m_Model as IPartDoc).FeatureByName(name) as IFeature;
            }
            else if (m_Model is IAssemblyDoc)
            {
                return (m_Model as IAssemblyDoc).FeatureByName(name) as IFeature;
            }
            else if (m_Model is IDrawingDoc)
            {
                return (m_Model as IDrawingDoc).FeatureByName(name) as IFeature;
            }
            else
            {
                return null;
            }
        }

        private void DetachEvents()
        {
            if (m_Model is IPartDoc)
            {
                (m_Model as PartDoc).DeleteItemPreNotify -= OnDeleteItemPreNotify;
                (m_Model as PartDoc).DeleteItemNotify -= OnDeleteItemNotify;
                (m_Model as PartDoc).DestroyNotify2 -= OnDestroyNotify2;
            }
            else if (m_Model is IAssemblyDoc)
            {
                (m_Model as AssemblyDoc).DeleteItemPreNotify -= OnDeleteItemPreNotify;
                (m_Model as AssemblyDoc).DeleteItemNotify -= OnDeleteItemNotify;
                (m_Model as AssemblyDoc).DestroyNotify2 -= OnDestroyNotify2;
            }
            else if (m_Model is IDrawingDoc)
            {
                (m_Model as DrawingDoc).DeleteItemPreNotify -= OnDeleteItemPreNotify;
                (m_Model as DrawingDoc).DeleteItemNotify -= OnDeleteItemNotify;
                (m_Model as DrawingDoc).DestroyNotify2 -= OnDestroyNotify2;
            }
        }

        private int OnDestroyNotify2(int DestroyType)
        {
            ModelDisposed?.Invoke(m_Model);
            DetachEvents();

            //Marshal.FinalReleaseComObject(m_Model);
            m_Model = null;
            //m_UnloadQueue.Values.ToList().ForEach(f => Marshal.FinalReleaseComObject(f));
            m_UnloadQueue.Clear();
            GC.Collect();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return 0;
        }
    }
}
