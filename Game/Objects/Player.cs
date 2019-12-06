using Game.Utils;
using GXPEngine;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace Game {
    public class Player : GameObject {
        public float Speed = 200f;
        private Camera mainCamera;
        private Texture2D playerRight, playerLeft, playerDown;
        private Texture2D activeTexture;

        public Player(Vector2 position) {
            SetXY(position.x * Globals.TILE_SIZE, position.y * Globals.TILE_SIZE);
            mainCamera = new Camera(-32, -32, Globals.WIDTH, Globals.HEIGHT);
            playerRight = Texture2D.GetInstance("data/playerRight.png", true);
            playerLeft = Texture2D.GetInstance("data/playerLeft.png", true);
            playerDown = Texture2D.GetInstance("data/playerDown.png", true);
            activeTexture = playerRight;
            AddChild(mainCamera);
        }

        public void Update() {
            var movement = new Vector2(Input.GetAxisDown("Horizontal"), Input.GetAxisDown("Vertical"));
            if (!movement.Equals(Vector2.zero)) {
                var dTilePosition = new Vector2(x, y) + (movement * Globals.TILE_SIZE);
                var dTileGrid = WorldToGrid(dTilePosition);
                var currentPosGrid = WorldToGrid(new Vector2(x, y));
                var dTile = Level.Tileset.Tiles[Level.ActiveLevel.Tiles[(int) dTileGrid.x, (int) dTileGrid.y]];
                var world = parent.GetChildren().Find(o => o.name.Equals("World")) as World;
                if (dTile.Passable) {
                    Move(movement * Globals.TILE_SIZE);
                    if (dTile.Name == "Dirt" || dTile.Name == "Miner") Level.ActiveLevel.Tiles[(int) dTileGrid.x, (int) dTileGrid.y] = TileType.Empty;
                    Level.ActiveLevel.Tiles[(int) dTileGrid.x, (int) dTileGrid.y] = TileType.Miner;
                }

                world.RebuildMesh();
            }
        }

        protected override void RenderSelf(GLContext glContext) {
            float[] verts = {
                0, 0, Globals.TILE_SIZE, 0, Globals.TILE_SIZE, Globals.TILE_SIZE, 0, Globals.TILE_SIZE
            };
            float[] uv = {0, 0, 1, 0, 1, 1, 0, 1};
            activeTexture.Bind();
            glContext.SetColor(255, 255, 255, 255);
            glContext.DrawQuad(verts, uv);
            glContext.SetColor(255, 255, 255, 255);
        }

        public Vector2 WorldToGrid(Vector2 world) {
            return (world / Globals.TILE_SIZE);
        }

        public Vector2 GridToWorld(Vector2 grid) {
            return grid * Globals.TILE_SIZE;
        }
    }
}