using System;
using Game.Utils;
using GXPEngine;

namespace Game {
    public class Amoeba : Sprite {
        public bool UpdatedThisFrame = false;
        public Amoeba(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Amoeba(float x, float y, int color1, int color2) : base("data/tiles/amoeba.png", true, false) {
            SetXY(x, y);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/amoeba.png", color1, color2);
                texture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file \"data/tiles/amoeba.png\"");
                throw e;
            }
        }
    }
}