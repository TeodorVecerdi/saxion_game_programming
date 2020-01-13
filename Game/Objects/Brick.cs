using System;
using Game.Utils;
using GXPEngine;

namespace Game {
    public class Brick : Sprite {
        public Brick(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Brick(float x, float y, int color1, int color2) : base("data/tiles/brick.png", true) {
            SetXY(x, y);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/brick.png", color1, color2);
                texture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file \"data/tiles/brick.png\"");
                throw e;
            }
        }
    }
}