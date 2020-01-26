using System;
using Game.Utils;
using GXPEngine;

namespace Game {
    public class Boulder : Sprite {
        public bool IsFalling = false;
        public bool UpdatedThisFrame = false;
        public Boulder(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Boulder(float x, float y, int color1, int color2) : base("data/tiles/boulder.png", true) {
            SetXY(x, y);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/boulder.png", color1, color2);
                texture.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/boulder.png");
                throw;
            }
        }
    }
}