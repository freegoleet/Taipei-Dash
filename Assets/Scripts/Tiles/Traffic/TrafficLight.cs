using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    public class TrafficLight : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private GameObject m_GreenLight = null;
        [SerializeField]
        private GameObject m_YellowLight = null;
        [SerializeField]
        private GameObject m_RedLight = null;

        [Header("Durations")]
        [SerializeField]
        private float m_GreenDuration = 10f;
        [SerializeField]
        private float m_YellowDuration = 1f;
        [SerializeField]
        private float m_RedDuration = 10f;

        public TileRoad Road { get; private set; } = null;
        private List<TileRoad> RoadsAffected { get; set; } = new();
        public HashSet<TrafficLight> SyncedLights { get; private set; } = new();
        public HashSet<TrafficLight> SyncedReverseLights { get; private set; } = new();
        public float GreenDuration { get => m_GreenDuration; set => m_GreenDuration = value; }
        public float YellowDuration { get => m_YellowDuration; set => m_YellowDuration = value; }
        public float RedDuration { get => m_RedDuration; set => m_RedDuration = value; }

        private Dictionary<TrafficLightColor, float> m_ColorsAndTimes = null;
        private TrafficLightColor CurrentColor { get; set; } = TrafficLightColor.Green;
        private float m_Timer = 0f;

        public void OnValidate() {
            m_ColorsAndTimes = new() {
                { TrafficLightColor.Green, GreenDuration },
                { TrafficLightColor.Yellow, YellowDuration },
                { TrafficLightColor.Red, RedDuration },
            };
        }

        public void OnEnable() {
            m_ColorsAndTimes = new() {
                { TrafficLightColor.Green, GreenDuration },
                { TrafficLightColor.Yellow, YellowDuration },
                { TrafficLightColor.Red, RedDuration },
            };
        }

        public void Initialize(TileRoad tileRoad) {
            m_ColorsAndTimes = new() {
                { TrafficLightColor.Green, GreenDuration },
                { TrafficLightColor.Yellow, YellowDuration },
                { TrafficLightColor.Red, RedDuration },
            };

            Road = tileRoad;
            SetFacing(Road.Facing);
            GameService.Instance.AddTrafficLight(this);
        }

        public void Tick(float dt) {
            m_Timer += dt;
            Debug.Log("tick");
            if (m_Timer >= m_ColorsAndTimes[CurrentColor]) {
                m_Timer = 0f;
                NextColor();
            }
        }

        /// <summary>
        /// Add a road tile to be affected by this traffic light.
        /// </summary>
        public void AddRoad(TileRoad tileRoad) {
            RoadsAffected.Add(tileRoad);
        }

        /// <summary>
        /// Remove a road tile from being affected by this traffic light.
        /// </summary>
        public void RemoveRoad(TileRoad tileRoad) {
            RoadsAffected.Remove(tileRoad);
        }

        private void NextColor() {
            switch (CurrentColor) {
                case TrafficLightColor.Red:
                    m_RedLight.SetActive(false);
                    break;
                case TrafficLightColor.Yellow:
                    m_YellowLight.SetActive(false);
                    break;
                case TrafficLightColor.Green:
                    m_GreenLight.SetActive(false);
                    break;
            }

            CurrentColor = GetNextColor(CurrentColor);

            switch (CurrentColor) {
                case TrafficLightColor.Red:
                    m_RedLight.SetActive(true);
                    SetRoadsDrivability(false);
                    break;
                case TrafficLightColor.Yellow:
                    m_YellowLight.SetActive(true);
                    break;
                case TrafficLightColor.Green:
                    m_GreenLight.SetActive(true);
                    SetRoadsDrivability(true);
                    break;
            }
        }

        public void SetFacing(Direction direction) {
            switch (direction) {
                case Direction.Up:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Right:
                    transform.rotation = Quaternion.Euler(0, 0, 270);
                    break;
                case Direction.Down:
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case Direction.Left:
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
            }
        }

        public void SyncNew(TrafficLight trafficLight) {
            SyncedLights.Add(trafficLight);
            trafficLight.SyncedLights.Add(this);
        }

        public void SyncNewReverse(TrafficLight trafficLight) {
            SyncedReverseLights.Add(trafficLight);
            trafficLight.SyncedReverseLights.Add(this);
        }

        public void UnSyncTrafficLight() {
            foreach (TrafficLight light in SyncedLights) {
                light.SyncedLights.Remove(this);
                SyncedLights.Remove(light);
            }

            foreach (TrafficLight light in SyncedReverseLights) {
                light.SyncedReverseLights.Remove(this);
                SyncedReverseLights.Remove(light);
            }
        }

        public void SyncTimers() {
            foreach (var light in SyncedLights) {
                light.GreenDuration = GreenDuration;
                light.YellowDuration = YellowDuration;
                light.RedDuration = RedDuration;
                light.SetLight(CurrentColor, m_Timer);
            }
            foreach (var light in SyncedReverseLights) {
                light.GreenDuration = RedDuration;
                light.YellowDuration = YellowDuration;
                light.RedDuration = GreenDuration;
                switch (CurrentColor) {
                    case TrafficLightColor.Red:
                        light.SetLight(TrafficLightColor.Green, m_Timer);
                        break;
                    case TrafficLightColor.Yellow:
                        light.SetLight(TrafficLightColor.Red, 0 + m_Timer);
                        break;
                    case TrafficLightColor.Green:
                        light.SetLight(TrafficLightColor.Red, m_Timer);
                        break;
                }
            }
        }

        public void SetLight(TrafficLightColor color, float timeProgressed) {
            CurrentColor = color;
            if (timeProgressed > m_ColorsAndTimes[color]) {
                float newTime = timeProgressed - m_ColorsAndTimes[color];
                SetLight(GetNextColor(CurrentColor), newTime);
                return;
            }

            m_Timer = timeProgressed;
        }

        private TrafficLightColor GetNextColor(TrafficLightColor currentColor) {
            switch (currentColor) {
                case TrafficLightColor.Red:
                    return TrafficLightColor.Green;
                case TrafficLightColor.Yellow:
                    return TrafficLightColor.Red;
                case TrafficLightColor.Green:
                    return TrafficLightColor.Yellow;
            }
            return TrafficLightColor.Green;
        }

        private void SetRoadsDrivability(bool drivable) {
            foreach (TileRoad road in RoadsAffected) {
                road.GreenLit = drivable;
            }
        }

        private void OnDestroy() {
            UnSyncTrafficLight();
        }
    }

}

