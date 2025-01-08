using NUnit.Framework;
using UnityEngine;

namespace Traffic
{
    public enum Occupant
    {
        None,
        Car,
        Player
    }

    public enum Direction
    {
        Up, Down, Left, Right, None
    }

    public enum TrafficLightColor
    {
        Red,
        Yellow,
        Green
    }

    public enum TileType
    {
        Gameplay,
        Road,
        Autofit,
        Deco,
        None
    }

    public enum TileModType
    {
        Placement,
        Connections,
        Crosswalk,
        Stopline,
        TrafficLight,
        Pointer
    }

    public enum ToggleType
    {
        Add,
        Remove,
        Toggle
    }

    public static class TrafficUtilities
    {
        public static Vector2Int GetVector2IntPosFromIntPos(int pos, int cols) {
            int row = pos / cols;
            int col = pos % cols;
            return new Vector2Int(row, col);
        }

        public static int GetIntPosFromVector2IntPos(Vector2Int pos, int cols) {
            // x = 7, y = 6, cols = 10
            int result = pos.x * cols;
            result += pos.y;
            return result;
        }

        public static Direction GetDirectionFromPosToPos(Vector2Int from, Vector2Int to) {
            if (from.x < to.x) {
                return Direction.Right;
            }
            if (from.x > to.x) {
                return Direction.Left;
            }
            if(from.y > to.y) {
                return Direction.Down;
            }
            if (from.y < to.y) {
                return Direction.Up;
            }
            return Direction.None;
        }

        /// <summary>
        /// Reverse the direction of the input Direction.
        /// </summary>
        /// <param name="directions">Direction</param>
        /// <returns></returns>
        public static Direction ReverseDirections(Direction direction) {
            return ReverseDirections((direction, Direction.None)).Item1;
        }

        /// <summary>
        /// Reverse the directions of both input Directions.
        /// </summary>
        /// <param name="directions">(Direction, Direction)</param>
        /// <returns></returns>
        public static (Direction, Direction) ReverseDirections((Direction, Direction) directions) {
            (Direction, Direction) reversedDirections = directions;

            for (int i = 0; i < 2; i++) {
                Direction direction = Direction.None;
                if (i == 0) {
                    direction = directions.Item1;
                }
                else {
                    direction = directions.Item2;
                }
                switch (direction) {
                    case Direction.Up:
                        direction = Direction.Down;
                        break;
                    case Direction.Down:
                        direction = Direction.Up;
                        break;
                    case Direction.Right:
                        direction = Direction.Left;
                        break;
                    case Direction.Left:
                        direction = Direction.Right;
                        break;
                    case Direction.None:
                        direction = Direction.None;
                        break;
                }
                if (i == 0) {
                    reversedDirections.Item1 = direction;
                }
                else {
                    reversedDirections.Item2 = direction;
                }
            }

            return reversedDirections;
        }

        public static Direction[] GetFlankDirections(Direction direction) {
            switch (direction) {
                case Direction.Up:
                    return new Direction[] { Direction.Left, Direction.Right };
                case Direction.Right:
                    return new Direction[] { Direction.Up, Direction.Down };
                case Direction.Down:
                    return new Direction[] { Direction.Right, Direction.Left };
                case Direction.Left:
                    return new Direction[] { Direction.Down, Direction.Up };
            }
            return null;
        }


        /// <summary>
        /// Get the next direction by rotating clockwise by 90 degrees. Up > Right > Down > Left.
        /// </summary>
        /// <param name="currentDirection"></param>
        /// <returns></returns>
        public static Direction GetNextDirection(Direction currentDirection) {
            switch (currentDirection) {
                case Direction.Up:
                    return Direction.Right;
                case Direction.Down:
                    return Direction.Left;
                case Direction.Left:
                    return Direction.Up;
                case Direction.Right:
                    return Direction.Down;
            }
            return Direction.None;
        }

        public static Direction NormalizeRotation(Direction from, Direction to) {
            if (from == to) {
                return Direction.None;
            }
            if (ReverseDirections(from) == to) {
                return Direction.Up;
            }

            int index = from - to;
            switch (index) {
                case 1:
                    return Direction.Right;
                case -1:
                    return Direction.Left;
                case 2:
                    return Direction.Left;
                case -2:
                    return Direction.Right;
                case 3:
                    return Direction.Right;
                case -3:
                    return Direction.Left;
            }

            return Direction.None;
        }

