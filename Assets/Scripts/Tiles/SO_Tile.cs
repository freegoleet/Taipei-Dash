using System;
using Traffic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "ScriptableObjects/Tiles/Tile", order = 1)]
public class SO_Tile : ScriptableObject
{
    [Flags]
    public enum eTraversable
    {
        Untraversable,
        Walking,
    }

    [SerializeField]
    private Sprite m_Sprite = null;
    public Sprite Sprite { get { return m_Sprite; } }

    [SerializeField]
    private eTraversable m_Traversable = eTraversable.Walking;
    public eTraversable Traversable { get { return m_Traversable; } }

    [SerializeField]
    private int m_Index = -1;
    public int Index { get { return m_Index; } }

    [SerializeField]
    private Directions[] m_DrivableDirections = null;
    public Directions[] DrivableDirections { get => m_DrivableDirections; }

    [SerializeField]
    private bool m_Rotatable = false;
    public bool Rotatable { get { return m_Rotatable; } }

    [SerializeField]
    private bool m_RoadTile = false;
    public bool RoadTile { get { return m_RoadTile; } }

}
