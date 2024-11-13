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
    public RoadUtils RoadUtils { get; private set; } = null;

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
        RoadUtils = new RoadUtils(gridManager);
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
        tiles.AddRange(SidewalkTileManager.Tiles);
        tiles.AddRange(RoadTileManager.Tiles);
        tiles.AddRange(DecoTileManager.AllTiles);
        tiles.AddRange(GameplayTileManager.Tiles);

        return tiles;
    }

    public List<TileGameplay> GetAllGameplayTiles() {
        List<TileGameplay> tiles = new List<TileGameplay>();
        tiles.AddRange(SidewalkTileManager.Tiles);
        tiles.AddRange(RoadTileManager.Tiles);
        tiles.AddRange(GameplayTileManager.Tiles);

        return tiles;
    }

    public Tile[] GetAllActiveGameplayTiles() {
        return AllTiles;
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

    public List<NeighborSystem> SetupTileNeighbors(Tile tile, bool original) {
        List<NeighborSystem> tiles = SetTileNeighbors(tile.NeighborSystem);
        List<NeighborSystem> newList = new();

        if(tile.GridPosition == new Vector2Int(1, 0)) {
            Debug.Log("me");
        }

        if (original == false) {
            return tiles;
        }

        foreach (NeighborSystem tileAutofit in tiles) {
            if (original == true) {
                newList.AddRange(SetupTileNeighbors(tileAutofit.Tile, false));
            }
        }

        foreach (NeighborSystem tileAutofit in tiles) {
            newList.Remove(tileAutofit);
        }

        newList = newList.GroupBy(x => x.Tile.GridPosition).Select(y => y.First()).ToList();

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

    private List<NeighborSystem> SetTileNeighbors(NeighborSystem tile) {
        List<NeighborSystem> neighbors = new List<NeighborSystem> {
            tile
        };

        foreach (KeyValuePair<(Direction, Direction), Neighbor> pair in tile.Neighbors) {
            if (pair.Value.Tile == null) {
                continue;
            }

            bool fittable = pair.Value.Tile.TileType == TileType.Road ? false : true;
            if (tile.Tile.TileType == TileType.Road) {
                fittable = !fittable;
            }

            tile.Neighbors[pair.Key].Fittable = fittable;

            neighbors.Add(tile.Neighbors[pair.Key].Tile.NeighborSystem);
            tile.Neighbors[pair.Key].Tile.NeighborSystem.Neighbors[TrafficLib.ReverseDirections(pair.Key)].Fittable = fittable;
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
