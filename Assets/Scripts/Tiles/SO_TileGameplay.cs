using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TileGameplay", menuName = "ScriptableObjects/Tiles/TileGameplay", order = 1)]
public class SO_TileGameplay : SO_Tile
{
    [Flags]
    public enum eTraversable
    {
        Untraversable,
        Walking,
    }

    [Header("Gameplay")]
    [SerializeField]
    private eTraversable m_Traversable = eTraversable.Walking;
    private bool m_AutoFit = false;

    public eTraversable Traversable { get { return m_Traversable; } }
    public bool AutoFit { get => m_AutoFit; set => m_AutoFit = value; }
}
