using System;
using Game.Utils;
using GXPEngine;

namespace Game {
    public class SteelWall : Sprite {
        public SteelWall(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public SteelWall(float x, float y, int color1, int color2) : base("data/tiles/steelWall.png", true) {
            SetXY(x, y);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/steelWall.png", color1, color2);
                texture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file \"data/tiles/steelWall.png\"");
                throw e;
            }
        }
    }
}