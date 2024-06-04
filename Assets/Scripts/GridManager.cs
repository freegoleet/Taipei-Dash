using System;
using System.Collections.Generic;
using System.Linq;
using Traffic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private SOTile_List m_TileList = null;
    [SerializeField]
    private SOTileSettings m_TileSettings = null;
    [SerializeField]
    private Transform m_TileContainer = null;
    [SerializeField]
    private Transform m_DecorativeTileContainer = null;
    [SerializeField]
    private Tile m_TilePrefab = null;

    [Header("Settings")]
    [SerializeField]
    private int m_TileSize = 10;
    [SerializeField]
    private int m_Rows = 10;
    [SerializeField]
    private int m_Cols = 10;
    [SerializeField]
    private int m_Walls = 10;
    [SerializeField]
    private int m_DecorativeLayerCount = 3;

    public Action<List<Tile>> OnMapGenerated = null;
    public SOTile_List TileList { get { return m_TileList; } }
    public SOTileSettings TileSettings { get { return m_TileSettings; } }
    public Tile TilePrefab { get { return m_TilePrefab; } }

    public int TileSize { get => m_TileSize; }
    public int Rows { get => m_Rows; }
    public int Cols { get => m_Cols; }
    public Transform TileContainer { get => m_TileContainer; }
    public Dictionary<int, List<Tile>> DecorativeLayers { get => m_DecorativeLayers; }

    private List<int> m_CurrentWallIndexes = new List<int>();
    private int m_MaxTiles = -1;
    private List<Tile> m_AllTiles = new();
    private List<Tile> m_ActiveTiles = new();
    private Dictionary<Vector2, Tile> m_ActiveTilesLocation = new();
    private List<Tile> m_InactiveTiles = new();

    private Dictionary<int, List<Tile>> m_DecorativeLayers = new();
    private Dictionary<(int, Vector2), Tile> m_DecorativeTilesLocation = new();

    private GridData m_GridData = null;

    public class GridData
    {
        public int[] Tiles { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
    }

    public void Start() {
        PlayModeStateChangedExample.OnEditMode = EnteredEditMode;
    }

    public void GenerateMap() {
        transform.position = new Vector3(Camera.main.transform.position.x - Cols / 2 + 0.5f, Camera.main.transform.position.y - Rows / 2 + 0.5f, 0f);
        Tile tile = null;

        m_MaxTiles = Rows * Cols;
        m_CurrentWallIndexes.Clear();
        m_ActiveTilesLocation.Clear();
        int currentTile = 0;
        m_GridData = new GridData();

        SO_Tile tileType = null;
        int tileTypeIndex = 0;

        m_GridData.Tiles = new int[m_MaxTiles];
        m_GridData.Rows = Rows;
        m_GridData.Cols = Cols;

        if (m_ActiveTiles.Count > m_MaxTiles) {
            List<Tile> tilesToReturn = new();

            for (int i = m_MaxTiles; i < m_ActiveTiles.Count; i++) {
                tilesToReturn.Add(m_ActiveTiles[i]);
            }

            foreach (Tile t in tilesToReturn) {
                ReturnTile(t);
            }
        }

        for (int row = 0; row < Rows; row++) {
            for (int col = 0; col < Cols; col++) {
                if (currentTile < m_ActiveTiles.Count) {
                    tile = m_ActiveTiles[currentTile];
                }
                else {
                    tile = GetTile();
                }

                // TODO: Figure out how to choose tile type
                tileTypeIndex = 0;
                tileType = m_TileList.Tiles[tileTypeIndex];

                Vector2 pos = new Vector2(col * TileSize, row * TileSize);

                tile.transform.localPosition = pos;
                tile.Initialize(tileType, col, row);

                // May not need to be cleared
                m_ActiveTilesLocation.Add(new Vector2(col, row), tile);

                m_GridData.Tiles[currentTile] = tileTypeIndex;

                currentTile++;
            }
        }

        // TODO: Figure out how to choose tile type
        //tileTypeIndex = 1;
        //tileType = m_TileList.Tiles[tileTypeIndex]; 

        //for (int wall = 0; wall < m_Walls; wall++) {
        //    int index = UnityEngine.Random.Range(0, m_ActiveTiles.Count);

        //    while (m_CurrentWallIndexes.Contains(index) == true) {
        //        index = UnityEngine.Random.Range(0, m_ActiveTiles.Count);
        //    }

        //    m_GridData.Tiles[index] = tileTypeIndex;

        //    tile = m_ActiveTiles[index];
        //    tile.Initialize(tileType);

        //    m_CurrentWallIndexes.Add(index);
        //}

        SetupDecorativeLayers();

        OnMapGenerated?.Invoke(m_ActiveTiles);
    }

    public void GenerateMap(GridData networkGridData) {
        Tile tile = null;
        int currentTile = 0;

        transform.position = new Vector3(Camera.main.transform.position.x - networkGridData.Cols / 2 + 0.5f, Camera.main.transform.position.y - networkGridData.Rows / 2 + 0.5f, 0f);

        for (int row = 0; row < networkGridData.Rows; row++) {
            for (int col = 0; col < networkGridData.Cols; col++) {

                if (currentTile < m_ActiveTiles.Count) {
                    tile = m_ActiveTiles[currentTile];
                }
                else {
                    tile = GetTile();
                }

                tile.gameObject.name = "x: " + col + " y: " + row;
                tile.transform.localPosition = new Vector3(col, row);


                tile.Initialize(TileList.Tiles[networkGridData.Tiles[currentTile]], col, row);

                currentTile++;

                // May not need to be cleared
                Vector2 loc = new Vector2(col, row);
                if (m_ActiveTilesLocation.ContainsKey(loc) == false) {
                    m_ActiveTilesLocation.Add(loc, tile);
                }
            }
        }

        OnMapGenerated?.Invoke(m_ActiveTiles);
    }

    public void SetupDecorativeLayers() {
        for (int i = 0; i < m_DecorativeLayerCount; i++) {
            if (DecorativeLayers[i] != null) {
                continue;
            }
            DecorativeLayers.Add(i, new());
        }
    }

    private Tile GetTile() {
        Tile tile = null;

        if (m_InactiveTiles.Count > 0) {
            tile = m_InactiveTiles[0];

            tile.gameObject.SetActive(true);
            m_InactiveTiles.Remove(tile);
            m_ActiveTiles.Add(tile);

            return tile;
        }

        tile = Instantiate(m_TilePrefab, TileContainer);
        SceneVisibilityManager.instance.DisablePicking(tile.gameObject, true);

        m_AllTiles.Add(tile);
        m_ActiveTiles.Add(tile);

        return tile;
    }

    public Tile GetActiveTileAtLocation(Vector2 location) {
        return m_ActiveTilesLocation[location];
    }

    public Tile GetDecorativeTileAtLocation(int layer, Vector2 location) {
        return m_DecorativeTilesLocation[(layer, location)];
    }

    private void ReturnTile(Tile tile) {
        tile.Entity = null;
        m_ActiveTiles.Remove(tile);
        m_InactiveTiles.Add(tile);
        tile.gameObject.SetActive(false);
    }

    private void ReturnDecorativeTile(int layer, Tile tile) {
        tile.Entity = null;
        DecorativeLayers[layer].Remove(tile);
        m_InactiveTiles.Add(tile);
        tile.gameObject.SetActive(false);
    }

    public List<NodeBase> GetNeighbors(Tile node) {
        List<NodeBase> neighbors = new List<NodeBase>();

        int index = m_ActiveTiles.IndexOf(node) + 1;

        if (index + Cols <= m_ActiveTiles.Count) // Up
        {
            neighbors.Add(m_ActiveTiles[index - 1 + Cols]);
        }

        if (index - Cols > 0) // Down
        {
            neighbors.Add(m_ActiveTiles[index - 1 - Cols]);
        }

        if ((index - 1) % Cols != 0) // Left
        {
            neighbors.Add(m_ActiveTiles[index - 2]);
        }

        if ((index) % Cols != 0) // Right
        {
            neighbors.Add(m_ActiveTiles[index]);
        }

        return neighbors;
    }

    public float GetDistance(NodeBase from, NodeBase to) {
        return Mathf.Abs(to.Col - from.Col) + Mathf.Abs(to.Row - from.Row);
    }

    public Vector2 GetDirection(NodeBase from, NodeBase to) {
        return new Vector2(to.Col - from.Col, to.Row - from.Row);
    }

    public Tile GetTileClosestToPosition(Vector3 pos, int decLayer = -1) {
        List<Tile> list = m_ActiveTiles;

        if (decLayer != -1) {
            list = DecorativeLayers[decLayer];
        }

        Tile closest = list[0];
        float currentMag = (closest.transform.position - pos).magnitude;

        for (int i = 0; i < list.Count; i++) {
            if (list[i].Entity != null) {
                continue;
            }

            float nextTileMag = (list[i].transform.position - pos).magnitude;
            if (nextTileMag < currentMag) {
                closest = list[i];
                currentMag = nextTileMag;
            }
        }

        return closest;
    }

    public List<Tile> GetActiveTiles() {
        return m_ActiveTiles;
    }

    public void ToggleCoords(bool show) {
        foreach (NodeBase node in m_ActiveTiles) {
            Tile tile = node as Tile;
            tile.ToggleCoords(show);
        }
    }

    public void DestroyAllTiles() {
        foreach (Tile tile in m_ActiveTiles) {
            if (tile == null) {
                continue;
            }
            DestroyImmediate(tile.gameObject);
        }

        foreach (List<Tile> tiles in DecorativeLayers.Values) {
            foreach (Tile tile in tiles) {
                if (tile == null) {
                    continue;
                }
                DestroyImmediate(tile.gameObject);
            }
        }

        m_ActiveTiles.Clear();
        m_AllTiles.Clear();
        m_ActiveTilesLocation.Clear();
        DecorativeLayers.Clear();
        m_DecorativeTilesLocation.Clear();
    }

    public void EnteredEditMode() {
        m_AllTiles = TileContainer.GetComponentsInChildren<Tile>().ToList();
        m_ActiveTiles = m_AllTiles;
    }

}
