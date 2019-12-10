using GXPEngine;

namespace Game {
    public class Butterfly : Sprite {
        public Butterfly(Vector2 position) : this(position.x, position.y) { }

        public Butterfly(float x, float y) : base("data/tiles/butterfly.png", true, false) {
            SetXY(x, y);
        }
    }
}