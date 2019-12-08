using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Door : Sprite {
        public Texture2D doorTexture, steelWallTexture;
        public bool IsOpen = false;
        public Door(Vector2 position) : this(position.x, position.y) {}
        public Door(float x, float y) : base("data/tiles/door.png", true, false) {
            SetXY(x, y);
            doorTexture = Texture2D.GetInstance("data/tiles/door.png", true);
            steelWallTexture = Texture2D.GetInstance("data/tiles/steelWall.png", true);
            _texture = steelWallTexture;
        }

        public void SetTexture(Texture2D texture) {
            _texture = texture;
        }
    }
}