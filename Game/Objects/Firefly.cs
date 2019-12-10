using GXPEngine;

namespace Game {
    public class Firefly : Sprite {
        public Firefly(Vector2 position) : this(position.x, position.y) { }

        public Firefly(float x, float y) : base("data/tiles/firefly.png", true, false) {
            SetXY(x, y);
        }
    }
}