using Traffic;
using UnityEngine;

public enum LineType
{
    LaneSeparator,
    DirectionSeparator,
    Stop
}

[CreateAssetMenu(fileName = "TileRoad", menuName = "ScriptableObjects/Tiles/TileRoad", order = 1)]
public class SO_TileRoad : SO_TileGameplay
{
    [Header("Road")]
    [SerializeField]
    private Directions[] m_DrivableDirections = null;
    public Directions[] DrivableDirections { get => m_DrivableDirections; }

    [SerializeField]
    private bool m_HasCrosswalk = false;
    public bool HasCrosswalk { get { return m_HasCrosswalk; } }
}
