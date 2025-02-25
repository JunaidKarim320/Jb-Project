using Spirit604.Collections.Dictionary;
using Spirit604.Extensions;
using Spirit604.Gameplay.Road;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Spirit604.CityEditor.Road
{
    [CreateAssetMenu(fileName = "RoadSegmentCreatorConfig", menuName = CityEditorBookmarks.CITY_EDITOR_LEVEL_CONFIG_EDITOR_ROAD_PATH + "RoadSegmentCreatorConfig")]
    public class RoadSegmentCreatorConfig : ScriptableObject
    {
        private const string RelativeSavePath = "Roads/";

        [SerializeField] private TrafficNode trafficNodePrefab;
        [SerializeField] private PedestrianNode pedestrianNodePrefab;
        [SerializeField] private Path pathPrefab;
        [SerializeField] private LightPrefabsDataDictionary lightPrefabsData;
        [SerializeField] private GameObject pedestrianLightPrefab;
        [SerializeField] private RoadSegmentCreator.ViewType viewType = RoadSegmentCreator.ViewType.Toolbar;
        [SerializeField][Range(0.1f, 20f)] private float laneWidth = 3.8f;
        [SerializeField] private bool customSubLaneWidth;
        [SerializeField][Range(0.1f, 20f)] private float subLaneWidth = 3.8f;
        [SerializeField] private Vector3 crossWalkOffset = new Vector3(0.95f, 0, -1.5f);
        [SerializeField] private RoadSegmentCreator.TempNodeSettings newNodeSettings;
        [SerializeField] private int selectedParkingPresetIndex = 0;
        [SerializeField] private string savePrefabPath;
        [SerializeField] private List<ParkingLineSettingsContainer> parkingLineConfigs = new List<ParkingLineSettingsContainer>();
        [SerializeField] private bool showSegmentPositionHandle = true;
        [SerializeField] private bool autoRecalculateExternalPaths = true;
        [SerializeField] private bool snapSegmentPosition = true;
        [SerializeField] private bool addHalfOffset = true;
        [SerializeField][Range(0.01f, 20f)] private float customSnapSize = 1f;

        [Tooltip("Offset between snap point and the node (Y axis)")]
        [SerializeField][Range(-10, 20f)] private float snapSurfaceOffset;

        [SerializeField] private LayerMask snapLayerMask = ~0;
        [SerializeField] private string roadParkingConfigSavePath;
        [SerializeField] private bool debugCast;
        [SerializeField] private bool snapOnCreate = true;

        [SerializeField] private int extrudeLaneCount = 1;
        [SerializeField] private bool selectAfterCreation = true;
        [SerializeField] private TrafficNodeDirectionType pathDirection = TrafficNodeDirectionType.Right;

        [Tooltip("<b>Strip Nodes</b> : strip out any unnecessary nodes if they are on the same line as each other")]
        [SerializeField] private RoadSegmentCreator.AdditionalStraightRoadSettings additionalSettings;

        [Tooltip("Minimum angle between the previous straight line and the current node, if the angle is less than the specified angle, the node is deleted")]
        [SerializeField][Range(0, 10f)] private float minStripAngle = 1f;

        [Tooltip("Minimum distance between cutout nodes")]
        [SerializeField][Range(0, 100f)] private float minStripDistance = 20f;

        [SerializeField][Range(0, 100f)] private float minSpawnNodeOffset = 15f;

        [Header("Hotkeys")]
        [SerializeField]
        private HotkeyDictionary hotkeyDictionary = new HotkeyDictionary()
        {
            { "RotateRoad", KeyCode.CapsLock },
            { "SelectNode", KeyCode.R },
        };

        public TrafficNode TrafficNodePrefab { get => trafficNodePrefab; set => trafficNodePrefab = value; }
        public PedestrianNode PedestrianNodePrefab { get => pedestrianNodePrefab; set => pedestrianNodePrefab = value; }
        public Path PathPrefab { get => pathPrefab; set => pathPrefab = value; }
        public LightPrefabsDataDictionary LightPrefabs { get => lightPrefabsData; set => lightPrefabsData = value; }
        public GameObject PedestrianLightPrefab { get => pedestrianLightPrefab; set => pedestrianLightPrefab = value; }
        public RoadSegmentCreator.ViewType ViewType { get => viewType; set => viewType = value; }
        public float LaneWidth { get => laneWidth; set => laneWidth = value; }
        public float SubLaneWidth { get => customSubLaneWidth ? subLaneWidth : laneWidth; set => subLaneWidth = value; }
        public Vector3 CrossWalkOffset { get => crossWalkOffset; set => crossWalkOffset = value; }
        public RoadSegmentCreator.TempNodeSettings NewNodeSettings { get => newNodeSettings; set => newNodeSettings = value; }
        public string SavePrefabPath { get => savePrefabPath; set => savePrefabPath = value; }
        public int SelectedParkingPresetIndex { get => selectedParkingPresetIndex; set => selectedParkingPresetIndex = value; }
        public ParkingLineSettingsContainer GetParkingLineConfig => parkingLineConfigs.Count > selectedParkingPresetIndex ? parkingLineConfigs[selectedParkingPresetIndex] : null;
        public bool ShowSegmentPositionHandle { get => showSegmentPositionHandle; set => showSegmentPositionHandle = value; }
        public bool AutoRecalculateExternalPaths { get => autoRecalculateExternalPaths; set => autoRecalculateExternalPaths = value; }
        public bool SnapSegmentPosition { get => snapSegmentPosition; set => snapSegmentPosition = value; }
        public bool AddHalfOffset { get => addHalfOffset; set => addHalfOffset = value; }
        public float CustomSnapSize { get => customSnapSize; set => customSnapSize = value; }
        public float SnapSurfaceOffset { get => snapSurfaceOffset; set => snapSurfaceOffset = value; }
        public LayerMask SnapLayerMask { get => snapLayerMask; set => snapLayerMask = value; }
        public string RoadParkingConfigSavePath { get => roadParkingConfigSavePath; set => roadParkingConfigSavePath = value; }
        public bool DebugCast { get => debugCast; set => debugCast = value; }
        public bool SnapOnCreate { get => snapOnCreate; set => snapOnCreate = value; }
        public TrafficNodeDirectionType PathDirection { get => pathDirection; set => pathDirection = value; }
        public RoadSegmentCreator.AdditionalStraightRoadSettings AdditionalSettings { get => additionalSettings; set => additionalSettings = value; }
        public float MinStripAngle { get => minStripAngle; set => minStripAngle = value; }
        public float MinStripDistance { get => minStripDistance; set => minStripDistance = value; }
        public float MinSpawnNodeOffset { get => minSpawnNodeOffset; set => minSpawnNodeOffset = value; }
        public bool SelectAfterCreation { get => selectAfterCreation; set => selectAfterCreation = value; }
        public int ExtrudeLaneCount { get => extrudeLaneCount; set => extrudeLaneCount = value; }

        public void AddParkingConfig(ParkingLineSettingsContainer parkingLineSettings)
        {
            parkingLineConfigs.TryToAdd(parkingLineSettings);
        }

        public void RemoveParkingConfig(ParkingLineSettingsContainer parkingLineSettings)
        {
            parkingLineConfigs.TryToRemove(parkingLineSettings);
        }

        public void CheckForSavePath()
        {
#if UNITY_EDITOR
            if (!AssetDatabase.IsValidFolder(savePrefabPath))
            {
                ResetSavePath();
            }

            if (!AssetDatabase.IsValidFolder(roadParkingConfigSavePath))
            {
                ResetParkingSavePath();
            }
#endif
        }

        public void ResetSavePath()
        {
            savePrefabPath = CityEditorBookmarks.PREFAB_ROOT_PATH + RelativeSavePath;
            EditorSaver.SetObjectDirty(this);
        }

        public void ResetParkingSavePath()
        {
            roadParkingConfigSavePath = CityEditorBookmarks.CITY_EDITOR_CONFIGS_PATH;
            EditorSaver.SetObjectDirty(this);
        }

        public string[] GetParkingConfigNames()
        {
            if (parkingLineConfigs.Count > 0)
            {
                return parkingLineConfigs.Where(a => a != null).Select(a => a.name).ToArray();
            }

            return new string[0];
        }

        public void CheckForNullConfigs()
        {
            int index = 0;

            bool changed = false;

            while (index < parkingLineConfigs.Count)
            {
                if (parkingLineConfigs[index] == null)
                {
                    parkingLineConfigs.RemoveAt(index);
                    changed = true;
                }
                else
                {
                    index++;
                }
            }

            if (changed)
            {
                EditorSaver.SetObjectDirty(this);
            }
        }

        public KeyCode GetKey(string dataKey, KeyCode defaultKey)
        {
            return hotkeyDictionary.GetKey(dataKey, defaultKey);
        }
    }
}
