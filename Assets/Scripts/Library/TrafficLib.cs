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

    public enum Directions
    {
        Up, Right, Down, Left
    }

    public enum TrafficLightColor
    {
        Red,
        Yellow,
        Green
    }

    public static class TrafficLib
    {
        public static Directions GetNextDirection(Directions currentDirection) {
            int index = (int)currentDirection + 1;
            if (index >= Enum.GetValues(typeof(Directions)).Length) {
                index = 0;
            }

            return (Directions)index;
        }

        public static Texture2D AddWatermark(Texture2D background, Texture2D watermark, int startX, int startY) {
            Texture2D newTex = new Texture2D(background.width, background.height, background.format, false);
            for (int x = 0; x < background.width; x++) {
                for (int y = 0; y < background.height; y++) {
                    if (x >= startX && y >= startY && x < watermark.width && y < watermark.height) {
                        Color bgColor = background.GetPixel(x, y);
                        Color wmColor = watermark.GetPixel(x - startX, y - startY);

                        Color final_color = Color.Lerp(bgColor, wmColor, wmColor.a / 1.0f);

                        newTex.SetPixel(x, y, final_color);
                    }
                    else
                        newTex.SetPixel(x, y, background.GetPixel(x, y));
                }
            }

            newTex.Apply();
            return newTex;
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
