using System.Collections.Generic;

namespace Game.Utils {
    public class TileDefinition {
        public int Code;
        public bool Rounded;
        public bool Explodable;
        public bool Consumable;
        public Dictionary<string, SpriteDef> Sprite = new Dictionary<string, SpriteDef>();
        public SpriteDef MainSprite => Sprite["main"];

        public TileDefinition(int code, bool rounded, bool explodable, bool consumable) {
            Code = code;
            Rounded = rounded;
            Explodable = explodable;
            Consumable = consumable;
        }

        public TileDefinition AddSprite(SpriteDef sprite, string spriteType = "main") {
            Sprite.Add(spriteType, sprite);
            return this;
        }
    }

    public class SpriteDef {
        public int X;
        public int Y;
        public int Frames;
        public int FramesPerSecond;

        public SpriteDef(int x, int y, int frames=1, int framesPerSecond=1) {
            X = x;
            Y = y;
            Frames = frames;
            FramesPerSecond = framesPerSecond;
        }
    }
}