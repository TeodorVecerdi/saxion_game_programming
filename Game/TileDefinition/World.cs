
using GXPEngine;

namespace Game.TileDefinition {
    public class World {
        public const string TileMapName = "TileMap_World.png";
        public static TileDefinition Dirt = new TileDefinition("Dirt", 0f, 0f, 0f + 0.5f, 0f + 0.25f, 64f, 64f);
        public static TileDefinition Grass = new TileDefinition("Grass", 0.5f, 0f, 0.5f + 0.5f, 0f + 0.25f, 64f, 64f);
        public static TileDefinition Sand = new TileDefinition("Sand", 0f, 0.25f, 0f + 0.5f, 0.25f + 0.25f, 64f, 64f);
        public static TileDefinition Stone = new TileDefinition("Stone", 0.5f, 0.25f, 0.5f + 0.5f, 0.25f + 0.25f, 64f, 64f);
        public static TileDefinition Water = new TileDefinition("Water", 0f, 0.5f, 0f + 0.5f, 0.5f + 0.25f, 64f, 64f);
        

        public class TileDefinition {
            public string SpriteName;
            public Bounds SpriteUV;
            public Vector2 SpriteSize;

            public TileDefinition(string spriteName, float uvx, float uvy, float uvw, float uvh, float w, float h) {
                this.SpriteName = spriteName;
                this.SpriteUV = new Bounds(uvx, uvy, uvw, uvh);
                this.SpriteSize = new Vector2(w, h);
            }
        }

        public class Bounds {
            public float xa;
            public float ya;
            public float xb;
            public float yb;

            public Bounds(float xa, float ya, float xb, float yb) {
                this.xa = xa;
                this.ya = ya;
                this.xb = xb;
                this.yb = yb;
            }
        }
    }
}