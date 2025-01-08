using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace Traffic
{
    [Serializable]
    public struct TileRoadData
    {
        public Direction Facing { get; set; }
        public int Position { get; set; }
        public Connections Connections { get; set; }
        public Dictionary<Direction, LineType> ManualLines { get; set; }
    }

    [Serializable]
    public struct TrafficLightData
    {
        public int[] SyncedLightsPos { get; set; }
        public int[] ReverseSyncedLightPos { get; set; }
        public float GreenDuration { get; set; }
        public float YellowDuration { get; set; }
        public float RedDuration { get; set; }
        public int Position { get; set; }
    }

    [Serializable]
    public struct LevelData
    {
        public string Name { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public TileRoadData[] Roads { get; set; }
        public int[] Crosswalks { get; set; }
        public TrafficLightData[] TrafficLights { get; set; }
    }

    public class SaveSystem
    {
        public Dictionary<string, LevelData> AllLevels = new();
        public LevelData CurrentLevel { get; private set; } = default;

        public Action<LevelData> CurrentLevelChanged = null;

        private string m_SaveFilePath = null;

        /// <summary>
        /// Get all locally available levels from the save file.
        /// </summary>
        /// <param name="onFinished"> An action that will be invoked when all levels have been loaded. </param>
        public async void GetAllLevels(Action<Dictionary<string, LevelData>> onFinished) {
            DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
            FileInfo[] fileInfo = info.GetFiles();
            AllLevels.Clear();

            for (int i = 0; i < fileInfo.Length; i++) {
                using StreamReader reader = File.OpenText(Path.Combine(info.FullName, fileInfo[i].Name));
                string text = await reader.ReadToEndAsync();
                AddLevelFromJson(text);
            }
            onFinished?.Invoke(AllLevels);
        }

        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Save the provided level locally.
        /// </summary>
        /// <param name="levelData"> The LevelData of the level to save. </param>
        /// <param name="onFinished"> An action that will be invoked when the save has completed. </param>
        public async void SaveLevelToJson(LevelData levelData, Action onFinished = null) {
            m_SaveFilePath = Application.persistentDataPath + "/" + levelData.Name + ".json";

            using StreamWriter outputFile = new StreamWriter(m_SaveFilePath);
            await outputFile.WriteAsync(JsonConvert.SerializeObject(levelData));
        }

        /// <summary>
        /// Get a specific level from the save file.
        /// </summary>
        /// <param name="jsonData"> The name of the level to get. </param>
        /// <returns></returns>
        public LevelData AddLevelFromJson(string jsonData) {
            LevelData level = JsonConvert.DeserializeObject<LevelData>(jsonData);
            if (AllLevels.TryAdd(level.Name, level) == false) {
                AllLevels[level.Name] = level;
            }
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

        public LevelData SetCurrentLevel(string levelName) {
            CurrentLevel = AllLevels[levelName];

            string s = string.Empty;
            BindingFlags bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static;

            foreach (FieldInfo field in CurrentLevel.GetType().GetFields(bindingFlags)) {
                s += field.Name + ":" + field.GetValue(CurrentLevel) + ". ";
            }

            //Debug.Log(s);

            CurrentLevelChanged?.Invoke(CurrentLevel);
            return CurrentLevel;
        }
    }
}