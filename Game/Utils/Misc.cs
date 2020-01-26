using System.Drawing;
using System.Drawing.Imaging;
using GXPEngine;

namespace Game {
    public static class Misc {
        /// <summary>
        ///     Applies the current level color to a Bitmap to be later used as a Texture2D
        ///     Adapted from https://stackoverflow.com/a/17208320 by Adriano Repetti
        /// </summary>
        public static unsafe Bitmap ApplyLevelColor(string sourcePath, int color1, int color2) {
            if (BitmapCache.Cache.ContainsKey(sourcePath))
                return BitmapCache.GetBitmap(sourcePath);

            var source = new Bitmap(sourcePath);
            var col1 = Color.FromArgb(0xff, (color1 >> 16) & 0xff, (color1 >> 8) & 0xff, color1 & 0xff);
            var col2 = Color.FromArgb(0xff, (color2 >> 16) & 0xff, (color2 >> 8) & 0xff, color2 & 0xff);

            const int pixelSize = 4;
            var target = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
            BitmapData sourceData = null, targetData = null;

            try {
                sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                targetData = target.LockBits(new Rectangle(0, 0, target.Width, target.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                for (var y = 0; y < source.Height; y++) {
                    var sourceRow = (byte*) sourceData.Scan0 + y * sourceData.Stride;
                    var targetRow = (byte*) targetData.Scan0 + y * targetData.Stride;
                    for (var x = 0; x < source.Width; x++) {
                        var b = sourceRow[x * pixelSize + 0];
                        var g = sourceRow[x * pixelSize + 1];
                        var r = sourceRow[x * pixelSize + 2];
                        var a = sourceRow[x * pixelSize + 3];
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

            BitmapCache.AddToCache(sourcePath, target);
            return target;
        }

        #region Nested type: FollowWall
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
        #endregion
    }
}