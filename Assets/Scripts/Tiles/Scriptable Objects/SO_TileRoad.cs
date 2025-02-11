using Traffic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileRoad", menuName = "ScriptableObjects/Tiles/TileRoad", order = 1)]
public class SO_TileRoad : SO_TileGameplay
{
    [Header("Road")]
    [SerializeField]
    private Direction[] m_DrivableDirections = null;
    public Direction[] DrivableDirections { get => m_DrivableDirections; }

    [SerializeField]
    private bool m_HasCrosswalk = false;
    public bool HasCrosswalk { get { return m_HasCrosswalk; } }

    public override TileType GetTileType() {
        return TileType.Road;
    }
}
