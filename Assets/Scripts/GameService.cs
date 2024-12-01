using System.Collections.Generic;
using Traffic;
using UnityEngine;

[ExecuteInEditMode]
public class GameService : MonoBehaviour
{
    [SerializeField]
    private bool m_Tick = false;

    // Singleton
    private static GameService m_Instance = null;
    public static GameService Instance { get { return m_Instance; } }

    // Managers
    [SerializeField]
    private GridManager m_GridManager = null;
    public GridManager GridManager { get { return m_GridManager; } }

    // Tilemap Editor
    [SerializeField]
    private TilemapEditor m_TilemapEditor = null;
    public TilemapEditor TilemapEditor { get { return m_TilemapEditor; } }

    // Traffic
    [SerializeField]
    private SO_TrafficSettings m_TrafficSettings = null;
    public SO_TrafficSettings TrafficSettings { get => m_TrafficSettings; }

    private HashSet<TrafficLight> TrafficLights { get; set; } = new();

    private bool Initialized = false;

    public void Initialize() {
        if (m_Instance != null && m_Instance != this) {
            DestroyImmediate(this);
            return;
        }

        m_Instance = this;
    }

    private void Awake() {
        DontDestroyOnLoad(this);

        Initialize();
    }

    private void OnEnable() {
        Initialize();
    }

    private void Update() {
        if(Instance == null) {
            Initialize();
        }

        if (m_Tick == false) {
            return;
        }

        foreach (TrafficLight light in TrafficLights) {
            light.Tick(Time.deltaTime);
        }

        EntityManager.Tick(Time.deltaTime);
    }

    public void AddTrafficLight(TrafficLight trafficLight) {
        TrafficLights.Add(trafficLight);
    }

    public void RemoveTrafficLight(TrafficLight trafficLight) {
        TrafficLights.Remove(trafficLight);
    }
}
