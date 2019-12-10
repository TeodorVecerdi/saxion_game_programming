using GXPEngine;

namespace Game {
    public class Boulder : Sprite {
        public bool IsFalling = false;
        public bool UpdatedThisFrame = false;
        public Boulder(Vector2 position) : this(position.x, position.y) { }

        public Boulder(float x, float y) : base("data/tiles/boulder.png", true) {
            SetXY(x, y);
        }
    }
}