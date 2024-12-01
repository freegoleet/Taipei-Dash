using TMPro;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer m_SpriteRenderer = null;
    [SerializeField]
    protected TileGameplay m_Tile = null;
    [SerializeField]
    protected TextMeshPro m_TxtEntityId = null;

    public ulong Id { get; set; } = ulong.MaxValue;

    public int Hitpoints { get { return m_Hitpoints; } }
    public TileGameplay TileGameplay { get { return m_Tile; } set { m_Tile = value; } }
    public TextMeshPro TxtEntityId { get { return m_TxtEntityId; } }
    public Pathfinding Pathfinding { get; private set; } = null;

    public SO_Entity EntityData { get; private set; } = null;
    protected int m_Hitpoints = 0;

    public void Initialize(SO_Entity data) {
        EntityData = data;
        Pathfinding = new(this);
    }

    public void Tick(float dt) {

    }

    public void SetTile(TileGameplay tile)
    {
        if(tile == null)
        {
            TileGameplay = null;
            return;
        }

        TileGameplay = tile; 
        TileGameplay.Entity = this;
    }
}