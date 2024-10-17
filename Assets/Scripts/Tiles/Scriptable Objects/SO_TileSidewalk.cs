using Traffic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileSidewalk", menuName = "ScriptableObjects/Tiles/TileSidewalk", order = 1)]
public class SO_TileSidewalk : SO_TileAutoFit
{
    [Header("Curbs")]
    [SerializeField]
    private Sprite m_CurbSide = null;
    [SerializeField]
    private Sprite m_CurbCorner = null;
    [SerializeField]
    private Sprite m_CurbMiddleCorner = null;
    [SerializeField]
    private Sprite m_CurbDeadEnd = null;

    public Sprite CurbSide { get => m_CurbSide; }
    public Sprite CurbCorner { get => m_CurbCorner; }
    public Sprite CurbInnerCorner { get => m_CurbMiddleCorner; }
    public Sprite CurbDeadEnd {  get => m_CurbDeadEnd; }

    public override TileType GetTileType() {
        return TileType.Autofit;
    }
}
