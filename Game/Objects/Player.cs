using System.Runtime.InteropServices;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Player : Sprite {
        private Camera mainCamera;
        private Texture2D playerRight, playerLeft, playerDown;
        private Texture2D activeTexture;
        private World world;
        private Texture2D[] UINumbers = {Texture2D.GetInstance("data/UI/0.png", true),
            Texture2D.GetInstance("data/UI/1.png", true),
            Texture2D.GetInstance("data/UI/2.png", true),
            Texture2D.GetInstance("data/UI/3.png", true),
            Texture2D.GetInstance("data/UI/4.png", true),
            Texture2D.GetInstance("data/UI/5.png", true),
            Texture2D.GetInstance("data/UI/6.png", true),
            Texture2D.GetInstance("data/UI/7.png", true),
            Texture2D.GetInstance("data/UI/8.png", true),
            Texture2D.GetInstance("data/UI/9.png", true),
            Texture2D.GetInstance("data/UI/diamond.png", true)
        };

        public Player(Vector2 position, World world) : base("data/playerDown.png", true, true) {
            SetXY(position.x, position.y);
            mainCamera = new Camera(-32, -32, Globals.WIDTH, Globals.HEIGHT);
            playerRight = Texture2D.GetInstance("data/playerRight.png", true);
            playerLeft = Texture2D.GetInstance("data/playerLeft.png", true);
            playerDown = Texture2D.GetInstance("data/playerDown.png", true);
            activeTexture = playerRight;
            this.world = world;
            AddChild(mainCamera);
        }

        protected override void RenderSelf(GLContext glContext) {
            base.RenderSelf(glContext);
            int n1 = world.diamondsCollected / 10 % 10;
            int n2 = world.diamondsCollected % 10;
            glContext.SetColor(255,255,255,255);
            UINumbers[n1].Bind();
            float[] verts = {0-Globals.WIDTH/2f + 32, 0-Globals.HEIGHT/2f + 32,
                Globals.TILE_SIZE-Globals.WIDTH/2f + 32, 0-Globals.HEIGHT/2f + 32,
                Globals.TILE_SIZE-Globals.WIDTH/2f + 32, 32-Globals.HEIGHT/2f + 32,
                0-Globals.WIDTH/2f + 32, 32-Globals.HEIGHT/2f + 32};
            glContext.DrawQuad(verts, Globals.QUAD_UV);
            UINumbers[n2].Bind();
            float[] verts2 = {0-Globals.WIDTH/2f + 32+64, 0-Globals.HEIGHT/2f + 32,
                Globals.TILE_SIZE-Globals.WIDTH/2f + 32+64, 0-Globals.HEIGHT/2f + 32,
                Globals.TILE_SIZE-Globals.WIDTH/2f + 32+64, 32-Globals.HEIGHT/2f + 32,
                0-Globals.WIDTH/2f + 32+64, 32-Globals.HEIGHT/2f + 32};
            glContext.DrawQuad(verts2, Globals.QUAD_UV);
            UINumbers[10].Bind();
            float[] verts3 = {0-Globals.WIDTH/2f + 32+64+64, 0-Globals.HEIGHT/2f + 32,
                Globals.TILE_SIZE-Globals.WIDTH/2f + 32+64+64, 0-Globals.HEIGHT/2f + 32,
                Globals.TILE_SIZE-Globals.WIDTH/2f + 32+64+64, 32-Globals.HEIGHT/2f + 32,
                0-Globals.WIDTH/2f + 32+64+64, 32-Globals.HEIGHT/2f + 32};
            glContext.DrawQuad(verts3, Globals.QUAD_UV);

        }
    }
}