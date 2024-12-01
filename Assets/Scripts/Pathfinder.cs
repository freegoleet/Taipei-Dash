using System;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    private static bool m_StepThrough = false;
    private static List<TileGameplay> m_ToSearch = null;
    private static List<Tile> m_Processed = null;
    private static Vector2 m_LastDirection = Vector2.zero;
    private static List<Tile> m_AStarvalues = new();

    public static void ToggleStepThrough(bool active) {
        m_StepThrough = active;
    }

    public static List<Tile> GetAStarTiles() {
        return m_AStarvalues;
    }

    public static void ClearAStarTiles() {
        m_AStarvalues.Clear();
    }

    //public static HashSet<TileGameplay> StepThrough(TileGameplay startNode, TileGameplay targetNode, GridManager gridManager) {
    //    if (startNode == null) {
    //        return null;
    //    }

    //    if (targetNode == null) {
    //        return null;
    //    }

    //    if (m_ToSearch == null) {
    //        m_ToSearch = new() { startNode };
    //    }
    //    else if (m_ToSearch.Count == 0) {
    //        m_ToSearch.Add(startNode);
    //    }

    //    if (m_Processed == null) {
    //        m_Processed = new();
    //    }

    //    HashSet<TileGameplay> path = null;
    //    while (path == null) {
    //        path = FindPath(startNode, targetNode, gridManager);
    //    }

    //    return path;
    //}

    public static HashSet<TileGameplay> Automatic(TileGameplay startNode, TileGameplay targetNode, SO_Entity.ePathfindType pathfindType, GridManager gridManager) {

        if (startNode == null) {
            return null;
        }

        if (targetNode == null) {
            return null;
        }

        m_ToSearch = new() { startNode };

        startNode.SetH(int.MaxValue);
        m_Processed = new();

        int repeats = 100;

        HashSet<TileGameplay> path = null;
        while (path == null && repeats > 0) {
            path = FindPath(startNode, targetNode, gridManager);
            repeats--;
        }

        foreach (Tile tile in GetAStarTiles()) {
            tile.SetG(0);
            tile.SetH(0);
        }

        return path;
    }

    private static HashSet<TileGameplay> FindPath(TileGameplay startNode, TileGameplay targetNode, GridManager gridManager) {
        if (m_ToSearch.Count == 0) {
            return null;
        }

        TileGameplay currentNode = m_ToSearch[0];

        foreach (TileGameplay node in m_ToSearch) {
            if (m_StepThrough == true) {
                if (node != startNode) {

                }

                node.ToggleValue(true);
                m_AStarvalues.Add(node);
            }

            float directionVariationCoef = 0f;
            var direction = gridManager.GetDirection(currentNode, node);

            if (direction == m_LastDirection) {
                directionVariationCoef = 0.1f;
            }

            if (node.F + directionVariationCoef < currentNode.F || node.F == currentNode.F && node.H < currentNode.H) {
                currentNode = node;
            }
        }

        m_LastDirection = gridManager.GetDirection(m_ToSearch[0], currentNode);

        m_Processed.Add(currentNode);
        m_ToSearch.Remove(currentNode);

        if (currentNode == targetNode) {
            TileGameplay currentPathTile = targetNode;
            HashSet<TileGameplay> path = new();
            int count = 100;

            while (currentPathTile != startNode) {
                path.Add(currentPathTile);
                currentPathTile = (TileGameplay)currentPathTile.Connection;
                count--;

                if (count < 0) {
                    throw new Exception();
                }
            }

            m_LastDirection = Vector2.zero;
            m_ToSearch.Clear();
            m_Processed.Clear();

            return path;
        }

        HashSet<Tile> neighbors = new();
        if (currentNode is TileRoad tr) {
            foreach (var kvp in tr.OutConnections) {
                if (kvp.Value == false) {
                    continue;
                }
                neighbors.Add(tr.NeighborSystem.Neighbors[(kvp.Key, Traffic.Direction.None)].Tile);
            }
        }
        else {
            neighbors = currentNode.NeighborSystem.GetAllFittableAdjacentNeighbors();
        }

        foreach (TileGameplay neighbor in neighbors) {
            if (neighbor == null) {
                continue;
            }
            //if (neighbor.GpData.Traversable.Equals(SO_TileGameplay.eTraversable.Untraversable))
            //{
            //    continue;
            //}

            if (m_Processed.Contains(neighbor) == true) {
                continue;
            }

            bool inSearch = m_ToSearch.Contains(neighbor);

            float costToNeighbor = currentNode.G;

            if (inSearch == false || costToNeighbor < neighbor.G) {
                neighbor.SetG(costToNeighbor);
                neighbor.SetConnection(currentNode);

                if (inSearch == false) {
                    neighbor.SetH(gridManager.GetDistance(neighbor, targetNode));
                    m_ToSearch.Add(neighbor);
                }
            }
        }

        return null;
    }
}
