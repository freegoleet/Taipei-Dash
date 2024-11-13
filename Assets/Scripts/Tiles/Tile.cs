using TMPro;
using Traffic;
using UnityEngine;

public class Tile : NodeBase
{
    [Header("Tile")]
    [SerializeField]
    private SpriteRenderer m_ImgBackground = null;
    [SerializeField]
    private SpriteRenderer m_ImgHighlight = null;
    [SerializeField]
    private TextMeshPro m_TxtCoords = null;
    [SerializeField]
    private TextMeshPro m_TxtValue = null;

    public TileType TileType { get; private set; } = TileType.None;
    public SO_Tile Data { get; protected set; } = null;
    public Direction Facing { get; private set; } = Direction.Up;

    public SpriteRenderer ImgBackground { get => m_ImgBackground; }
    public SpriteRenderer ImgHighlight { get => m_ImgHighlight; }
    public TextMeshPro TxtCoords { get => m_TxtCoords; }
    public TextMeshPro TxtValue { get => m_TxtValue; }
    public NeighborSystem NeighborSystem { get; set; } = null;

    public virtual void Initialize(SO_Tile data, Vector2Int gridPos, bool cursor = false) {
        if (cursor == false) {
            m_Col = gridPos.x;
            m_Row = gridPos.y;
            gameObject.name = "x: " + Col + " y: " + Row;
        }

        Data = data;
        TileType = data.GetTileType();
        ImgBackground.sprite = data.Background;
        NeighborSystem = new NeighborSystem(this);
    }

    public virtual void SetFacing(Direction facing) {
        Facing = facing;
    }

    public void ToggleHighlight(bool show, Color? color = null) {
        ImgHighlight.gameObject.SetActive(show);
        if (color != null) {
            ImgHighlight.color = (Color)color;
        }
    }

    public void SetData(SO_Tile data) {
        Data = data;
    }

    public void SetValue(int value) {
        TxtValue.text = value.ToString();
    }

    public void ToggleValue(bool show) {
        TxtValue.gameObject.SetActive(show);
    }

    public void ToggleCoords(bool show) {
        TxtCoords.gameObject.SetActive(show);
    }

    public virtual void FitTile() {

    }
}

public class TileGUI
{
    public Texture2D Texture2D { get; set; }
    public Direction[] Directions { get; set; } = null;
}
