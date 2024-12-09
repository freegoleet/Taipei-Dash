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
    public string[] LevelNames { get; private set; } = null;
    private TileManager TileManager { get; set; } = null;
    public Action<Dictionary<string, LevelData>> OnLevelChanged = null;

    public void Initialize() {
        OnLevelChanged += SetLevels;
    }
    
    public void SaveLevel() {
        TileManager = GameService.Instance.GridManager.TileManager;
        TileRoadData[] roads = new TileRoadData[TileManager.RoadTileManager.TileCount];
        for (int i = 0; i < roads.Length; i++) {
            TileRoad road = TileManager.RoadTileManager.Tiles[i];
            roads[i] = new TileRoadData() { 
                Position = road.GridPosition, 
                Connections = road.Connections,
                Facing = road.Facing,
            };
        }

        CurrentLevelData = new LevelData() { Name = InputLevelName, Roads = roads };
        LevelSaveSystem.SaveLevelToJson(CurrentLevelData);
    }

    public void LoadLevel(int index) {
        CurrentLevelData = LevelSaveSystem.LoadLevelFromJson(LevelNames[index]);
        InputLevelName = CurrentLevelData.Name;
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

