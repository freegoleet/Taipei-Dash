using System.Collections.Generic;
using TMPro;
using Traffic;
using UnityEngine;

public class TileRoad : TileGameplay
{
    [Header("Traffic Details")]
    [SerializeField]
    private SpriteRenderer m_ImgPointer = null;

    [Header("Lines")]
    [SerializeField]
    private SpriteRenderer m_ImgLineTop = null;
    [SerializeField]
    private SpriteRenderer m_ImgLineBot = null; 
    [SerializeField]
    private SpriteRenderer m_ImgLineLeft = null; 
    [SerializeField]
    private SpriteRenderer m_ImgLineRight = null;

    [Header("AStar Values")]
    [SerializeField]
    private GameObject m_AStarValues = null;
    [SerializeField]
    private TextMeshPro m_GCost = null;
    [SerializeField]
    private TextMeshPro m_HCost = null;
    [SerializeField]
    private TextMeshPro m_FCost = null;

    public SpriteRenderer Pointer { get => m_ImgPointer; }
    public SpriteRenderer ImgLineTop { get => m_ImgLineTop; }
    public SpriteRenderer ImgLineBot { get => m_ImgLineBot; }
    public SpriteRenderer ImgLineLeft { get => m_ImgLineLeft; }
    public SpriteRenderer ImgLineRight { get => m_ImgLineRight; }

    public Direction[] DrivableDirections { get; set; } = null;

    private Dictionary<Direction, Vector2Int> RoadBehind = new() {
        { Direction.Up, new Vector2Int(0, -1) },
        { Direction.Down, new Vector2Int(0, 1) },
        { Direction.Left, new Vector2Int(1, 0) },
        { Direction.Right, new Vector2Int(-1, 0) },
    };

    public override void Initialize(SO_Tile data, Vector2Int gridPos, bool cursor = false) {
        base.Initialize(data, gridPos, cursor);
    }

    public void TogglePointer(bool show) {
        Pointer.gameObject.SetActive(show);
    }

    public void ToggleTopLine(bool show) {
        ImgLineTop.gameObject.SetActive(show);
    }

    public void ToggleBotLine(bool show) {
        ImgLineBot.gameObject.SetActive(show);
    }

    public void ToggleLeftLine(bool show) {
        ImgLineLeft.gameObject.SetActive(show);
    }

    public void ToggleRightLine(bool show) {
        ImgLineRight.gameObject.SetActive(show);
    }

    public void SetLeftLineType(SO_RoadLine line) {
        ImgLineLeft.sprite = line.Sprite;
        ImgLineLeft.color = line.Color;
    }

    public void SetRightLineType(SO_RoadLine line) {
        ImgLineRight.sprite = line.Sprite;
        ImgLineRight.color = line.Color;
    }

    public override void SetFacing(Direction facing) {
        base.SetFacing(facing);

        switch (facing) {
            case Direction.Up:
                transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case Direction.Right:
                transform.localEulerAngles = new Vector3(0, 0, 270);
                break;
            case Direction.Down:
                transform.localEulerAngles = new Vector3(0, 0, 180);
                break;
            case Direction.Left:
                transform.localEulerAngles = new Vector3(0, 0, 90);
                break;
        }
    }

    public void FitRoad() {
        // Check auto neighbors
        List<Direction> neighbors = NeighborSystem.GetAllFittableAdjacentDirections();
        foreach (Direction direction in neighbors) {
            
        }
        // Do stuff depending on the results
    }


    #region A-Star
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
    #endregion
}
