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
    private Transform m_ActiveTileContainer = null;
    [SerializeField]
    private Transform m_DecorativeTileContainer = null;

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
    public TileManager TileManager { get; private set; } = null;

    public int TileSize { get => m_TileSize; }
    public int Rows { get => m_Rows; }
    public int Cols { get => m_Cols; }
    public SOTile_List TileList { get { return m_TileList; } }
    public SOTileSettings TileSettings { get { return m_TileSettings; } }
    public Transform ActiveTileContainer { get => m_ActiveTileContainer; }
    public Transform DecorativeTileContainer { get => m_DecorativeTileContainer; }

    private List<int> m_CurrentWallIndexes = new List<int>();
    private int m_MaxTileCount = -1;
    private GridData m_GridData = null;

    public class GridData
    {
        public int[] Tiles { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
    }

    public void SetupManager() {
        TileManager = new TileManager(this, TileList, ActiveTileContainer, DecorativeTileContainer, Rows);
    }

    public Vector2 GetGridCameraOffset() {
        return new Vector2(
            Camera.main.transform.position.x - (Cols * TileSize) * 0.5f + TileSize * 0.5f,
            Camera.main.transform.position.y - (Rows * TileSize) * 0.5f + TileSize * 0.5f);
    }

    public void GenerateMap() {
        Tile tile = null;
        SO_Tile tileData = null;
        int currentTile = 0;
        int tileTypeIndex = 0;

        transform.position = GetGridCameraOffset();
        m_GridData = new GridData();
        m_MaxTileCount = Rows * Cols;
        m_GridData.Tiles = new int[m_MaxTileCount];
        m_GridData.Rows = Rows;
        m_GridData.Cols = Cols;

        m_CurrentWallIndexes.Clear();
        TileManager.Reset();
        TileManager.NewGridSize(m_MaxTileCount);

        if (TileManager.GetGameplayTilecount() > m_MaxTileCount) {
            List<Tile> tilesToReturn = new();

            for (int i = m_MaxTileCount; i < TileManager.GetGameplayTilecount(); i++) {
                tilesToReturn.Add(TileManager.GetAllGameplayTiles()[i]);
            }

            foreach (Tile t in tilesToReturn) {
                TileManager.ReturnTile(t);
            }
        }

        for (int row = 0; row < Rows; row++) {
            for (int col = 0; col < Cols; col++) {
                Vector2Int pos = new Vector2Int(col, row);
                // TODO: Figure out how to choose tile type
                tileTypeIndex = 0;
                tileData = m_TileList.GameplayTiles[tileTypeIndex];

                if (currentTile < TileManager.GetGameplayTilecount()) {
                    tile = TileManager.GetAllGameplayTiles()[currentTile];
                }
                else {
                    tile = TileManager.GetNewTile(TileType.Autofit, pos, currentTile);
                }

                tile.Initialize(tileData, pos);
                m_GridData.Tiles[currentTile] = tileTypeIndex;
                currentTile++;
            }
        }

        for (int i = 0; i < TileManager.GameplayTilecount; i++) {
            Tile t = TileManager.AllTiles[i];
            SetNeighbors(t);
        }

        //for (int i = 0; i < TileManager.GameplayTilecount; i++) {
        //    TileAutofit autofitTile = TileManager.SidewalkTileManager.AllTiles[i];
        //    var neighbors = GetNeighbors(autofitTile);
        //    for (int j = 0; j < neighbors.Length; j++) {
        //        if (neighbors[j] != null) {
        //            autofitTile.AutofitNeighbors.SetFittableNeighborExists(j, true);
        //        }
        //    }
        //}

        for (int i = 0; i < TileManager.GameplayTilecount; i++) {
            TileManager.SetupAutofit(TileManager.GetAllGameplayTiles()[i], false);
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

        TileManager.DecoTileManager.SetupDecorativeLayers(Rows);
        OnMapGenerated?.Invoke(TileManager.GetAllTiles());
    }

    public void SetNeighbors(Tile tile, bool setNeighborsNewNeighbor = false) {
        NodeBase[] neighbors = GetNeighbors(tile);
        for (int i = 0; i < neighbors.Length; i++) {
            if (neighbors[i] == null) {
                continue;
            }
            Tile neighboringTile = (Tile)neighbors[i];
            switch (i) {
                case 0: // Up
                    tile.NeighborSystem.Neighbors[(Direction.Up, Direction.None)].Tile = neighboringTile;
                    if(setNeighborsNewNeighbor == true) {
                        neighboringTile.NeighborSystem.Neighbors[(Direction.Down, Direction.None)].Tile = tile;
                    }
                    break;
                case 1: // Down
                    tile.NeighborSystem.Neighbors[(Direction.Down, Direction.None)].Tile = neighboringTile;
                    if (setNeighborsNewNeighbor == true) {
                        neighboringTile.NeighborSystem.Neighbors[(Direction.Up, Direction.None)].Tile = tile;
                    }
                    break;
                case 2: // Left
                    tile.NeighborSystem.Neighbors[(Direction.Left, Direction.None)].Tile = neighboringTile;
                    if (setNeighborsNewNeighbor == true) {
                        neighboringTile.NeighborSystem.Neighbors[(Direction.Right, Direction.None)].Tile = tile;
                    }
                    break;
                case 3: // Right
                    tile.NeighborSystem.Neighbors[(Direction.Right, Direction.None)].Tile = neighboringTile;
                    if (setNeighborsNewNeighbor == true) {
                        neighboringTile.NeighborSystem.Neighbors[(Direction.Left, Direction.None)].Tile = tile;
                    }
                    break;
            }
        }
        
        neighbors = GetDiagonalNeighbors(tile);
        for (int i = 0; i < neighbors.Length; i++) {
            if (neighbors[i] == null) {
                continue;
            }
            Tile neighboringTile = (Tile)neighbors[i];
            switch (i) {
                case 0: // Up-Left
                    tile.NeighborSystem.Neighbors[(Direction.Up, Direction.Left)].Tile = neighboringTile;
                    if (setNeighborsNewNeighbor == true) {
                        neighboringTile.NeighborSystem.Neighbors[(Direction.Down, Direction.Right)].Tile = tile;
                    }
                    break;
                case 1: // Up_Right
                    tile.NeighborSystem.Neighbors[(Direction.Up, Direction.Right)].Tile = neighboringTile;
                    if (setNeighborsNewNeighbor == true) {
                        neighboringTile.NeighborSystem.Neighbors[(Direction.Down, Direction.Left)].Tile = tile;
                    }
                    break;
                case 2: // Down-Left
                    tile.NeighborSystem.Neighbors[(Direction.Down, Direction.Left)].Tile = neighboringTile;
                    if (setNeighborsNewNeighbor == true) {
                        neighboringTile.NeighborSystem.Neighbors[(Direction.Up, Direction.Right)].Tile = tile;
                    }
                    break;
                case 3: // Down-Right
                    tile.NeighborSystem.Neighbors[(Direction.Down, Direction.Right)].Tile = neighboringTile;
                    if (setNeighborsNewNeighbor == true) {
                        neighboringTile.NeighborSystem.Neighbors[(Direction.Up, Direction.Left)].Tile = tile;
                    }
                    break;
            }
        }
    }

    public void GenerateMap(GridData networkGridData) {
        TileGameplay tile = null;
        int currentTile = 0;

        transform.position = new Vector3(Camera.main.transform.position.x - networkGridData.Cols / 2 + 0.5f, Camera.main.transform.position.y - networkGridData.Rows / 2 + 0.5f, 0f);

        for (int row = 0; row < networkGridData.Rows; row++) {
            for (int col = 0; col < networkGridData.Cols; col++) {
                Vector2Int pos = new Vector2Int(col, row);

                if (currentTile < TileManager.SidewalkTileManager.TileCount) {
                    tile = TileManager.SidewalkTileManager.AllTiles[currentTile];
                }
                else {
                    //tile = TileManager.SidewalkTileManager.GetNewTile(pos);
                }

                tile.gameObject.name = "x: " + col + " y: " + row;
                tile.transform.localPosition = new Vector3(col, row);

                //tile.Initialize(TileList.GameplayTiles[networkGridData.Tiles[currentTile]], col, row);

                currentTile++;

                // May not need to be cleared
                //Vector2 loc = new Vector2(col, row);
                //if (TileManager.SidewalkTileManager.ActiveTilesByLocation.ContainsKey(loc) == false) {
                //    TileManager.SidewalkTileManager.ActiveTilesByLocation.Add(loc, tile);
                //}
            }
        }

        OnMapGenerated?.Invoke(TileManager.GetAllTiles());
    }

    public TileGameplay GetActiveTileAtLocation(Vector2Int location) {
        return TileManager.SidewalkTileManager.ActiveTilesByGridpos[location];
    }

    public TileDeco GetDecorativeTileAtLocation(int layer, Vector2 position) {
        if (TileManager.DecoTileManager.DecoLayers.ContainsKey(layer) == true) {
            return TileManager.DecoTileManager.DecoLayers[layer].GetTile(position);
        }
        return null;
    }

    /// <summary>
    /// Returns neighbors in order of: Up>Down>Left>Right.
    /// </summary>
    /// <param name="node">Node to get neighbors of.</param>
    /// <returns></returns>
    public NodeBase[] GetNeighbors(NodeBase node) {
        NodeBase[] neighbors = new NodeBase[4];

        NodeBase[] tiles = TileManager.GetAllActiveGameplayTiles().ToArray<NodeBase>();

        int index = Array.IndexOf(tiles, node) + 1;

        if (index + Cols <= TileManager.GameplayTilecount) // Up
        {
            neighbors[0] = tiles[index - 1 + Cols];
        }

        if (index - Cols > 0) // Down
        {
            neighbors[1] = tiles[index - 1 - Cols];
        }

        if ((index - 1) % Cols != 0) // Left
        {
            neighbors[2] = tiles[index - 2];
        }

        if (index % Cols != 0) // Right
        {
            neighbors[3] = tiles[index];
        }

        return neighbors;
    }

    /// <summary>
    /// Returns diagonal neighbors in order of: Top-Left, Top-Right, Bot-Left, Bot-Right
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public NodeBase[] GetDiagonalNeighbors(NodeBase node) {
        NodeBase[] neighbors = new NodeBase[4];

        NodeBase[] tiles = TileManager.GetAllActiveGameplayTiles().ToArray<NodeBase>();

        int index = Array.IndexOf(tiles, node) + 1;

        if (index % Cols != 0) { // Right
            if (index + Cols <= TileManager.GameplayTilecount) // Up
            {
                neighbors[1] = tiles[index + Cols];
            }

            if (index - Cols > 0) // Down
            {
                neighbors[3] = tiles[index - Cols];
            }
        }

        if ((index - 1) % Cols != 0) // Left
        {
            if (index + Cols <= TileManager.GameplayTilecount) // Up
            {
                neighbors[0] = tiles[index - 2 + Cols];
            }

            if (index - Cols > 0) // Down
            {
                neighbors[2] = tiles[index - 2 - Cols];
            }
        }

        return neighbors;
    }



    public float GetDistance(NodeBase from, NodeBase to) {
        return Mathf.Abs(to.Col - from.Col) + Mathf.Abs(to.Row - from.Row);
    }

    public Vector2 GetDirection(NodeBase from, NodeBase to) {
        return new Vector2(to.Col - from.Col, to.Row - from.Row);
    }

    public NodeBase GetTileClosestToPosition(Vector2 mousePos, int decoLayer = -1) {
        Vector2 offset = transform.position;
        Vector2 convertedPos = (mousePos - offset) / TileSize;
        Vector2Int posInGrid = new Vector2Int(Convert.ToInt32(convertedPos.x), Convert.ToInt32(convertedPos.y));
        Vector2Int result = new Vector2Int();

        if (posInGrid.x < 0) {
            result.x = 0;
        }
        else if (posInGrid.x > Cols - 1) {
            result.x = Cols - 1;
        }
        else {
            result.x = posInGrid.x;
        }

        if (posInGrid.y < 0) {
            result.y = 0;
        }
        else if (posInGrid.y > Rows - 1) {
            result.y = Rows - 1;
        }
        else {
            result.y = posInGrid.y;
        }

        //List<NodeBase> list = TileManager.GetAllActiveGameplayTiles().ToList<NodeBase>();
        //if (decoLayer != -1) {
        //list.AddRange(TileManager.DecoTileManager.DecoLayers[decoLayer].TilesByLocation.Values.ToList());
        //}

        //NodeBase closest = list.First();
        //float currentMag = (closest.transform.position - pos).magnitude;

        //foreach (NodeBase tile in list) {
        //    float nextTileMag = (tile.transform.position - pos).magnitude;
        //    if (nextTileMag < currentMag) {
        //        closest = tile;
        //        currentMag = nextTileMag;
        //    }
        //}

        return TileManager.GetTileByPos(result);
    }

    public void ToggleCoords(bool show) {
        foreach (NodeBase node in TileManager.SidewalkTileManager.AllTiles) {
            TileGameplay tile = node as TileGameplay;
            tile.ToggleCoords(show);
        }
    }

    public void ToggleLayerVisibility(int layerIndex, bool show) {
        TileManager.DecoTileManager.DecoLayers[layerIndex].ToggleVisibility(show);
    }

    public void DestroyAllTiles() {
        if (TileManager == null) {
            SetupManager();
        }

        TileManager.Reset();
    }
}
