using Spirit604.Attributes;
using Unity.Scenes;
using UnityEngine;

namespace Spirit604.DotsCity.Core
{
    public abstract class SyncConfigBase : MonoBehaviour
    {
#if UNITY_EDITOR
        private ConfigAutoSyncer configAutoSyncer;
#endif

        private static SubScene subScene;

        public static SubScene SubScene
        {
            get
            {
                if (subScene == null)
                {
                    var go = GameObject.Find("EntitySubScene");

                    if (go != null)
                    {
                        subScene = go.GetComponent<SubScene>();
                    }
                }

                return subScene;
            }
        }

        protected virtual bool AutoSync => true;

        public virtual void Sync()
        {
            if (!AutoSync)
                return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                configAutoSyncer?.Sync();
            }
#endif
        }

#if UNITY_EDITOR

        [OnInspectorEnable]
        private void OnInspectorEnabled()
        {
            if (AutoSync)
            {
                configAutoSyncer = new ConfigAutoSyncer(this, SubScene);
            }
        }

        [OnInspectorDisable]
        private void OnInspectorDisabled()
        {
            if (AutoSync)
            {
                configAutoSyncer?.Dispose();
                configAutoSyncer = null;
            }
        }
#endif
    }
}
