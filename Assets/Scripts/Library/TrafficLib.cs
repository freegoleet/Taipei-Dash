using System;
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
        Up, Right, Down, Left, None
    }

    public enum TrafficLightColor
    {
        Red,
        Yellow,
        Green
    }

    public static class TrafficLib
    {
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


        /// <summary>
        /// Get the next direction by rotating clockwise by 90 degrees. Up > Right > Down > Left.
        /// </summary>
        /// <param name="currentDirection"></param>
        /// <returns></returns>
        public static Direction GetNextDirection(Direction currentDirection) {
            int index = (int)currentDirection + 1;
            if (index >= Enum.GetValues(typeof(Direction)).Length) {
                index = 0;
            }

            return (Direction)index;
        }

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
