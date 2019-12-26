using System.Drawing;
using System.Drawing.Imaging;
using GXPEngine;

namespace Game.Utils {
    public static class Misc {
        /// <summary>
        /// Applies the current level color to a Bitmap to be later used as a Texture2D
        /// Adapted from https://stackoverflow.com/a/17208320 by Adriano Repetti
        /// </summary>
        public static unsafe Bitmap ApplyLevelColor(Bitmap source, int color1, int color2) {
            Color col1 = Color.FromArgb(0xff, (color1 >> 16) & 0xff, (color1 >> 8) & 0xff, (color1) & 0xff);
            Color col2 = Color.FromArgb(0xff, (color2 >> 16) & 0xff, (color2 >> 8) & 0xff, (color2) & 0xff);
            //a52a00 (165, 42, 0) -> color1
            //3f3f3f (63, 63, 63) -> color2
            const int pixelSize = 4;
            Bitmap target = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            BitmapData sourceData = null, targetData = null;

            try {
                sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                targetData = target.LockBits(new Rectangle(0, 0, target.Width, target.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                for (int y = 0; y < source.Height; y++) {
                    byte* sourceRow = (byte*) sourceData.Scan0 + y * sourceData.Stride;
                    byte* targetRow = (byte*) targetData.Scan0 + y * targetData.Stride;
                    for (int x = 0; x < source.Width; x++) {
                        byte b = sourceRow[x * pixelSize + 0];
                        byte g = sourceRow[x * pixelSize + 1];
                        byte r = sourceRow[x * pixelSize + 2];
                        byte a = sourceRow[x * pixelSize + 3];
                        if (r == 63 && g == 63 && b == 63) {
                            r = col2.R;
                            g = col2.G;
                            b = col2.B;
                        } else if (r == 165 && g == 42 && b == 0) {
                            r = col1.R;
                            g = col1.G;
                            b = col1.B;
                        }

                        targetRow[x * pixelSize + 0] = b;
                        targetRow[x * pixelSize + 1] = g;
                        targetRow[x * pixelSize + 2] = r;
                        targetRow[x * pixelSize + 3] = a;
                    }
                }
            } finally {
                if (sourceData != null) source.UnlockBits(sourceData);
                if (targetData != null) target.UnlockBits(targetData);
            }

            return target;
        }

        public static class FollowWall {
            public static Vector2Int Vec2IntRotate90(Vector2Int vector) {
                if (vector.x == 0 && vector.y == -1) return Vector2Int.From(-1, 0);
                if (vector.x == 0 && vector.y == 1) return Vector2Int.From(1, 0);
                if (vector.x == -1 && vector.y == 0) return Vector2Int.From(0, 1);
                if (vector.x == 1 && vector.y == 0) return Vector2Int.From(0, -1);
                return Vector2Int.left;
            }
            public static Vector2Int Vec2IntRotate180(Vector2Int vector) {
                return Vector2Int.From(-vector.x, -vector.y);
            }
            public static Vector2Int Vec2IntRotate270(Vector2Int vector) {
                return Vec2IntRotate90(Vec2IntRotate180(vector));
            }
        }
    }
}