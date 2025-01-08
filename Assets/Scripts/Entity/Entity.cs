using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Base Entity")]
    [SerializeField]
    protected TileGameplay m_Tile = null;
    [SerializeField]
    protected TextMeshPro m_TxtEntityId = null;
    [SerializeField]
    private Transform m_PivotPoint = null;

    public ulong Id { get; set; } = ulong.MaxValue;

    public int Hitpoints { get { return m_Hitpoints; } }
    public TileGameplay Tile { get { return m_Tile; } set { m_Tile = value; } }
    public TextMeshPro TxtEntityId { get { return m_TxtEntityId; } }
    public EntityPathfinding Pathfinding { get; private set; } = null;
    public EntityControllerNPC EntityControllerNPC { get; private set; } = null;
    public List<TileGameplay> OccupiedTiles { get; private set; } = new();
    public Vector3 OffsetPos { get; set; } = Vector3.zero;
    public Transform PivotPoint { get => m_PivotPoint; set => m_PivotPoint = value; }

    protected int m_Hitpoints = 0;


    public virtual void Initialize() {
        Pathfinding = new EntityPathfinding(this);
        EntityControllerNPC = new EntityControllerNPC(this);
        transform.position = PivotPoint.position;
    }

    public virtual void Tick(float dt) {
        EntityControllerNPC.Tick(dt);
    }

    public void SetTile(TileGameplay tile) {
        if (tile == null) {
            Tile = null;
            return;
        }

        Tile = tile;
        Tile.Entity = this;
    }

    public void OccupyTile(TileGameplay tileGameplay) {
        tileGameplay.Entity = this;
        Tile = tileGameplay;
        OccupiedTiles.Add(tileGameplay);
    }

    public void LeaveTile(TileGameplay tileGameplay) {
        tileGameplay.Entity = null;
        Tile = tileGameplay;
        OccupiedTiles.Remove(tileGameplay);
    }

    public void IncrementPosition(Vector3 pos) {
        transform.position += pos;
    }

    public Vector2 GetOffset() {
        return PivotPoint.position - transform.position;
    }
     
    public void OnDrawGizmos() {
        EntityControllerNPC.DrawPath();
    }
}