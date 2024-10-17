using TMPro;
using Traffic;
using UnityEngine;


public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer m_SpriteRenderer = null;
    [SerializeField]
    protected TileGameplay m_Tile = null;
    [SerializeField]
    protected TextMeshPro m_TxtEntityId = null;

    protected int m_Hitpoints = 0;
    public ulong Id { get; set; } = ulong.MaxValue;
    public ulong OwnerId { get; set; } = ulong.MaxValue;

    public int Hitpoints { get { return m_Hitpoints; } }
    public TileGameplay TileGameplay { get { return m_Tile; } set { m_Tile = value; } }

    public TextMeshPro TxtEntityId { get { return m_TxtEntityId; } }

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