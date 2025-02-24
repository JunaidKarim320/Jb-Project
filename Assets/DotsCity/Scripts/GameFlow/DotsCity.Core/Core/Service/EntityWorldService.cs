﻿using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;

namespace Spirit604.DotsCity.Core
{
    public class EntityWorldService : MonoBehaviour, IEntityWorldService
    {
        public event Action OnEntitySceneUnload = delegate { };

        public IEnumerator DisposeWorldRoutine(bool autoRecreateWorld = true)
        {
            yield return new WaitForEndOfFrame();
            DisposeWorld(autoRecreateWorld);
        }

        public void DisposeWorld(bool autoRecreateWorld = true)
        {
            OnEntitySceneUnload();
            World.DisposeAllWorlds();

            if (autoRecreateWorld)
                CreateWorld();
        }

        public void CreateWorld()
        {
            DefaultWorldInitialization.Initialize("Default World", false);
        }
    }
}