using System.Collections.Generic;
using System.Linq;
using TMPro;
using Traffic;
using UnityEngine;

public class TileRoad : TileGameplay
{
    [Header("Traffic Details")]
    [SerializeField]
    public DebugDirections m_DebugDirections = null;
    [SerializeField]
    public RoadPointer m_RoadPointer = null;

    [Header("Lines")]
    [SerializeField]
    private SpriteRenderer m_ImgLineTop = null;
    [SerializeField]
    private SpriteRenderer m_ImgLineBot = null;
    [SerializeField]
    private SpriteRenderer m_ImgLineLeft = null;
    [SerializeField]
    private SpriteRenderer m_ImgLineRight = null;
    [SerializeField]
    private SpriteRenderer m_ImgCrosswalk = null;

    [Header("AStar Values")]
    [SerializeField]
    private GameObject m_AStarValues = null;
    [SerializeField]
    private TextMeshPro m_GCost = null;
    [SerializeField]
    private TextMeshPro m_HCost = null;
    [SerializeField]
    private TextMeshPro m_FCost = null;

    public RoadPointer Pointer { get => m_RoadPointer; }
    public DebugDirections DebugDirections { get => m_DebugDirections; }
    public SpriteRenderer LineTop { get => m_ImgLineTop; }
    public SpriteRenderer LineBot { get => m_ImgLineBot; }
    public SpriteRenderer LineLeft { get => m_ImgLineLeft; }
    public SpriteRenderer LineRight { get => m_ImgLineRight; }
    public SpriteRenderer Crosswalk { get => m_ImgCrosswalk; }

    public bool HasCrosswalk { get; set; } = false;
    public bool HasStopLine { get; set; } = false;
    public int ConnectionIndex { get; set; } = 0;
    public TrafficLight TrafficLight { get; private set; } = null;
    public RoadtileNeighbors RoadtileNeighbors { get; private set; } = null;
    public bool GreenLit { get; set; } = true;

    public Dictionary<Direction, bool> InConnections { get; set; } = new Dictionary<Direction, bool>() {
        { Direction.Up, false },
        { Direction.Down, false },
        { Direction.Left, false },
        { Direction.Right, false },
    };

    public Dictionary<Direction, bool> OutConnections { get; set; } = new Dictionary<Direction, bool>() {
        { Direction.Up, false },
        { Direction.Down, false },
        { Direction.Left, false },
        { Direction.Right, false },
    };

    private Dictionary<Direction, bool> Lines { get; set; } = new() {
        { Direction.Up, false },
        { Direction.Down, false },
        { Direction.Left, false },
        { Direction.Right, false },
    };

    public override void Initialize(SO_Tile data, Vector2Int gridPos, bool cursor = false) {
        base.Initialize(data, gridPos, cursor);
        RoadtileNeighbors = new RoadtileNeighbors(this);
        Pointer.ToggleShow(false);
    }

    public void ToggleShowConnections(bool show) {
        DebugDirections.ToggleShowDebugDirections(show);
    }

    private void ToggleTopLine(bool show) {
        LineTop.gameObject.SetActive(show);
        Lines[Direction.Up] = show;
    }

    private void ToggleBotLine(bool show) {
        LineBot.gameObject.SetActive(show);
        Lines[Direction.Down] = show;
    }

    private void ToggleLeftLine(bool show) {
        LineLeft.gameObject.SetActive(show);
        Lines[Direction.Left] = show;
    }

    private void ToggleRightLine(bool show) {
        LineRight.gameObject.SetActive(show);
        Lines[Direction.Right] = show;
    }

    public void ToggleCrosswalk(bool show) {
        foreach (Direction dir in Lines.Keys.ToList()) {
            Lines[dir] = !show;
        }
        HasCrosswalk = show;
        Crosswalk.gameObject.SetActive(show);
        SetCrosswalkDir(Facing);

        if(show == true) {
            ToggleLine(Direction.Up, false);
            ToggleLine(Direction.Down, false);
            ToggleLine(Direction.Left, false);
            ToggleLine(Direction.Right, false);
        }
    }

    public void SetCrosswalkDir(Direction dir) {
        switch (dir) {
            case Direction.Up:
                Crosswalk.transform.localEulerAngles = new Vector3(0, 0, 90);
                break;
            case Direction.Right:
                Crosswalk.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case Direction.Down:
                Crosswalk.transform.localEulerAngles = new Vector3(0, 0, 90);
                break;
            case Direction.Left:
                Crosswalk.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
        }
    }

