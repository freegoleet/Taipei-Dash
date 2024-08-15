using TMPro;
using Traffic;
using UnityEngine;

public class TileRoad : Tile
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

    //public SO_TileRoad Data { get; private set; }

    public override void Initialize<T>(T data, int col = -1, int row = -1) {
        Data = data as SO_TileRoad;
        base.Initialize(Data, col, row);
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
