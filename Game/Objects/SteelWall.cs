using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class SteelWall : Sprite {
        public SteelWall(Vector2 position) : this(position.x, position.y) {}
        public SteelWall(float x, float y) : base("data/tiles/steelWall.png", true, true) {
            SetXY(x, y);
        }
    }
}