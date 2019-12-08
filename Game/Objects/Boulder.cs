using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Boulder : Sprite {
        public bool UpdatedThisFrame = false;
        public bool IsFalling = false;
        public Boulder(Vector2 position) : this(position.x, position.y) {}
        public Boulder(float x, float y) : base("data/tiles/boulder.png", true, true){
            SetXY(x, y);
        }
    }
}