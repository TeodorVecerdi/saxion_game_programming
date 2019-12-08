using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Diamond : Sprite{
        
        public Diamond(Vector2 position) : this(position.x, position.y) {}
        public Diamond(float x, float y) : base("data/tiles/diamond.png", true, false){
            SetXY(x, y);
        }

    }
}