using System;
using System.Collections.Generic;
using Traffic;
using UnityEngine;

public static class Pathfinder
{
    private static bool m_StepThrough = false;
    private static List<Tile> m_AStarvalues = new();

    public struct PathfindData
    {
        public TileGameplay StartTile;
        public TileGameplay TargetTile;
        public SO_Entity.ePathfindType PathfindType;
        public List<Tile> ProcessedTiles;
        public List<TileGameplay> ToSearch;
    }

    public static void ToggleStepThrough(bool active) {
        m_StepThrough = active;
    }

    public static List<Tile> GetAStarTiles() {
        return m_AStarvalues;
    }

    public static void ClearAStarTiles() {
        m_AStarvalues.Clear();
    }

    public static List<TileGameplay> Automatic(TileGameplay startTile, TileGameplay targetNode, SO_Entity.ePathfindType pathfindType, GridManager gridManager) {

        if (startTile == null) {
            return null;
        }

        if (targetNode == null) {
            return null;
        }

        startTile.SetH(int.MaxValue);

        int repeats = 100;

        List<TileGameplay> path = null;

        PathfindData data = new PathfindData();
        data.StartTile = startTile;
        data.TargetTile = targetNode;
        data.PathfindType = pathfindType;
        data.ProcessedTiles = new();
        data.ToSearch = new() { data.StartTile };

        while (path == null && repeats > 0) {
            path = FindPath(data, gridManager);
            repeats--;
        }

        foreach (Tile tile in GetAStarTiles()) {
            tile.SetG(0);
            tile.SetH(0);
        }

        return path;
    }

    private static List<TileGameplay> FindPath(PathfindData data, GridManager gridManager) {
        Vector2 lastDirection = Vector2.zero;

        if (data.ToSearch.Count == 0) {
            return null;
        }

        TileGameplay currentNode = data.ToSearch[0];

        foreach (TileGameplay node in data.ToSearch) {
            if (m_StepThrough == true) {
                if (node != data.StartTile) {

                }

                node.ToggleValue(true);
                m_AStarvalues.Add(node);
            }

            float directionVariationCoef = 0f;
            var direction = gridManager.GetDirection(currentNode, node);

            if (direction == lastDirection) {
                directionVariationCoef = 0.1f;
            }

            if (node.F + directionVariationCoef < currentNode.F || node.F == currentNode.F && node.H < currentNode.H) {
                currentNode = node;
            }
        }

        data.ProcessedTiles.Add(currentNode);
        data.ToSearch.Remove(currentNode);

        if (currentNode == data.TargetTile) {
            TileGameplay currentPathTile = data.TargetTile;
            List<TileGameplay> path = new();
            int count = 100;

            while (currentPathTile != data.StartTile) {
                path.Add(currentPathTile);
                currentPathTile = (TileGameplay)currentPathTile.Connection;
                count--;

                if (count < 0) {
                    throw new Exception();
                }
            }

            lastDirection = Vector2.zero;
            data.ToSearch.Clear();
            data.ProcessedTiles.Clear();

            return path;
        }

        HashSet<Tile> neighbors = new();
        if (data.PathfindType == SO_Entity.ePathfindType.Car) {
            if (currentNode is TileRoad road == true) {
                for (int i = 0; i < road.Connections.OutConnections.Length; i++) {
                    if (road.Connections.OutConnections[i] == false) {
                        continue;
                    }
                    neighbors.Add(road.NeighborSystem.GetNeighborTile(((Direction)i, Direction.None)));
                }
            }
        }
        else {
            foreach (Tile tile in currentNode.NeighborSystem.GetAllAdjacentNeighbors()) {
                if (tile is TileRoad road == false) {
                    neighbors.Add(tile);
                    continue;
                }
                if (road.HasCrosswalk == true) {
                    neighbors.Add(road);
                }
            }
        }

        foreach (TileGameplay neighbor in neighbors) {
            if (neighbor == null) {
                continue;
            }
            //if (neighbor.GpData.Traversable.Equals(SO_TileGameplay.eTraversable.Untraversable))
            //{
            //    continue;
            //}

            if (data.ProcessedTiles.Contains(neighbor) == true) {
                continue;
            }

            bool inSearch = data.ToSearch.Contains(neighbor);

            float costToNeighbor = currentNode.G;

            if (inSearch == false || costToNeighbor < neighbor.G) {
                neighbor.SetG(costToNeighbor);
                neighbor.SetConnection(currentNode);

                if (inSearch == false) {
                    neighbor.SetH(gridManager.GetDistance(neighbor, data.TargetTile));
                    data.ToSearch.Add(neighbor);
                }
            }
        }

        return null;
    }
}
