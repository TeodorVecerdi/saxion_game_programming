using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Dirt : Sprite {
        
        public Dirt(Vector2 position) : this(position.x, position.y) {}
        public Dirt(float x, float y) : base("data/tiles/dirt.png", true, false){
            SetXY(x, y);
        }
    }
}