        public static Direction RelativeRotation(Direction from, Direction to) {
            if (from == ReverseDirections(to)) {
                return Direction.Down;
            }
            if (from == to) {
                return Direction.Up;
            }

            switch (from) {
                case Direction.Up:
                    if (to == Direction.Left) {
                        return Direction.Left;
                    }
                    return Direction.Right;
                case Direction.Right:
                    if (to == Direction.Up) {
                        return Direction.Left;
                    }
                    return Direction.Right;
                case Direction.Down:
                    if (to == Direction.Left) {
                        return Direction.Right;
                    }
                    return Direction.Left;
                case Direction.Left:
                    if (to == Direction.Up) {
                        return Direction.Right;
                    }
                    return Direction.Left;
            }

            return Direction.None;
        }

        public static bool IsDirectionAdjacent(Direction start, Direction target) {
            int index = start - target;
            switch (index) {
                case 1:
                    return true;
                case -1:
                    return true;
                case 3:
                    return true;
                case -3:
                    return true;
            }
            return false;
        }

        public static Direction[] GetAdjacentDirections(Direction start, Direction target) {
            Direction[] directions = new Direction[2];
            int arrayIndex = 0;
            int enumIndex = start - target;
            switch (enumIndex) {
                case 0:
                    directions[arrayIndex] = start;
                    arrayIndex++;
                    break;
                case 1:
                    directions[arrayIndex] = Direction.Right;
                    arrayIndex++;
                    break;
                case -1:
                    directions[arrayIndex] = Direction.Left;
                    arrayIndex++;
                    break;
                case 3:
                    directions[arrayIndex] = Direction.Left;
                    arrayIndex++;
                    break;
                case -3:
                    directions[arrayIndex] = Direction.Right;
                    arrayIndex++;
                    break;
            }
            return directions;
        }

        public static Vector3 GetDirectionAsQuaternion(Direction direction) {
            float angle = 0;
            switch (direction) {
                case Direction.Up:
                    angle = 0;
                    break;
                case Direction.Down:
                    angle = 180;
                    break;
                case Direction.Left:
                    angle = 90;
                    break;
                case Direction.Right:
                    angle = 270;
                    break;
            }
            return new Vector3(0, 0, angle);
        }

        //public static List<Vector2> GetPointsAlongCurve(Vector2Int start, Vector2Int end, int points) {

        //}


        public static Texture2D AddWatermark(Texture2D background, Texture2D watermark, int startPositionX, int startPositionY) {
            //only read and rewrite the area of the watermark
            for (int x = startPositionX; x < background.width; x++) {
                for (int y = startPositionY; y < background.height; y++) {
                    if (x - startPositionX < watermark.width && y - startPositionY < watermark.height) {
                        var bgColor = background.GetPixel(x, y);
                        var wmColor = watermark.GetPixel(x - startPositionX, y - startPositionY);

                        var finalColor = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                        background.SetPixel(x, y, finalColor);
                    }
                }
            }

            background.Apply();
            return background;
        }

        public static void RotateImage(Texture2D tex, float angleDegrees) {
            int width = tex.width;
            int height = tex.height;
            float halfHeight = height * 0.5f;
            float halfWidth = width * 0.5f;

            var texels = tex.GetRawTextureData<Color32>();
            var copy = System.Buffers.ArrayPool<Color32>.Shared.Rent(texels.Length);
            Unity.Collections.NativeArray<Color32>.Copy(texels, copy, texels.Length);

            float phi = Mathf.Deg2Rad * angleDegrees;
            float cosPhi = Mathf.Cos(phi);
            float sinPhi = Mathf.Sin(phi);

            int address = 0;
            for (int newY = 0; newY < height; newY++) {
                for (int newX = 0; newX < width; newX++) {
                    float cX = newX - halfWidth;
                    float cY = newY - halfHeight;
                    int oldX = Mathf.RoundToInt(cosPhi * cX + sinPhi * cY + halfWidth);
                    int oldY = Mathf.RoundToInt(-sinPhi * cX + cosPhi * cY + halfHeight);
                    bool InsideImageBounds = oldX > -1 & oldX < width
                                           & oldY > -1 & oldY < height;

                    texels[address++] = InsideImageBounds ? copy[oldY * width + oldX] : default;
                }
            }

            // No need to reinitialize or SetPixels - data is already in-place.
            tex.Apply(true);

            System.Buffers.ArrayPool<Color32>.Shared.Return(copy);
        }

    }
}
