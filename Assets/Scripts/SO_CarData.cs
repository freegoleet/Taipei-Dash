using UnityEngine;

[CreateAssetMenu(fileName = "Car Data", menuName = "ScriptableObjects/Traffic/Car Data", order = 1)]
public class SO_CarData : ScriptableObject
{
    [SerializeField]
    private float m_PreferredSpeed = 1f;
    [SerializeField]
    private float m_StopAtRedChance = 0.5f;
    [SerializeField]
    private float m_StopForPedestrianChance = 0.5f;

    public float PreferredSpeed { get => m_PreferredSpeed; }
    public float StopAtRedChance { get => m_StopAtRedChance; }
    public float StopForPedestrianChance { get => m_StopForPedestrianChance; }
}
