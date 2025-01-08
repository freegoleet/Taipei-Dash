using UnityEngine;
using Traffic;
using System;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class SaveEditor : MonoBehaviour
{
    public SaveSystem LevelSaveSystem { get; private set; } = new SaveSystem();
    public LevelData CurrentLevelData { get; private set; } = default;
    public string CurrentlySelectedLevel {  get; private set; } = string.Empty;
    public string InputLevelName { get; private set; } = string.Empty;
    public string[] LevelNames { get; private set; } = new string[0];
    private TileManager TileManager { get; set; } = null;
    public Action<Dictionary<string, LevelData>> OnLevelChanged = null;

    public void Initialize() {
        OnLevelChanged += SetLevels;
        TileManager = GameService.Instance.GetTileManager();
    }
    
    public void SaveLevel() {
        int cols = TileManager.GridManager.Cols;
        List<TrafficLightData> trafficLights = new List<TrafficLightData>();
        HashSet<TileRoad> roadsWithTLAdded = new HashSet<TileRoad>();
        List<int> crosswalks = new List<int>();

        TileRoadData[] roads = new TileRoadData[TileManager.RoadTileManager.TileCount];
        for (int i = 0; i < roads.Length; i++) {
            TileRoad road = TileManager.RoadTileManager.Tiles[i];
            roads[i] = new TileRoadData() { 
                Position = TrafficUtilities.GetIntPosFromVector2IntPos(road.GridPosition, cols), 
                Connections = road.Connections,
                Facing = road.Facing,
                ManualLines = road.ManualLines,
            };

            if(road.HasTrafficLight == true) {
                if(roadsWithTLAdded.Contains(road) == false) {
                    var tlData = SetTrafficLightData(cols, road, roadsWithTLAdded);
                    trafficLights.Add(tlData);
                }
            }

            if(road.HasCrosswalk == true) {
                crosswalks.Add(TrafficUtilities.GetIntPosFromVector2IntPos(road.GridPosition, cols));
            }
        }

        CurrentLevelData = new LevelData() { Name = InputLevelName, 
            Rows = TileManager.GridManager.Rows,  
            Columns = cols,
            Roads = roads,
            Crosswalks = crosswalks.ToArray(),
            TrafficLights = trafficLights.ToArray(),
        };

        LevelSaveSystem.SaveLevelToJson(CurrentLevelData);
    }

    private static TrafficLightData SetTrafficLightData(int cols, TileRoad road, HashSet<TileRoad> roadsWithTLAdded) {
        roadsWithTLAdded.Add(road);
        TrafficLight tlref = road.GetComponentInChildren<TrafficLight>();
        int[] syncedLights = new int[tlref.SyncedLights.Count];
        for (int j = 0; j < syncedLights.Length; j++) {
            syncedLights[j] = TrafficUtilities.GetIntPosFromVector2IntPos(
                tlref.SyncedLights.ElementAt(j).Tile.GridPosition, cols);
            roadsWithTLAdded.Add(tlref.SyncedLights.ElementAt(j).Tile);
        }

        int[] unsyncedLights = new int[tlref.UnsyncedLights.Count];
        for (int j = 0; j < unsyncedLights.Length; j++) {
            unsyncedLights[j] = TrafficUtilities.GetIntPosFromVector2IntPos(
                tlref.UnsyncedLights.ElementAt(j).Tile.GridPosition, cols);
            roadsWithTLAdded.Add(tlref.UnsyncedLights.ElementAt(j).Tile);
        }

        TrafficLightData tl = new TrafficLightData() {
            Position = TrafficUtilities.GetIntPosFromVector2IntPos(road.GridPosition, cols),
            SyncedLightsPos = syncedLights,
            ReverseSyncedLightPos = unsyncedLights,
            GreenDuration = tlref.GreenDuration,
            YellowDuration = tlref.YellowDuration,
            RedDuration = tlref.RedDuration
        };
        return tl;
    }

    public void LoadLevel(int index) {
        GameService.Instance.Refresh();
        TileManager = GameService.Instance.GetTileManager();
        CurrentLevelData = LevelSaveSystem.SetCurrentLevel(LevelNames[index]);
        InputLevelName = CurrentLevelData.Name;
        TileManager.SetTilesFromLevelData(CurrentLevelData);
    }

    public void SetLevelName(string name) {
        InputLevelName = name;
    }

    public Action<Dictionary<string, LevelData>> GetAllLevels() {
        LevelSaveSystem.GetAllLevels(OnLevelChanged);
        return OnLevelChanged;
    }

    public void SetLevels(Dictionary<string, LevelData> levelData) {
        LevelNames = levelData.Keys.ToArray();
    }
   
}

