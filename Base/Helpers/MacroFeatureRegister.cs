using CodeStack.SwEx.MacroFeature.Base;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CodeStack.SwEx.MacroFeature.Helpers
{
    internal class MacroFeatureRegister<THandler> : IDisposable
        where THandler : class, IMacroFeatureHandler, new()
    {
        private class ModelDictionary : Dictionary<IModelDoc2, MacroFeatureDictionary>
        {
        }

        private class MacroFeatureDictionary : Dictionary<IFeature, THandler>
        {
        }

        private readonly ModelDictionary m_Register;
        private readonly Dictionary<IModelDoc2, MacroFeatureLifecycleManager> m_LifecycleManagers;
        private readonly string m_BaseName;
        
        internal MacroFeatureRegister(string baseName)
        {           
            m_BaseName = baseName;
            m_Register = new ModelDictionary();
            m_LifecycleManagers = new Dictionary<IModelDoc2, MacroFeatureLifecycleManager>();
        }

        internal THandler EnsureFeatureRegistered(ISldWorks app, IModelDoc2 model, IFeature feat, out bool isNew)
        {
            isNew = false;

            MacroFeatureDictionary featsDict;

            if (!m_Register.TryGetValue(model, out featsDict))
            {
                featsDict = new MacroFeatureDictionary();
                m_Register.Add(model, featsDict);

                var lcm = new MacroFeatureLifecycleManager(model, m_BaseName);
                lcm.ModelDisposed += OnModelDisposed;
                lcm.FeatureDisposed += OnFeatureDisposed;
                m_LifecycleManagers.Add(model, lcm);
            }

            THandler handler = null;
            if (!featsDict.TryGetValue(feat, out handler))
            {
                handler = new THandler();
                featsDict.Add(feat, handler);
                handler.Init(app, model, feat);
                isNew = true;
            }

            return handler;
        }

        private void OnFeatureDisposed(IModelDoc2 model, IFeature feat)
        {
            DestroyInRegister(model, feat);
        }

        private void OnModelDisposed(IModelDoc2 model)
        {
            DestroyInRegister(model);
        }

        private void DestroyInRegister(IModelDoc2 model)
        {
            MacroFeatureLifecycleManager lcm;

            if (m_LifecycleManagers.TryGetValue(model, out lcm))
            {
                m_LifecycleManagers.Remove(model);
            }
            else
            {
                Debug.Assert(false, "Model is not registered");
            }

            MacroFeatureDictionary modelDict;

            if (m_Register.TryGetValue(model, out modelDict))
            {
                foreach (var handler in modelDict.Values)
                {
                    handler.Unload();
                }

                m_Register.Remove(model);
            }
            else
            {
                Debug.Assert(false, "Model is not registered");
            }
        }

        private void DestroyInRegister(IModelDoc2 model, IFeature feat)
        {
            MacroFeatureDictionary modelDict;

            if (m_Register.TryGetValue(model, out modelDict))
            {
                THandler handler;

                if (modelDict.TryGetValue(feat, out handler))
                {
                    handler.Unload();

                    modelDict.Remove(feat);
                }
                else
                {
                    Debug.Assert(false, "Handler is not registered");
                }
            }
            else
            {
                Debug.Assert(false, "Model is not registered");
            }
        }
        
        public void Dispose()
        {
            foreach (var model in m_Register.Keys)
            {
                DestroyInRegister(model);
            }

            m_Register.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
