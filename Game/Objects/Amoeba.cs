using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Amoeba : Sprite {
        public Amoeba(Vector2 position) : this(position.x, position.y) {}
        public Amoeba(float x, float y) : base("data/tiles/amoeba.png", true, false) {
            SetXY(x, y);
        }
    }
}