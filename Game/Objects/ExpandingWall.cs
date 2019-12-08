using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class ExpandingWall : Sprite{
        public ExpandingWall(Vector2 position) : this(position.x, position.y) {}
        public ExpandingWall(float x, float y) :base("data/tiles/expandingWall.png", true, true) {
            SetXY(x, y);
        }
    }
}