using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Linq;
using Traffic;
using UnityEngine;

public class TileManager
{
    public DecorativeTileManager DecoTileManager { get; private set; } = null;
    public GameplayTileManager<TileSidewalk> SidewalkTileManager { get; private set; } = null;
    public GameplayTileManager<TileRoad> RoadTileManager { get; private set; } = null;
    public GameplayTileManager<TileGameplay> GameplayTileManager { get; private set; } = null;

    public Tile[] AllTiles { get; private set; } = new Tile[0];
    public Dictionary<Vector2Int, Tile> AllTilesByGridpos { get; private set; } = new();

    public TileRoad RoadTile { get; private set; } = null;
    public TileSidewalk SidewalkTile { get; private set; } = null;
    public TileDeco DecoTile { get; private set; } = null;
    public TileGameplay GameplayTile { get; private set; } = null;
    public GridManager GridManager { get; private set; } = null;

    public int GameplayTilecount { get; set; } = 0;

    public TileManager(GridManager gridManager, SOTile_List tileList, Transform activeTileContainer, Transform decoContainer, int decoLayerCount) {
        RoadTile = tileList.RoadTile;
        SidewalkTile = tileList.SidewalkTile;
        DecoTile = tileList.DecoTile;
        GameplayTile = tileList.GameplayTile;
        GridManager = gridManager;
        NewGridSize(GridManager.Cols * GridManager.Rows);

        DecoTileManager = new DecorativeTileManager(decoLayerCount, tileList.DecoTile, decoContainer);
        SidewalkTileManager = new GameplayTileManager<TileSidewalk>(TileType.Autofit, tileList.SidewalkTile, activeTileContainer, this);
        RoadTileManager = new GameplayTileManager<TileRoad>(TileType.Road, tileList.RoadTile, activeTileContainer, this);
        GameplayTileManager = new GameplayTileManager<TileGameplay>(TileType.Gameplay, tileList.GameplayTile, activeTileContainer, this);
        SetupAllTiles();
    }

    public void NewGridSize(int size) {
        AllTiles = new Tile[size];
    }

    public int GetGameplayTilecount() {
        return RoadTileManager.TileCount + SidewalkTileManager.TileCount;
    }

    public List<Tile> GetAllTiles() {
        List<Tile> tiles = new List<Tile>();
        tiles.AddRange(SidewalkTileManager.AllTiles);
        tiles.AddRange(RoadTileManager.AllTiles);
        tiles.AddRange(DecoTileManager.AllTiles);
        tiles.AddRange(GameplayTileManager.AllTiles);

        return tiles;
    }

    public List<TileGameplay> GetAllGameplayTiles() {
        List<TileGameplay> tiles = new List<TileGameplay>();
        tiles.AddRange(SidewalkTileManager.AllTiles);
        tiles.AddRange(RoadTileManager.AllTiles);
        tiles.AddRange(GameplayTileManager.AllTiles);

        return tiles;
    }

    public Tile[] GetAllActiveGameplayTiles() {
        return AllTiles;
    }

    public Tile ReplaceTile(Tile oldTile, TileType newTileType) {
        Tile newTile = GetNewTile(newTileType, oldTile.GridPosition, oldTile.transform.GetSiblingIndex());
        ReturnTile(oldTile);
        GridManager.SetNeighbors(newTile, true);
        return newTile;
    }

    public Tile GetTileByPos(Vector2Int gridPos) {
        return AllTilesByGridpos[gridPos];
    }

    public Tile GetNewTile(TileType tileType, Vector2Int pos, int index = -1) {
        switch (tileType) {
            case TileType.Road:
                return RoadTileManager.GetNewTile(pos, index);
            case TileType.Autofit:
                return SidewalkTileManager.GetNewTile(pos, index);
            case TileType.Deco:
                return DecoTileManager.GetNewTile();
            case TileType.None:
                return null;
            case TileType.Gameplay:
                return GameplayTileManager.GetNewTile(pos, index);
        }
        return null;
    }

    public void ReturnTile(Tile tile) {
        switch (tile.TileType) {
            case TileType.Road:
                RoadTileManager.RemoveTile((TileRoad)tile);
                break;
            case TileType.Autofit:
                SidewalkTileManager.RemoveTile((TileSidewalk)tile);
                break;
            case TileType.Deco:
                TileDeco decotile = (TileDeco)tile;
                DecoTileManager.ReturnTile(decotile, decotile.Layer);
                break;
            case TileType.None:
                break;
            case TileType.Gameplay:
                GameplayTileManager.RemoveTile((TileGameplay)tile);
                break;
        }
    }

    public void Reset() {
        DecoTileManager.DestroyAllTiles();
        RoadTileManager.DestroyAllTiles();
        SidewalkTileManager.DestroyAllTiles();
        GameplayTileManager.DestroyAllTiles();
        AllTilesByGridpos.Clear();
        AllTiles = null;
        GameplayTilecount = 0;
    }

    private void SetupAllTiles() {
        List<Tile> tiles = new List<Tile>();
        tiles.AddRange(RoadTileManager.AddTiles());
        tiles.AddRange(SidewalkTileManager.AddTiles());
        tiles.AddRange(GameplayTileManager.AddTiles());
        tiles.OrderBy(tile => tile.GridPosition.x);
        AllTiles = tiles.ToArray();
    }

    public List<TileAutofit> SetupAutofit(TileGameplay tile, bool original) {
        List<TileAutofit> tiles = SetAutoTileNeighbors(tile);
        List<TileAutofit> newList = new();

        if (original == false) {
            return tiles;
        }

        foreach (TileAutofit tileAutofit in tiles) {
            if (original == true) {
                newList.AddRange(SetupAutofit(tileAutofit, false));
            }
        }

        foreach (TileAutofit tileAutofit in tiles) {
            tileAutofit.FitTile();
        }

        foreach (TileAutofit tileAutofit in tiles) {
            newList.Remove(tileAutofit);
        }

        newList = newList.GroupBy(x => x.GridPosition).Select(y => y.First()).ToList();

        foreach (TileAutofit tileAutofit in newList) {
            tileAutofit.FitTile();
        }

        foreach (TileAutofit tileAutofit in newList) {
            //tileAutofit.FixMiddle(GridManager.GetDiagonalNeighbors(tileAutofit));
        }

        return newList;
    }

    private List<TileAutofit> SetAutoTileNeighbors(Tile tile) {
        // TODO: Enum flags
        bool fittable = tile.TileType != TileType.Road;

        List<TileAutofit> tiles = new List<TileAutofit>();

        TileAutofit thisTile = null;
        if (tile is TileAutofit) {
            thisTile = (TileAutofit)tile;
        }

        foreach (KeyValuePair<(Direction, Direction), Neighbor> pair in tile.NeighborSystem.Neighbors) {
            if (pair.Value.Tile == null) {
                continue;
            }
            if (thisTile != null) {
                thisTile.NeighborSystem.Neighbors[pair.Key].Fittable = fittable;
            }

            if (pair.Value.Tile is TileAutofit afTile) {
                tiles.Add(afTile);
                afTile.NeighborSystem.Neighbors[TrafficLib.ReverseDirections(pair.Key)].Fittable = fittable;
            }
        }

        return tiles;
    }
}
