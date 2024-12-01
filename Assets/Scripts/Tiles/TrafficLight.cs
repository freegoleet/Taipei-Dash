//using System.Collections.Generic;
//using UnityEngine;

//namespace Traffic
//{
//    public class TrafficLight : TileRoad
//    {
//        [SerializeField]
//        private SO_TrafficLightData m_Data = null;

//        public TrafficLightColor CurrentColor { get; private set; } = TrafficLightColor.Red;

//        private Dictionary<TrafficLightColor, float> m_ColorsAndTimes = null;
//        private TrafficLightColor m_NextColor = TrafficLightColor.Red;
//        private float m_Timer = 0f;

//        private void Start() {
//            m_ColorsAndTimes = m_Data.GetColorsAndTimes();
//        }

//        private void Update() {
//            m_Timer += Time.deltaTime;

//            if (m_Timer >= m_ColorsAndTimes[CurrentColor]) {
//                m_Timer = 0f;

//                if (CurrentColor == TrafficLightColor.Yellow) {
//                    CurrentColor = m_NextColor;
//                    return;
//                }

//                m_NextColor = GetNextColor(CurrentColor);
//                CurrentColor = TrafficLightColor.Yellow;
//            }
//        }

//        private TrafficLightColor GetNextColor(TrafficLightColor currentColor) {
//            return currentColor == TrafficLightColor.Red ? TrafficLightColor.Green : TrafficLightColor.Red;
//        }
//    }
//}