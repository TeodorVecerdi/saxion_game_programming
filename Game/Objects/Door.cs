using System;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Door : Sprite {
        private readonly Texture2D doorTexture;
        private readonly Texture2D steelWallTexture;
        private bool doorTextureShown;
        public bool IsOpen = false;
        public Door(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Door(float x, float y, int color1, int color2) : base("data/tiles/door.png", true, false) {
            SetXY(x, y);
            doorTexture = Texture2D.GetInstance("data/tiles/door.png", true);
            steelWallTexture = Texture2D.GetInstance("data/tiles/steelWall.png", true);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/door.png", color1, color2);
                doorTexture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file data/tiles/door.png");
                throw e;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/steelWall.png", color1, color2);
                steelWallTexture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file data/tiles/steelWall.png");
                throw e;
            }

            _texture = steelWallTexture;
        }

        public void Flash() {
            doorTextureShown = !doorTextureShown;
            if (doorTextureShown) _texture = doorTexture;
            else _texture = steelWallTexture;
        }
    }
}