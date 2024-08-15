using TMPro;
using Traffic;
using UnityEngine;

public class Tile : NodeBase
{
    [Header("Tile")]
    [SerializeField]
    private SpriteRenderer m_ImgTile = null;
    [SerializeField]
    private SpriteRenderer m_ImgHighlight = null;
    [SerializeField]
    private TextMeshPro m_TxtCoords = null;
    [SerializeField]
    private TextMeshPro m_TxtValue = null;

    public TileType TileType { get; private set; } = TileType.None;

    // Data
    private Entity m_Entity = null;
    public Entity Entity { get { return m_Entity; } set { m_Entity = value; } }
    
    public Directions[] DrivableDirections { get; set; } = null;
    public Directions TileRotation { get; private set; } = Directions.Up;

    private SO_Tile m_Data = null;
    public SO_Tile Data { get { return m_Data; } protected set { m_Data = value; } }

    public SpriteRenderer ImgTile { get => m_ImgTile; }

    public override void Initialize<T>(T data, int col = -1, int row = -1) {
        Data = data as SO_Tile;
        ImgTile.sprite = Data.Sprite;
        
        if (col >= 0 && row >= 0) {
            m_Col = col;
            m_Row = row;
            gameObject.name = "x: " + col + " y: " + row;
        }

        m_TxtCoords.text = gameObject.name;


    }

    public void SetDirections(Directions[] directions) {
        DrivableDirections = directions;
    }

    public void SetDirection(Directions direction) {
        switch (direction) {
            case Directions.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Directions.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case Directions.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case Directions.Left:
                transform.rotation = Quaternion.Euler(0, 0, -270);
                break;
        }

        TileRotation = direction;
    }

    public void ToggleHighlight(bool show, Color? color = null) {
        m_ImgHighlight.gameObject.SetActive(show);
        if (color != null) {
            m_ImgHighlight.color = (Color)color;
        }
    }

    public void SetData(SO_Tile data) {
        Data = data;
    }

    public void SetValue(int value) {
        m_TxtValue.text = value.ToString();
    }

    public void ToggleValue(bool show) {
        m_TxtValue.gameObject.SetActive(show);
    }

    public void ToggleCoords(bool show) {
        m_TxtCoords.gameObject.SetActive(show);
    }
}

public class TileGUI
{
    public Texture2D Texture2D { get; set; }
}

public class TileRoadGUI : TileGUI
{
    public Directions[] Directions { get; set; } = null;
}
