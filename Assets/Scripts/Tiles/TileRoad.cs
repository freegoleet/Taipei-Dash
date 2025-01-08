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
    public bool HasTrafficLight { get; set; } = false;
    public int ConnectionIndex { get; set; } = 0;
    public TrafficLight TrafficLight { get; private set; } = null;
    public RoadtileNeighbors RoadtileNeighbors { get; private set; } = null;
    public Connections Connections { get; private set; } = new Connections(4);
    public Dictionary<Direction, bool> Passable { get; set; } = new Dictionary<Direction, bool>() {
        { Direction.Up, true },
        { Direction.Down, true },
        { Direction.Left, true },
        { Direction.Right, true },
    };
    public Dictionary<Direction, LineType> ManualLines { get; set; } = new Dictionary<Direction, LineType>();

    private Dictionary<Direction, bool> Lines { get; set; } = null;

    public override void Initialize(SO_Tile data, Vector2Int gridPos, bool cursor = false) {
        base.Initialize(data, gridPos, cursor);
        RoadtileNeighbors = new RoadtileNeighbors(this);
        Pointer.ToggleShow(false);
        if (Lines == null) {
            Lines = new() {
                { Direction.Up, false },
                { Direction.Down, false },
                { Direction.Left, false },
                { Direction.Right, false },
            };
        }
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

        if (show == true) {
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

    public void SetLineType(Direction dir, SO_RoadLine line, bool manual = false) {
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

        if(manual == true) {
            ManualLines.TryAdd(dir, line.LineType);
        }
        else {
            ManualLines.Remove(dir);
        }

        ToggleLine(dir, true, manual);
    }

    public void ToggleLine(Direction dir, bool show, bool manual = false) {
        if(manual == false && ManualLines.ContainsKey(dir)) {
            return;
        }
        if (HasCrosswalk == true && show == true) {
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
        Connections.AddInConnection(connection);
        UpdateDebugDirections();
    }

    public void AddOutConnection(Direction connection) {
        Connections.AddOutConnection(connection);
        UpdateDebugDirections();
    }

    public void RemoveInConnection(Direction connection) {
        Connections.RemoveInConnection(connection);
        UpdateDebugDirections();
    }

    public void RemoveOutConnection(Direction connection) {
        Connections.RemoveOutConnection(connection);
        UpdateDebugDirections();
    }

    public void SetOutConnections(Dictionary<Direction, bool> connections) {
        foreach (var item in connections) {
            if (item.Value == true) {
                AddOutConnection(item.Key);
                continue;
            }
            RemoveOutConnection(item.Key);
        }
        UpdateDebugDirections();
    }

    public void SetAllConnections(Connections connections) {
        Connections = connections;
        UpdateDebugDirections();
    }

    public void UpdateDebugDirections() {
        for (int i = 0; i < Connections.InConnections.Length; i++) {
            bool inDir = true;
            if (Connections.InConnections[i] == false) {
                inDir = false;
            }
            for (int j = 0; j < Connections.OutConnections.Length; j++) {
                if (inDir == false) {
                    DebugDirections.ToggleDebugDirection((Direction)i, (Direction)j, inDir);
                    continue;
                }
                bool outDir = true;
                if (i == j) {
                    outDir = false;
                }
                if (Connections.OutConnections[j] == false) {
                    outDir = false;
                }
                bool final = outDir == true && inDir == true ? true : false;
                DebugDirections.ToggleDebugDirection((Direction)i, (Direction)j, final);
            }
        }
    }

    public void AddToTrafficLight(TrafficLight trafficLight) {
        TrafficLight = trafficLight;
        TrafficLight.AddRoad(this);
    }

    public void RemoveFromTrafficLight() {
        TrafficLight.RemoveRoad(this);
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