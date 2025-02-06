using Spirit604.CityEditor.Pedestrian;
using Spirit604.CityEditor.Road;
using Spirit604.Extensions;
using Spirit604.Gameplay.Road;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using static Spirit604.CityEditor.CityEditorBookmarks;

namespace Spirit604.CityEditor
{
    public class CityEditorToolbar : ScriptableObject
    {
#if UNITY_EDITOR

        [MenuItem(CITY_CREATE_PATH + "Create RoadSegment", priority = 1)]
        public static GameObject CreateRoadSegmentCreator()
        {
            var roadSegment = CreatePrefab(ROAD_SEGMENT_PREFAB_PATH, VectorExtensions.GetCenterOfSceneView(), true, allowPrefabStage: true);

            if (roadSegment)
            {
                var count = ObjectUtils.FindObjectsOfType<RoadSegment>().Length;
                var indexText = count.ToString();
                roadSegment.name = $"{roadSegment.name}{indexText}";

                var roadParent = ObjectUtils.FindObjectOfType<RoadParent>();

                if (roadParent)
                {
                    roadSegment.transform.SetParent(roadParent.transform, true);
                    EditorGUIUtility.PingObject(roadSegment);
                }

                var creator = roadSegment.GetComponent<RoadSegmentCreator>();

                if (creator && creator.roadSegmentCreatorConfig && creator.roadSegmentCreatorConfig.SnapOnCreate)
                {
                    creator.transform.position = VectorExtensions.GetCenterOfSceneView(true);
                }
            }

            return roadSegment;
        }

        [MenuItem(CITY_CREATE_PATH + "Create TrafficPublicRoute", priority = 2)]
        public static void CreateTrafficPublicRoute()
        {
            CreatePrefab(ROUTE_PREFAB_PATH, Vector3.zero);
        }

        [MenuItem(CITY_CREATE_PATH + "Create PedestrianNode", priority = 4)]
        public static void CreatePedestrianNode()
        {
            CreatePrefab(PEDESTRIAN_NODE_PREFAB_PATH, VectorExtensions.GetCenterOfSceneView(), allowPrefabStage: true);
        }

        [MenuItem(CITY_CREATE_PATH + "City Base", priority = 5)]
        public static void CreateCityBase()
        {
            CreateCityBaseInternal();
        }

        [MenuItem(CITY_CREATE_PATH + "Create PedestrianNodeCreator", priority = 6)]
        public static void CreatePedestrianNodeCreator()
        {
            if (!TryToSelect<PedestrianNodeCreator>())
            {
                CreatePrefab(PEDESTRIAN_NODE_CREATOR_PREFAB_PATH, Vector3.zero);
            }
        }

        [MenuItem(CITY_CREATE_PATH + "Create RoadSegmentPlacer", priority = 7)]
        public static void CreateRoadSegmentPlacer()
        {
            if (!TryToSelect<RoadSegmentPlacer>())
            {
                CreatePrefab(ROADSEGMENTPLACER_PREFAB_PATH, Vector3.zero);
            }
        }

        private static GameObject CreatePrefab(string prefabPath, Vector3 position, bool unpackPrefab = false, bool allowSearchRoot = true, bool allowPrefabStage = false)
        {
            var prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));

            if (prefab == null && allowSearchRoot)
            {
                CityEditorStartup.ForceUpdateRoot();
                return CreatePrefab(prefabPath, position, unpackPrefab, false);
            }

            if (prefab == null)
            {
                Debug.LogError("Prefab not found!");
                return null;
            }

            GameObject createdObject = PrefabExtension.InstantiatePrefab(prefab, unpackPrefab);

            createdObject.transform.position = position;
            createdObject.transform.SetAsLastSibling();

            if (allowPrefabStage)
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();

                if (prefabStage != null)
                {
                    EditorSceneManager.MoveGameObjectToScene(createdObject, prefabStage.scene);
                    createdObject.transform.SetParent(prefabStage.scene.GetRootGameObjects()[0].transform);
                }
            }

            Selection.activeObject = createdObject.gameObject;

            return createdObject;
        }

        private static void CreateCityBaseInternal(bool allowSearchRoot = true)
        {
            var prefabs = AssetDatabaseExtension.TryGetUnityObjectsOfTypeFromPath<GameObject>(CITY_BASE_PATH);

            if ((prefabs == null || prefabs.Count == 0) && allowSearchRoot)
            {
                CityEditorStartup.ForceUpdateRoot();
                CreateCityBaseInternal(false);
                return;
            }

            for (int i = 0; i < prefabs?.Count; i++)
            {
#if !DOTS_SIMULATION
                if (prefabs[i].name == "HubBase")
                    continue;
#else
                if (prefabs[i].name == "Hub")
                    continue;
#endif

                var createdObject = PrefabUtility.InstantiatePrefab(prefabs[i]) as GameObject;
                createdObject.transform.SetAsLastSibling();
            }
        }

        private static bool TryToSelect<T>() where T : MonoBehaviour
        {
            var obj = ObjectUtils.FindObjectOfType<T>();

            if (obj)
            {
                Selection.activeObject = obj;
                return true;
            }

            return false;
        }
#endif
    }
}
