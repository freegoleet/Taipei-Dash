using System.Collections.Generic;
using UnityEngine;

namespace Traffic
{
    public class NeighborSystem
    {
        public Tile Tile { get; private set; }

        public NeighborSystem(Tile tile) {
            Tile = tile;    
        }

        public Dictionary<(Direction, Direction), Neighbor> Neighbors { get; private set; } = new() {
            { (Direction.Up, Direction.None), new Neighbor(new Vector2Int(0, 1), null) },
            { (Direction.Down, Direction.None), new Neighbor(new Vector2Int(0, -1), null) },
            { (Direction.Left, Direction.None), new Neighbor(new Vector2Int(-1, 0), null) },
            { (Direction.Right, Direction.None), new Neighbor(new Vector2Int(1, 0), null) },
            { (Direction.Up, Direction.Left), new Neighbor(new Vector2Int(-1, 1), null) },
            { (Direction.Up, Direction.Right), new Neighbor(new Vector2Int(1, 1), null) },
            { (Direction.Down, Direction.Left), new Neighbor(new Vector2Int(-1, -1), null) },
            { (Direction.Down, Direction.Right), new Neighbor(new Vector2Int(1, -1), null) },
        };

        public List<Vector2Int> GetAllNeighbors() {
            List<Vector2Int> list = new();
            foreach (Neighbor neighbor in Neighbors.Values) {
                if (neighbor.Tile == null) {
                    continue;
                }
                list.Add(neighbor.RelativeGridPos);
            }
            return list;
        }

        public List<Vector2Int> GetAllFittableNeighbors() {
            List<Vector2Int> list = new();
            foreach (Neighbor neighbor in Neighbors.Values) {
                if (neighbor.Fittable == false) {
                    continue;
                }
                list.Add(neighbor.RelativeGridPos);
            }
            return list;
        }

        public List<Vector2Int> GetAllUnfittableNeighbors() {
            List<Vector2Int> list = new();
            foreach (Neighbor neighbor in Neighbors.Values) {
                if (neighbor.Fittable == true) {
                    continue;
                }
                list.Add(neighbor.RelativeGridPos);
            }
            return list;
        }

        public Direction GetFirstUnfittableDirection() {
            foreach (KeyValuePair<(Direction, Direction), Neighbor> kvp in Neighbors) {
                if (kvp.Key.Item2 != Direction.None) {
                    continue;
                }
                if (kvp.Value.Fittable == false) {
                    return kvp.Key.Item1;
                }
            }
            return default;
        }

        public Direction GetFirstFittableDirection() {
            foreach (KeyValuePair<(Direction, Direction), Neighbor> kvp in Neighbors) {
                if (kvp.Key.Item2 != Direction.None) {
                    continue;
                }
                if (kvp.Value.Fittable == true) {
                    return TrafficLib.ReverseDirections(kvp.Key).Item1;
                }
            }
            return default;
        }

        public List<Direction> GetAllUnfittableAdjacentDirections() {
            List<Direction> directions = new();
            foreach (KeyValuePair<(Direction, Direction), Neighbor> kvp in Neighbors) {
                if (kvp.Key.Item2 != Direction.None) {
                    continue;
                }
                if (kvp.Value.Fittable == false) {
                    directions.Add(kvp.Key.Item1);
                }
            }
            return directions;
        }

        public List<Direction> GetAllFittableAdjacentDirections() {
            List<Direction> directions = new();
            foreach (KeyValuePair<(Direction, Direction), Neighbor> kvp in Neighbors) {
                if (kvp.Key.Item2 != Direction.None) {
                    continue;
                }
                if (kvp.Value.Fittable == true) {
                    directions.Add(kvp.Key.Item1);
                }
            }
            return directions;
        }

        public Direction GetCornerDirection() {
            (Direction, Direction) emptyDirs = (Direction.None, Direction.None);
            foreach (Direction dir in GetAllUnfittableAdjacentDirections()) {
                if (emptyDirs.Item1 == Direction.None) {
                    emptyDirs.Item1 = dir;
                    continue;
                }
                emptyDirs.Item2 = dir;
                break;
            }
            switch (emptyDirs) {
                case (Direction.Up, Direction.Left):
                    return Direction.Left;
                case (Direction.Up, Direction.Right):
                    return Direction.Up;
                case (Direction.Down, Direction.Left):
                    return Direction.Down;
                case (Direction.Down, Direction.Right):
                    return Direction.Right;
                default:
                    break;
            }
            return Direction.None;
        }

        public (Direction, Direction) GetFirstEmptyDiagonalDirection() {
            foreach (KeyValuePair<(Direction, Direction), Neighbor> kvp in Neighbors) {
                if (kvp.Key.Item2 == Direction.None) {
                    continue;
                }
                return kvp.Key;
            }
            return default;
        }

        public List<(Direction, Direction)> GetAllUnfittableDiagonalDirections() {
            List<(Direction, Direction)> emptyDiagonals = new();
            foreach (KeyValuePair<(Direction, Direction), Neighbor> kvp in Neighbors) {
                if (kvp.Key.Item2 == Direction.None) {
                    continue;
                }
                if (kvp.Value.Fittable == true) {
                    continue;
                }
                emptyDiagonals.Add(kvp.Key);
            }

            return emptyDiagonals;
        }

        public List<(Direction, Direction)> GetAllFittableDiagonalDirections() {
            List<(Direction, Direction)> fittableDiagonals = new();
            foreach (KeyValuePair<(Direction, Direction), Neighbor> item in Neighbors) {
                if (item.Key.Item2 == Direction.None) {
                    continue;
                }
                if (item.Value.Fittable == false) {
                    continue;
                }
                fittableDiagonals.Add(item.Key);
            }
            return fittableDiagonals;
        }

        public int NeighborCount() {
            int count = 0;
            foreach (KeyValuePair<(Direction, Direction), Neighbor> kvp in Neighbors) {
                if (kvp.Key.Item2 != Direction.None) {
                    continue;
                }
                if (kvp.Value.Fittable == false) {
                    continue;
                }
                count++;
            }
            return count;
        }
    }

    public class Neighbor
    {
        public Vector2Int RelativeGridPos { get; set; } = new Vector2Int();
        public Tile Tile { get; set; } = null;
        public bool Fittable { get; set; } = false;

        public Neighbor(Vector2Int relativeGridPos, Tile tile) {
            RelativeGridPos = relativeGridPos;
            Tile = tile;
        }
    }
}