    public bool GetHasLine(Direction dir) {
        bool hasLine = false;
        switch (dir) {
            case Direction.Up:
                hasLine = LineTop.gameObject.activeSelf;
                break;
            case Direction.Right:
                hasLine = LineRight.gameObject.activeSelf;
                break;
            case Direction.Down:
                hasLine = LineBot.gameObject.activeSelf;
                break;
            case Direction.Left:
                hasLine = LineLeft.gameObject.activeSelf;
                break;
        }
        return hasLine;
    }

    public void SetLineType(Direction dir, SO_RoadLine line) {
        SpriteRenderer renderer = null;

        switch (dir) {
            case Direction.Up:
                renderer = LineTop;
                break;
            case Direction.Right:
                renderer = LineRight;
                break;
            case Direction.Down:
                renderer = LineBot;
                break;
            case Direction.Left:
                renderer = LineLeft;
                break;
        }

        renderer.sprite = line.Sprite;
        renderer.color = line.Color;
    }

    public void ToggleLine(Direction dir, bool show) {
        if(HasCrosswalk == true && show == true) {
            return;
        }

        switch (dir) {
            case Direction.Up:
                ToggleTopLine(show);
                break;
            case Direction.Right:
                ToggleRightLine(show);
                break;
            case Direction.Down:
                ToggleBotLine(show);
                break;
            case Direction.Left:
                ToggleLeftLine(show);
                break;
        }
    }

    public void TogglePointer(bool show) {
        Pointer.gameObject.SetActive(show);
    }

    public void AddInConnection(Direction connection) {
        if (InConnections.ContainsKey(connection) == false) {
            return;
        }
        if (InConnections[connection] == true) {
            return;
        }

        InConnections[connection] = true;
        UpdateDebugDirections();
    }

    public void AddOutConnection(Direction connection) {
        if (OutConnections.ContainsKey(connection) == false) {
            return;
        }
        if (OutConnections[connection] == true) {
            return;
        }
        OutConnections[connection] = true;
        UpdateDebugDirections();
    }

    public void RemoveInConnection(Direction connection) {
        if (InConnections.ContainsKey(connection) == false) {
            return;
        }
        if (InConnections[connection] == false) {
            return;
        }
        InConnections[connection] = false;
        UpdateDebugDirections();
    }

    public void RemoveOutConnection(Direction connection) {
        if (OutConnections.ContainsKey(connection) == false) {
            return;
        }
        if (OutConnections[connection] == false) {
            return;
        }
        OutConnections[connection] = false;
        UpdateDebugDirections();
    }

    public void SetOutConnections(Dictionary<Direction, bool> connections) {
        OutConnections = connections;
        UpdateDebugDirections();
    }

    public void UpdateDebugDirections() {
        bool showDir = false;
        foreach (var inCons in InConnections) {
            foreach (var outCons in OutConnections) {
                showDir = false;
                if (inCons.Value == true && outCons.Value == true) {
                    showDir = true;
                }
                DebugDirections.ToggleDebugDirection(inCons.Key, outCons.Key, showDir);
            }
        }
    }

    public void AddToTrafficLight(TrafficLight trafficLight) {
        TrafficLight = trafficLight;
    }

    public void RemoveFromTrafficLight() {
        TrafficLight = null;
    }

    public override void SetFacing(Direction facing) {
        base.SetFacing(facing);
        bool leftTurn = false;

        switch (facing) {
            case Direction.Up:
                Pointer.transform.localEulerAngles = new Vector3(0, leftTurn ? 180 : 0, leftTurn ? 180 : 0);
                break;
            case Direction.Right:
                Pointer.transform.localEulerAngles = new Vector3(0, leftTurn ? 180 : 0, leftTurn ? 90 : 270);
                break;
            case Direction.Down:
                Pointer.transform.localEulerAngles = new Vector3(0, leftTurn ? 180 : 0, leftTurn ? 0 : 180);
                break;
            case Direction.Left:
                Pointer.transform.localEulerAngles = new Vector3(0, leftTurn ? 180 : 0, leftTurn ? 270 : 90);
                break;
        }
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