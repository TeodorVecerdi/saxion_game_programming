using GXPEngine;

namespace Game {
    public class MagicWall : Sprite {
        public MagicWall(Vector2 position) : this(position.x, position.y) { }

        public MagicWall(float x, float y) : base("data/tiles/magicWall.png", true) {
            SetXY(x, y);
        }
    }
}