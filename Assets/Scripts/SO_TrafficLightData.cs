using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    [CreateAssetMenu(fileName = "Traffic Light Data", menuName = "ScriptableObjects/Tiles/Traffic Light Data", order = 1)]
    public class SO_TrafficLightData : SO_Tile
    {
        [SerializeField]
        private float m_RedTime = 10f;
        [SerializeField]
        private float m_YellowTime = 1f;
        [SerializeField]
        private float m_GreenTime = 10f;

        public Dictionary<TrafficLightColor, float> GetColorsAndTimes() {
            return new() {
            { TrafficLightColor.Red, m_RedTime},
            { TrafficLightColor.Yellow, m_YellowTime},
            { TrafficLightColor.Green, m_GreenTime},
        };
        }
    }
}