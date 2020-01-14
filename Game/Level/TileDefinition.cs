using System.Collections.Generic;

namespace Game.Utils {
    public class TileDefinition {
        public int Code;
        public bool Consumable;
        public bool Explodable;
        public bool Rounded;
        public Dictionary<string, SpriteDef> Sprite = new Dictionary<string, SpriteDef>();

        public TileDefinition(int code, bool rounded, bool explodable, bool consumable) {
            Code = code;
            Rounded = rounded;
            Explodable = explodable;
            Consumable = consumable;
        }

        public SpriteDef MainSprite => Sprite["main"];

        public TileDefinition AddSprite(SpriteDef sprite, string spriteType = "main") {
            Sprite.Add(spriteType, sprite);
            return this;
        }
    }

    public class SpriteDef {
        public int Frames;
        public int FramesPerSecond;
        public int X;
        public int Y;

        public SpriteDef(int x, int y, int frames = 1, int framesPerSecond = 1) {
            X = x;
            Y = y;
            Frames = frames;
            FramesPerSecond = framesPerSecond;
        }
    }
}