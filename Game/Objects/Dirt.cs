using System;
using System.Drawing;
using System.Drawing.Imaging;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Dirt : Sprite {
        public Dirt(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Dirt(float x, float y, int color1, int color2) : base("data/tiles/dirt.png", true, false) {
            SetXY(x, y);
            Bitmap source;
            try {
                source = new Bitmap("data/tiles/dirt.png");
                var target = Misc.ApplyLevelColor(source, color1, color2);
                texture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file \"data/tiles/dirt.png\"");
                throw e;
            }
        }
    }
}