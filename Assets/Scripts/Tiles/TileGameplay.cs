using System.Collections.Generic;
using Traffic;
using UnityEngine;

public class TileGameplay : Tile
{
    [Header("Gameplay")]
        
    [SerializeField]
    private SpriteRenderer m_ImgFeature = null;
    [SerializeField]
    private float m_Offset = 0;

    public float Offset { get => m_Offset; }
    public SpriteRenderer ImgFeature { get => m_ImgFeature; }
    public List<Occupant> Occupants { get; private set; } = new();
    public Entity Entity { get; set; } = null;
    public SO_TileGameplay GpData { get; private set; } = null;

    public override void Initialize(SO_Tile data, Vector2Int gridPos, bool cursor = false) {
        base.Initialize(data, gridPos, cursor);
        if (Data is SO_TileGameplay gpdata) {
            GpData = gpdata;
            bool sidewalk = gpdata.Traversable != SO_TileGameplay.eTraversable.Untraversable;
            ImgFeature.gameObject.SetActive(sidewalk);
        }
    }

    public void AddOccupant(Occupant occupant) {
        Occupants.Add(occupant);
    }
}