using System.Collections.Generic;
using System.Data;
using System.Linq;
using Traffic;
using UnityEngine;

public class TileManager
{
    public DecorativeTileManager DecoTileManager { get; private set; } = null;
    public GameplayTileManager<TileSidewalk> SidewalkTileManager { get; private set; } = null;
    public GameplayTileManager<TileRoad> RoadTileManager { get; private set; } = null;
    public GameplayTileManager<TileGameplay> GameplayTileManager { get; private set; } = null;
    public RoadUtils RoadUtils { get; private set; } = null;

    public Tile[] AllTiles { get; private set; } = new Tile[0];
    public Dictionary<Vector2Int, Tile> AllTilesByGridpos { get; private set; } = new();

    public TileRoad RoadTile { get; private set; } = null;
    public TileSidewalk SidewalkTile { get; private set; } = null;
    public TileDeco DecoTile { get; private set; } = null;
    public TileGameplay GameplayTile { get; private set; } = null;
    public GridManager GridManager { get; private set; } = null;

    public int GameplayTileCount { get; set; } = 0;

    public TileManager(GridManager gridManager, SOTile_List tileList, Transform activeTileContainer, Transform decoContainer, int decoLayerCount) {
        RoadTile = tileList.RoadTile;
        SidewalkTile = tileList.SidewalkTile;
        DecoTile = tileList.DecoTile;
        GameplayTile = tileList.GameplayTile;
        GridManager = gridManager;
        RoadUtils = new RoadUtils(gridManager);
        NewGridSize(GridManager.Cols * GridManager.Rows);

        DecoTileManager = new DecorativeTileManager(decoLayerCount, tileList.DecoTile, decoContainer);
        SidewalkTileManager = new GameplayTileManager<TileSidewalk>(TileType.Autofit, tileList.SidewalkTile, activeTileContainer, this);
        RoadTileManager = new GameplayTileManager<TileRoad>(TileType.Road, tileList.RoadTile, activeTileContainer, this);
        GameplayTileManager = new GameplayTileManager<TileGameplay>(TileType.Gameplay, tileList.GameplayTile, activeTileContainer, this);
    }

    public void NewGridSize(int size) {
        AllTiles = new Tile[size];
    }

    public int GetGameplayTilecount() {
        return RoadTileManager.TileCount + SidewalkTileManager.TileCount;
    }

    public Tile[] GetAllTiles() {
        return AllTiles;
    }

    public List<TileGameplay> GetAllGameplayTiles() {
        List<TileGameplay> tiles = new List<TileGameplay>();
        tiles.AddRange(SidewalkTileManager.Tiles);
        tiles.AddRange(RoadTileManager.Tiles);
        tiles.AddRange(GameplayTileManager.Tiles);
        tiles.OrderBy(tile => tile.GridPosition.x);
        return tiles;
    }

    public Tile ReplaceTile(Tile oldTile, TileType newTileType) {
        Vector2Int gridpos = oldTile.GridPosition;
        int siblingIndex = oldTile.transform.GetSiblingIndex();
        ReturnTile(oldTile);
        Tile newTile = GetNewTile(newTileType, gridpos, siblingIndex);
        return newTile;
    }

    public Tile GetTileByPos(Vector2Int gridPos) {
        return AllTilesByGridpos[gridPos];
    }

    public Tile GetNewTile(TileType tileType, Vector2Int pos, int index = -1) {
        if (AllTilesByGridpos.ContainsKey(pos) == true) {
            if (AllTilesByGridpos[pos] != null) {
                return null;
            }
        }

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
        GameplayTileCount = 0;
    }

    public void SetupAllTiles() {
        List<Tile> tiles = new List<Tile>();
        tiles.AddRange(RoadTileManager.AddTiles());
        tiles.AddRange(SidewalkTileManager.AddTiles());
        tiles.AddRange(GameplayTileManager.AddTiles());
        for (int i = 0; i < tiles.Count; i++) {
            int col = tiles[i].Col;
            int row = tiles[i].Row;
            AllTiles[col + row * GridManager.Rows] = tiles[i];
        }
        for (int i = 0; i < AllTiles.Length; i++) {
            Tile t = AllTiles[i];
            GridManager.SetNeighbors(t);
            SetTileNeighbors(t.NeighborSystem);
        }
    }

    public HashSet<NeighborSystem> SetupTileNeighbors(Tile tile, bool original) {
        HashSet<NeighborSystem> tiles = SetTileNeighbors(tile.NeighborSystem);
        HashSet<NeighborSystem> newList = new();

        if (original == false) {
            return tiles;
        }

        foreach (NeighborSystem tileAutofit in tiles) {
            if (original == true) {
                newList.UnionWith(SetupTileNeighbors(tileAutofit.Tile, false));
            }
        }

        newList = newList.GroupBy(x => x.Tile.GridPosition).Select(something => something.First()).ToHashSet();

        foreach (NeighborSystem neighbor in newList) {
            if(neighbor.Tile is TileAutofit taf) {
                FitTile(taf);
            }
            else if(neighbor.Tile is TileRoad tr) {
                FitTile(tr);
            }
        }

        return newList;
    }

    private HashSet<NeighborSystem> SetTileNeighbors(NeighborSystem tile) {
        HashSet<NeighborSystem> neighbors = new HashSet<NeighborSystem> {
            tile
        };

        foreach (KeyValuePair<(Direction, Direction), Neighbor> pair in tile.GetAllNeighbors()) {
            if (pair.Value.Tile == null) {
                continue;
            }

            bool fittable = pair.Value.Tile.TileType == TileType.Road ? false : true;
            if (tile.Tile.TileType == TileType.Road) {
                fittable = !fittable;
            }

            tile.GetNeighbor(pair.Key).Fittable = fittable;

            neighbors.Add(tile.GetNeighborTile(pair.Key).NeighborSystem);
            tile.GetNeighborTile(pair.Key).NeighborSystem.GetNeighbor(TrafficUtilities.ReverseDirections(pair.Key)).Fittable = fittable;
        }

        return neighbors;
    }

    public void FitTile(TileRoad tile) {
        RoadUtils.FitTile(tile);
    }

    public void FitTile(TileAutofit tile) {
        int neighborcount = tile.NeighborSystem.NeighborCount();

        if (neighborcount == 4) {
            tile.SetFacing(Direction.Up);
            tile.SetAutofitType(AutofitType.Middle);
            return;
        }

        if (neighborcount == 3) {
            var direction = tile.NeighborSystem.GetFirstUnfittableDirection();
            tile.SetFacing(direction);
            tile.SetAutofitType(AutofitType.Side);
            return;
        }

        if (neighborcount == 2) {
            Direction direction = tile.NeighborSystem.GetCornerDirection();
            if (direction == Direction.None) {
                tile.SetFacing(tile.NeighborSystem.GetFirstUnfittableDirection());
                tile.SetAutofitType(AutofitType.Bridge);
                return;
            }
            tile.SetFacing(direction);
            tile.SetAutofitType(AutofitType.Corner);
            return;
        }

        if (neighborcount == 1) {
            tile.SetFacing(tile.NeighborSystem.GetFirstFittableDirection());
            tile.SetAutofitType(AutofitType.DeadEnd);
            return;
        }
    }
}
