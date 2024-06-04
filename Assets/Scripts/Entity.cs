using TMPro;
using UnityEngine;


public abstract class Entity : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer m_SpriteRenderer = null;
    [SerializeField]
    protected Tile m_Tile = null;
    [SerializeField]
    protected TextMeshPro m_TxtEntityId = null;

    protected int m_Hitpoints = 0;
    public ulong Id { get; set; } = ulong.MaxValue;
    public ulong OwnerId { get; set; } = ulong.MaxValue;

    public int Hitpoints { get { return m_Hitpoints; } }
    public Tile Tile { get { return m_Tile; } set { m_Tile = value; } }

    public TextMeshPro TxtEntityId { get { return m_TxtEntityId; } }

    public void Tick(float dt) {

    }

    public void SetTile(Tile tile)
    {
        if(tile == null)
        {
            Tile = null;
            return;
        }

        Tile = tile; 
        Tile.Entity = this;
    }
}