using Traffic;
using UnityEngine;

public abstract class SO_Tile : ScriptableObject
{
    [Header("Tile Base")]
    [SerializeField]
    private Sprite m_Sprite = null;
    public Sprite Sprite { get { return m_Sprite; } }

    [SerializeField]
    private int m_Index = -1;
    public int Index { get { return m_Index; } }

    [SerializeField]
    private bool m_Rotatable = false;
    public bool Rotatable { get { return m_Rotatable; } }

    public virtual TileType GetTileType() {
        return TileType.None;
    }
}
