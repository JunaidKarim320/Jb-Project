using Spirit604.Attributes;
using Spirit604.DotsCity.Core;
using Spirit604.Extensions;
using Spirit604.Gameplay.Road;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Simulation.Road
{
    [DefaultExecutionOrder(-10000)]
    public class TrafficLightHybridService : SingletonMonoBehaviour<TrafficLightHybridService>
    {
        [ShowIfNull]
        [SerializeField] private CitySettingsInitializerBase citySettingsInitializer;

        [Tooltip("Enable this option if you want to use the Monobehaviour script to read the light state of the crossroad using the <b>'GetLightState'</b> method by crossroad ID")]
        [SerializeField] private bool registerLightStates;
        [SerializeField] private bool registerLightEntities;

        private Dictionary<int, List<ITrafficLightListener>> lightListeners = new Dictionary<int, List<ITrafficLightListener>>();
        private Dictionary<int, LightState> lightStateData;
        private bool dotsSimulation;

        private GeneralSettingDataCore Settings => citySettingsInitializer ? citySettingsInitializer.GetSettings<GeneralSettingDataCore>() : null;
        private bool DOTSSimulation => Settings ? Settings.DOTSSimulation : true;

        private void Start()
        {
            if (registerLightStates)
            {
                lightStateData = new Dictionary<int, LightState>();
            }

            dotsSimulation = DOTSSimulation;

            if (!Settings)
            {
                Debug.LogError("TrafficLightHybridService. CitySettingsInitializer not assigned.");
            }

            if (!dotsSimulation)
            {
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TrafficLightHybridEventSystem>().Initialize(this);

                if (registerLightEntities)
                {
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TrafficLightHybridDataSystem>().Initialize();
                }
            }
        }

        public void AddListener(ITrafficLightListener listener, int id)
        {
            if (dotsSimulation)
                return;

            if (id == 0)
                return;

            if (!lightListeners.ContainsKey(id))
            {
                lightListeners.Add(id, new List<ITrafficLightListener>());
            }

            lightListeners[id].Add(listener);
        }

        public void RemoveListener(ITrafficLightListener listener, int id)
        {
            if (dotsSimulation)
                return;

            if (id == 0)
                return;

            if (lightListeners.ContainsKey(id))
            {
                lightListeners[id].TryToRemove(listener);

                if (lightListeners[id].Count == 0)
                {
                    lightListeners.Remove(id);
                }
            }
        }

        public LightState GetLightState(int id)
        {
            if (lightStateData.TryGetValue(id, out var lightState))
            {
                return lightState;
            }

            return LightState.Uninitialized;
        }

        public bool ForceLightState(int id, LightState lightState)
        {
            return World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TrafficLightHybridDataSystem>().SetForceState(id, lightState);
        }

        public bool RemoveForceLightState(int id)
        {
            return World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TrafficLightHybridDataSystem>().RemoveForceState(id);
        }

        internal void UpdateState(int id, LightState state)
        {
            if (registerLightStates)
            {
                if (lightStateData.ContainsKey(id))
                {
                    lightStateData[id] = state;
                }
                else
                {
                    lightStateData.Add(id, state);
                }
            }

            if (!lightListeners.ContainsKey(id))
                return;

            var list = lightListeners[id];

            for (int i = 0; i < list.Count; i++)
            {
                list[i].UpdateState(state);
            }
        }
    }
}
