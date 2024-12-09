using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Traffic
{
    public struct TileRoadData
    {
        public Direction Facing {  get; set; }
        public Vector2Int Position {  get; set; }
        public Connections Connections { get; set; }
    }

    public struct TrafficLightData {
        public TrafficLightData[] SyncedLights { get; set; }
        public TrafficLightData[] ReverseSyncedLight { get; set; }
        public int GreenDuration { get; set; }
        public int YellowDuration { get; set; }
        public int RedDuration { get; set; }
        public Vector2Int Position { get; set; }
    }

    public struct LevelData
    {
        public string Name { get; set; }
        public TileRoadData[] Roads { get; set; }
        public int[] Crosswalk {  get; set; }
        public TrafficLightData[] TrafficLight {  get; set; }
    }

    public class SaveSystem
    {
        public Dictionary<string, LevelData> AllLevels = new();
        public LevelData CurrentLevel { get; private set; } = default;
        public List<LevelData> LevelData { get; } = new();

        public Action<LevelData> CurrentCharacterChanged = null;

        private string m_SaveFilePath = null;

        /// <summary>
        /// Get all locally available levels from the save file.
        /// </summary>
        /// <param name="onFinished"> An action that will be invoked when all levels have been loaded. </param>
        public async void GetAllLevels(Action<Dictionary<string, LevelData>> onFinished) {
            var info = new DirectoryInfo(Application.persistentDataPath);
            var fileInfo = info.GetFiles();

            AllLevels.Clear();

            for (int i = 0; i < fileInfo.Length; i++) {
                var text = await fileInfo[i].OpenText().ReadToEndAsync();
                LevelData levelData = LoadLevelFromJson(text);
                AllLevels.Add(levelData.Name, levelData);
            }
            onFinished?.Invoke(AllLevels);
        }

        /// <summary>
        /// Save the provided level locally.
        /// </summary>
        /// <param name="levelData"> The LevelData of the level to save. </param>
        /// <param name="onFinished"> An action that will be invoked when the save has completed. </param>
        public async void SaveLevelToJson(LevelData levelData, Action onFinished = null) {
            m_SaveFilePath = Application.persistentDataPath + "/" + levelData.Name + ".json";
            await File.WriteAllTextAsync(m_SaveFilePath, JsonConvert.SerializeObject(levelData));
            onFinished?.Invoke();
        }

        /// <summary>
        /// Get a specific level from the save file.
        /// </summary>
        /// <param name="jsonData"> The name of the level to get. </param>
        /// <returns></returns>
        public LevelData LoadLevelFromJson(string jsonData) {
            LevelData level = JsonConvert.DeserializeObject<LevelData>(jsonData);
            LevelData.Add(level);
            return level;
        }

        /// <summary>
        /// Delete the local save file of a level.
        /// </summary>
        /// <param name="levelName"> The name of the level to delete. </param>
        public void DeleteLevelFromJson(string levelName) {
            File.Delete(Application.persistentDataPath + "/" + levelName + ".json");
            AllLevels.Remove(levelName);
            if (CurrentLevel.Name == levelName) {
                CurrentLevel = default;
            }
        }

        public LevelData SetCurrentLevel(string characterName) {
            CurrentLevel = AllLevels[characterName];

            string s = string.Empty;
            BindingFlags bindingFlags = 
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static;

            foreach (FieldInfo field in CurrentLevel.GetType().GetFields(bindingFlags)) {
                s += field.Name + ":" + field.GetValue(CurrentLevel) + ". ";
            }

            Debug.Log(s);

            CurrentCharacterChanged?.Invoke(CurrentLevel);
            return CurrentLevel;
        }
    }
}