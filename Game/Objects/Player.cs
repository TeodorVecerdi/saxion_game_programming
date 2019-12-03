using GXPEngine;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace Game {
    public class Player : GameObject {
        public float Speed = 200f;
        private Camera mainCamera;
        private Texture2D playerRight, playerLeft, playerDown;
        private Texture2D activeTexture;

        public Player() {
            mainCamera = new Camera(-32, -32, Globals.WIDTH, Globals.HEIGHT);
            playerRight = Texture2D.GetInstance("data/playerRight.png", true);
            playerLeft = Texture2D.GetInstance("data/playerLeft.png", true);
            playerDown = Texture2D.GetInstance("data/playerDown.png", true);
            activeTexture = playerRight;
            AddChild(mainCamera);
        }

        public void Update() {
            if (Input.GetKeyDown(Key.D) || Input.GetKeyDown(Key.RIGHT)) {
                Move(Globals.TILE_SIZE, 0f);
                activeTexture = playerRight;
            }

            if (Input.GetKeyDown(Key.A) || Input.GetKeyDown(Key.LEFT)) {
                Move(-Globals.TILE_SIZE, 0f);
                activeTexture = playerLeft;
            }

            if (Input.GetKeyDown(Key.S) || Input.GetKeyDown(Key.DOWN)) {
                Move(0f, Globals.TILE_SIZE);
                activeTexture = playerDown;
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
            glContext.SetColor(1, 1, 1, 1);
        }
    }
}