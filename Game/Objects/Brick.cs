using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Brick : Sprite {
        
        public Brick(Vector2 position) : this(position.x, position.y) {}
        public Brick(float x, float y) : base("data/tiles/brick.png", true, true) {
            SetXY(x, y);
        }

    }
}