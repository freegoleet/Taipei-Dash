using UnityEngine;

[CreateAssetMenu(fileName = "Entity", menuName = "ScriptableObjects/Entities/Entity", order = 1)]
public class SO_Entity : ScriptableObject
{
    public enum ePathfindType
    {
        Pedestrian,
        Car
    }

    [SerializeField]
    private float m_MoveSpeed = 1.0f;
    [SerializeField]
    private float m_LawBreakingFrequency = 1.0f;
    [SerializeField]
    private ePathfindType m_PathfindType = ePathfindType.Pedestrian;

    public float MoveSpeed { get => m_MoveSpeed; }
    public float LawBreakingFrequency { get => m_LawBreakingFrequency; }
    public ePathfindType PathfindType { get => m_PathfindType; }
}
