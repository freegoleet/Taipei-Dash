using System;
using Traffic;
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

    [Flags]
    public enum eFittableWith
    {
        Nothing,
        Sidewalk,
        Road
    }

    [Header("Gameplay")]
    [SerializeField]
    private eTraversable m_Traversable = eTraversable.Walking;
    public eTraversable Traversable { get { return m_Traversable; } }
    [SerializeField]
    private eFittableWith m_FittableWith = eFittableWith.Nothing;
    public eFittableWith FitsWith { get { return m_FittableWith; } }

    public override TileType GetTileType() {
        return TileType.Gameplay;
    }
}