using System;
using Game.Utils;
using GXPEngine;

namespace Game {
    public class Dirt : Sprite {
        public Dirt(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Dirt(float x, float y, int color1, int color2) : base("data/tiles/dirt.png", true, false) {
            SetXY(x, y);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/dirt.png", color1, color2);
                texture.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/dirt.png");
                throw;
            }
        }
    }
}