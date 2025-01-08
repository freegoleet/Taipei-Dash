using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car Data", menuName = "ScriptableObjects/Data/Car Data", order = 1)]
public class SO_CarData : ScriptableObject
{
    [SerializeField]
    private CarEntity m_CarPrefab = null;
    [SerializeField]
    private List<Color> m_AvailableColors = new List<Color>();
    [SerializeField]
    private float m_SpeedRange = 0.2f;
    [SerializeField]
    private float m_LawbreakingCoef = 0.1f;
    [SerializeField]
    private float m_AccelerationCoef = 0.5f;
    [SerializeField]
    private List<SO_CarType> m_CarBodies = new List<SO_CarType>();

    public CarEntity CarPrefab { get => m_CarPrefab; }
    public List<Color> AvailableColors { get => m_AvailableColors; }
    public float SpeedCoef { get => m_SpeedRange; }
    public float LawbreakingCoef { get => m_LawbreakingCoef; }
    public float AccelerationCoef { get => m_AccelerationCoef; }
    public List<SO_CarType> CarBodies { get => m_CarBodies; }
}