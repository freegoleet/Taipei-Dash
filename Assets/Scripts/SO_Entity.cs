using UnityEngine;
using static PathfindingUtilities;

[CreateAssetMenu(fileName = "Entity", menuName = "ScriptableObjects/Entities/Entity", order = 1)]
public class SO_Entity : ScriptableObject
{
    [SerializeField]
    private PathfindType m_PathfindType = PathfindType.Pedestrian;

    public PathfindType PathfindType { get => m_PathfindType; }
}
