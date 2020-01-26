using System;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Door : Sprite {
        public bool IsOpen = false;
        private readonly Texture2D doorTexture;
        private readonly Texture2D steelWallTexture;
        private bool doorTextureShown;
        public Door(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Door(float x, float y, int color1, int color2) : base("data/tiles/door.png", true, false) {
            SetXY(x, y);
            doorTexture = Texture2D.GetInstance("data/tiles/door.png", true);
            steelWallTexture = Texture2D.GetInstance("data/tiles/steelWall.png", true);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/door.png", color1, color2);
                doorTexture.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/door.png");
                throw;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/steelWall.png", color1, color2);
                steelWallTexture.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/steelWall.png");
                throw;
            }

            _texture = steelWallTexture;
        }

        public void Flash() {
            doorTextureShown = !doorTextureShown;
            _texture = doorTextureShown ? doorTexture : steelWallTexture;
        }
    }
}