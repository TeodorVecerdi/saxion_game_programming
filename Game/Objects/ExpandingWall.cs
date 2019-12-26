using System;
using System.Drawing;
using Game.Utils;
using GXPEngine;

namespace Game {
    public class ExpandingWall : Sprite {
        public ExpandingWall(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public ExpandingWall(float x, float y, int color1, int color2) : base("data/tiles/expandingWall.png", true) {
            SetXY(x, y);
            Bitmap source;
            try {
                source = new Bitmap("data/tiles/expandingWall.png");
                var target = Misc.ApplyLevelColor(source, color1, color2);
                texture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file \"data/tiles/expandingWall.png\"");
                throw e;
            }
        }
    }
}