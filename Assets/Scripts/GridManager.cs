using System;
using System.Collections.Generic;
using System.Linq;
using Traffic;
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

    public Action<List<Tile>> OnMapGenerated = null;
    public SOTile_List TileList { get { return m_TileList; } }
    public SOTileSettings TileSettings { get { return m_TileSettings; } }
    public Tile TilePrefab { get { return m_TilePrefab; } }

    public int TileSize { get => m_TileSize; }
    public int Rows { get => m_Rows; }
    public int Cols { get => m_Cols; }
    public Transform TileContainer { get => m_TileContainer; }

    private List<int> m_CurrentWallIndexes = new List<int>();
    private int m_MaxTiles = -1;

    public DecorativeTileManager DecoTileManager { get; private set; } = null;
    public GameplayTileManager GameplayTileManager { get; private set; } = null;

    private GridData m_GridData = null;

    public class GridData
    {
        public int[] Tiles { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
    }

    public void Start() {
        EnteredEditMode();
    }

    public void GenerateMap() {
        transform.position = new Vector3(Camera.main.transform.position.x - Cols / 2 + 0.5f, Camera.main.transform.position.y - Rows / 2 + 0.5f, 0f);
        Tile tile = null;

        m_MaxTiles = Rows * Cols;
        m_CurrentWallIndexes.Clear();
        GameplayTileManager.ActiveTilesByLocation.Clear();
        int currentTile = 0;
        m_GridData = new GridData();

        SO_Tile tileType = null;
        int tileTypeIndex = 0;

        m_GridData.Tiles = new int[m_MaxTiles];
        m_GridData.Rows = Rows;
        m_GridData.Cols = Cols;

        if (GameplayTileManager.TileCount > m_MaxTiles) {
            List<Tile> tilesToReturn = new();

            for (int i = m_MaxTiles; i < GameplayTileManager.TileCount; i++) {
                tilesToReturn.Add(GameplayTileManager.AllActiveTiles[i]);
            }

            foreach (Tile t in tilesToReturn) {
                GameplayTileManager.ReturnTile(t);
            }
        }

        for (int row = 0; row < Rows; row++) {
            for (int col = 0; col < Cols; col++) {
                if (currentTile < GameplayTileManager.TileCount) {
                    tile = GameplayTileManager.AllActiveTiles[currentTile];
                }
                else {
                    tile = GameplayTileManager.GetNewTile(TileType.Road);
                }

                // TODO: Figure out how to choose tile type
                tileTypeIndex = 0;
                tileType = m_TileList.Tiles[tileTypeIndex];

                Vector2 pos = new Vector2(col * TileSize, row * TileSize);

                tile.transform.localPosition = pos;
                tile.Initialize(tileType, col, row);

                // May not need to be cleared
                GameplayTileManager.ActiveTilesByLocation.Add(new Vector2(col, row), tile);

                m_GridData.Tiles[currentTile] = tileTypeIndex;

                currentTile++;
            }
        }

        #region othertiles
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
        #endregion

        DecoTileManager.SetupDecorativeLayers(Rows);
        OnMapGenerated?.Invoke(GameplayTileManager.AllActiveTiles);
    }

    public void GenerateMap(GridData networkGridData) {
        Tile tile = null;
        int currentTile = 0;

        transform.position = new Vector3(Camera.main.transform.position.x - networkGridData.Cols / 2 + 0.5f, Camera.main.transform.position.y - networkGridData.Rows / 2 + 0.5f, 0f);

        for (int row = 0; row < networkGridData.Rows; row++) {
            for (int col = 0; col < networkGridData.Cols; col++) {

                if (currentTile < GameplayTileManager.TileCount) {
                    tile = GameplayTileManager.AllActiveTiles[currentTile];
                }
                else {
                    tile = GameplayTileManager.GetNewTile(TileType.Gameplay);
                }

                tile.gameObject.name = "x: " + col + " y: " + row;
                tile.transform.localPosition = new Vector3(col, row);


                tile.Initialize(TileList.Tiles[networkGridData.Tiles[currentTile]], col, row);

                currentTile++;

                // May not need to be cleared
                Vector2 loc = new Vector2(col, row);
                if (GameplayTileManager.ActiveTilesByLocation.ContainsKey(loc) == false) {
                    GameplayTileManager.ActiveTilesByLocation.Add(loc, tile);
                }
            }
        }

        OnMapGenerated?.Invoke(GameplayTileManager.AllActiveTiles);
    }

    public Tile GetActiveTileAtLocation(Vector2 location) {
        return GameplayTileManager.ActiveTilesByLocation[location];
    }

    public TileDeco GetDecorativeTileAtLocation(int layer, Vector2 position) {
        if (DecoTileManager.DecoLayers.ContainsKey(layer) == true) {
            return DecoTileManager.DecoLayers[layer].GetTile(position);
        }
        return null;
    }

    public List<NodeBase> GetNeighbors(Tile node) {
        List<NodeBase> neighbors = new List<NodeBase>();
        var tiles = GameplayTileManager.AllTiles;
        int index = tiles.IndexOf(node) + 1;

        if (index + Cols <= GameplayTileManager.TileCount) // Up
        {
            neighbors.Add(tiles[index - 1 + Cols]);
        }

        if (index - Cols > 0) // Down
        {
            neighbors.Add(tiles[index - 1 - Cols]);
        }

        if ((index - 1) % Cols != 0) // Left
        {
            neighbors.Add(tiles[index - 2]);
        }

        if ((index) % Cols != 0) // Right
        {
            neighbors.Add(tiles[index]);
        }

        return neighbors;
    }

    public float GetDistance(NodeBase from, NodeBase to) {
        return Mathf.Abs(to.Col - from.Col) + Mathf.Abs(to.Row - from.Row);
    }

    public Vector2 GetDirection(NodeBase from, NodeBase to) {
        return new Vector2(to.Col - from.Col, to.Row - from.Row);
    }

    public Tile GetTileClosestToPosition(Vector3 pos, int decoLayer = -1) {
        IEnumerable<Tile> list = GameplayTileManager.AllActiveTiles;

        if (decoLayer != -1) {
            list = DecoTileManager.DecoLayers[decoLayer].TilesByLocation.Values.ToList();
        }

        Tile closest = list.First();
        float currentMag = (closest.transform.position - pos).magnitude;

        foreach (Tile tile in list) {
            float nextTileMag = (tile.transform.position - pos).magnitude;
            if (nextTileMag < currentMag) {
                closest = tile;
                currentMag = nextTileMag;
            }
        }

        //for (int i = 0; i < list.Count(); i++) {
        //    if (list[i].Entity != null) {
        //        continue;
        //    }

        //    float nextTileMag = (list[i].transform.position - pos).magnitude;
        //    if (nextTileMag < currentMag) {
        //        closest = list[i];
        //        currentMag = nextTileMag;
        //    }
        //}

        return closest;
    }

    public List<Tile> GetActiveTiles() {
        return GameplayTileManager.AllActiveTiles;
    }

    public void ToggleCoords(bool show) {
        foreach (NodeBase node in GameplayTileManager.AllActiveTiles) {
            Tile tile = node as Tile;
            tile.ToggleCoords(show);
        }
    }

    public void ToggleLayerVisibility(int layerIndex, bool show) {
        DecoTileManager.DecoLayers[layerIndex].ToggleVisibility(show);
    }

    public void DestroyAllTiles() {
        foreach (DecorativeTileLayer layer in DecoTileManager.DecoLayers.Values) {
            foreach (TileDeco tile in layer.TilesByLocation.Values) {
                if (tile == null) {
                    continue;
                }
                DestroyImmediate(tile.gameObject);
            }
        }
        DecoTileManager.DecoLayers.Clear();
    }

    public void EnteredEditMode() {
        GameplayTileManager.AddTiles(TileContainer.GetComponentsInChildren<Tile>().ToList());

        DecoTileManager.SetupDecorativeLayers(Rows);

        var tiles = m_DecorativeTileContainer.GetComponentsInChildren<Tile>(true).ToList();
        foreach (TileDeco tile in tiles) {
            DecoTileManager.DecoLayers[tile.Layer].AddTile(tile); 
        }

    }

}
