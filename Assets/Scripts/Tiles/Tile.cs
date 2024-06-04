using System.Collections.Generic;
using TMPro;
using Traffic;
using UnityEngine;

public class Tile : NodeBase
{
    [SerializeField]
    private SpriteRenderer m_ImgTile = null;
    [SerializeField]
    private SpriteRenderer m_ImgHighlight = null;
    [SerializeField]
    private TextMeshPro m_TxtCoords = null;
    [SerializeField]
    private TextMeshPro m_TxtValue = null;
    [Header("Road Tile")]
    [SerializeField]
    private SpriteRenderer m_Pointer = null;

    // Data
    private Entity m_Entity = null;
    public Entity Entity { get { return m_Entity; } set { m_Entity = value; } }
    
    public Directions[] DrivableDirections { get; set; } = null;
    public Directions TileRotation { get; private set; } = Directions.Up;
    public List<Occupant> Occupants { get; set; } = new();
    public SpriteRenderer ImgPointer { get => m_Pointer; }

    // Pathfinding
    [Header("AStar Values")]
    [SerializeField]
    private GameObject m_AStarValues = null;
    [SerializeField]
    private TextMeshProUGUI m_GCost = null;
    [SerializeField]
    private TextMeshProUGUI m_HCost = null;
    [SerializeField]
    private TextMeshProUGUI m_FCost = null;

    private SO_Tile m_Data = null;
    public SO_Tile Data { get { return m_Data; } }

    public SpriteRenderer ImgTile { get => m_ImgTile; }

    public override void Initialize<T>(T data, int col = -1, int row = -1) {
        m_Data = data as SO_Tile;

        ImgTile.sprite = m_Data.Sprite;
        m_ImgHighlight.sprite = m_Data.Sprite;


        if (col >= 0 && row >= 0) {
            m_Col = col;
            m_Row = row;
            gameObject.name = "x: " + col + " y: " + row;
        }

        m_TxtCoords.text = gameObject.name;
        ImgPointer.gameObject.SetActive(m_Data.Rotatable);
    }

    public void SetDirections(Directions[] directions) {
        DrivableDirections = directions;
    }

    public void AddOccupant(Occupant occupant) {
        Occupants.Add(occupant);
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

    public void TogglePointer(bool show) {
        ImgPointer.gameObject.SetActive(show);
    }

    public void ToggleHighlight(bool show, Color? color = null) {
        m_ImgHighlight.gameObject.SetActive(show);
        if (color != null) {
            m_ImgHighlight.color = (Color)color;
        }
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

    public void SetAStarValues(float g, float h, float f) {
        m_FCost.text = f.ToString();
    }

    public void ToggleAStarValues(bool show) {
        m_AStarValues.gameObject.SetActive(show);
    }

    public override void SetG(float g) {
        base.SetG(g);
        m_GCost.text = g.ToString();
        m_FCost.text = (G + H).ToString();
    }

    public override void SetH(float h) {
        base.SetH(h);
        m_HCost.text = h.ToString();
        m_FCost.text = (G + H).ToString();
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
