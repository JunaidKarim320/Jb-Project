using Spirit604.Extensions;
using Spirit604.Gameplay.Road;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Spirit604.CityEditor.Road
{
    public partial class RoadSegmentCreator : MonoBehaviour
    {
#if UNITY_EDITOR
        public void OnInspectorEnabled()
        {
            SwitchTempEnabledState(true);

            if (tempParkingLineSettings == null)
            {
                tempParkingLineSettings = ScriptableObject.CreateInstance<ParkingLineSettingsContainer>();
            }

            CheckLightData();

            OnConfigChanged();

            InitOuter(true);

            foreach (var createdLight in createdLights)
            {
                if (!createdLight)
                {
                    continue;
                }

                if (createdLight.transform.parent != trafficLights && createdLight.transform.parent != pedestrianLights)
                {
                    currentLightParent = createdLight.transform.parent;
                    break;
                }
            }
        }

        public void OnInspectorDisabled()
        {
            SwitchTempEnabledState(false);

            CurrentExtrudeState = ExtrudeState.Default;
            Dispose();
        }

        public void InitOuter(bool force = false)
        {
            if (allConnectedOuterPaths != null && !force)
            {
                return;
            }

            allConnectedOuterPaths = new Dictionary<TrafficNode, List<Path>>();

            foreach (var trafficNode in trafficLightCrossroad.TrafficNodes)
            {
                if (!trafficNode)
                {
                    continue;
                }

                trafficNode.IterateAllPaths((path) =>
                {
                    path.ShowInfoWaypoints = false;
                }, true);

                var paths = trafficNode.GetAllConnectedOuterPaths();

                allConnectedOuterPaths.Add(trafficNode, paths);
            }
        }

        public void Dispose()
        {
            allConnectedOuterPaths = null;
        }

        public void OnEscapeClicked()
        {
            ResetLaneExtrude();
        }

        public void OnConfigChanged()
        {
            roadSegmentCreatorConfig?.CheckForNullConfigs();
            roadSegmentCreatorConfig?.CheckForSavePath();

            SyncParkingConfigHeaders();
        }

        public void OnCrosswalkSettingsChanged()
        {
            UpdateCrosswalk();
        }

        public void OnPathSelectionChanged()
        {
            this.selectedPath = GetPathByIndex();
            OnPathSelectionChangedEvent.Invoke(this.selectedPath);
        }

        public void OnWayPointStraightCountChanged()
        {
            if (ObjectIsPrefab())
            {
                return;
            }

            Action<Path> callback = (Path path) =>
            {
                if (path.PathCurveType == PathCurveType.StraightLine)
                {
                    path.WayPointsCountPerCurve = wayPointStraightRoadCount;
                    path.CreatePath(true);
                }
            };

            IterateAllTrafficNodesPath(callback);
        }

        public void OnWayPointTurnCountChanged()
        {
            if (ObjectIsPrefab())
            {
                return;
            }

            Action<Path> callback = (Path path) =>
            {
                if (path.PathCurveType != PathCurveType.StraightLine)
                {
                    path.WayPointsCountPerCurve = wayPointTurnCurveCount;
                    path.CreatePath(true);
                }
            };

            IterateAllTrafficNodesPath(callback);
        }

        public void OnStraightSpeedLimitChanged()
        {
            Action<Path> callback = (Path path) =>
            {
                if (path.PathCurveType == PathCurveType.StraightLine)
                {
                    path.PathSpeedLimit = straightRoadPathSpeedLimit;
                    path.Priority = straightRoadPriority;
                    path.ResetSpeedLimit();
                }
            };

            IterateAllTrafficNodesPath(callback);
        }

        public void OnTurnSpeedLimitChanged()
        {
            Action<Path> callback = (Path path) =>
            {
                if (path.PathCurveType != PathCurveType.StraightLine)
                {
                    path.PathSpeedLimit = turnRoadPathSpeedLimit;
                    path.Priority = turnRoadPriority;
                    path.ResetSpeedLimit();
                }
            };

            IterateAllTrafficNodesPath(callback);
        }

        public void OnLightSettingsChanged()
        {
            ClearLights();
            TryToAddLights();
        }

        public void UndoClicked()
        {
            if (IsCustomStraight())
            {
                RecalculateCustomPath(false);
            }

            if (ParkingBuilderMode)
            {
                RecalcuteParkingPaths();
            }
        }

        public void RecalculateAllOuterConnectedPaths(bool recordUndo = true)
        {
            if (!roadSegmentCreatorConfig.AutoRecalculateExternalPaths)
            {
                return;
            }

            for (int i = 0; i < createdTrafficNodes.Count; i++)
            {
                RecalculateNodeOuterConnections(createdTrafficNodes[i], recordUndo);
            }
        }

        public void RecalculateNodeOuterConnections(TrafficNode trafficNode, bool recordUndo = false)
        {
            if (!roadSegmentCreatorConfig.AutoRecalculateExternalPaths)
            {
                return;
            }

            if (allConnectedOuterPaths == null || !allConnectedOuterPaths.ContainsKey(trafficNode))
            {
                return;
            }

            var paths = allConnectedOuterPaths[trafficNode];

            foreach (var path in paths)
            {
                if (!path)
                {
                    continue;
                }

                path.AttachConnectedNode(true, true, recordUndo);
            }
        }

        public void ProcessDuplicate()
        {
            var trafficNodes = GetComponentsInChildren<TrafficNode>();
            var pedestrianNodes = GetComponentsInChildren<PedestrianNode>();

            var disconnectNodes = new List<PedestrianNode>();

            for (int i = 0; i < pedestrianNodes.Length; i++)
            {
                var node = pedestrianNodes[i];

                node.IterateConnectedNodes(connectedNode =>
                {
                    if (Array.IndexOf(pedestrianNodes, connectedNode) == -1)
                    {
                        disconnectNodes.Add(connectedNode);
                    }
                });

                foreach (var disconnectNode in disconnectNodes)
                {
                    node.RemoveConnection(disconnectNode);
                }

                disconnectNodes.Clear();
            }

            var paths = new List<Path>();

            foreach (var trafficNode in trafficNodes)
            {
                trafficNode.IterateExternalPaths(path =>
                {
                    if (path.ConnectedTrafficNode == null || Array.IndexOf(trafficNodes, path.ConnectedTrafficNode) == -1)
                        paths.Add(path);
                });

                foreach (var path in paths)
                {
                    trafficNode.TryToRemovePath(path);
                    path.DestroyPath(false);
                }

                trafficNode.ExternalLanes.Clear();
                EditorSaver.SetObjectDirty(trafficNode);

                paths.Clear();
            }

            RegenerateAllIds(trafficNodes, pedestrianNodes);
        }

        public void RegenerateAllIds()
        {
            var trafficNodes = GetComponentsInChildren<TrafficNode>();
            var pedestrianNodes = GetComponentsInChildren<PedestrianNode>();

            RegenerateAllIds(trafficNodes, pedestrianNodes);
        }

        public void RegenerateAllIds(TrafficNode[] trafficNodes, PedestrianNode[] pedestrianNodes)
        {
            for (int i = 0; i < pedestrianNodes.Length; i++)
            {
                var node = pedestrianNodes[i];

                node.GenerateId(true);
                node.ClearCustomConnectionData();
            }

            foreach (var trafficNode in trafficNodes)
            {
                trafficNode.GenerateIds(true);
            }

            trafficLightCrossroad.TryToGenerateID(true);
        }

#endif
    }
}