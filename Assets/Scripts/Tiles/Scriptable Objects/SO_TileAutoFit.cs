using UnityEngine;

public enum AutofitType
{
    Middle,
    Side,
    Corner,
    Bridge,
    DeadEnd
}

[CreateAssetMenu(fileName = "TileAutoFit", menuName = "ScriptableObjects/Tiles/TileAutoFit", order = 1)]
public class SO_TileAutoFit : SO_TileGameplay
{
    [Header("TileFeatures")]
    [SerializeField]
    private Sprite m_TileSide = null;
    [SerializeField]
    private Sprite m_TileCorner = null;
    [SerializeField]
    private Sprite m_TileMiddle = null;
    [SerializeField]
    private Sprite m_TileMiddleCorner = null;

    public Sprite TileSide { get => m_TileSide; }
    public Sprite TileCorner { get => m_TileCorner; }
    public Sprite TileMiddle { get => m_TileMiddle; }
    public Sprite TileMiddleCorner { get => m_TileMiddleCorner; }
}
