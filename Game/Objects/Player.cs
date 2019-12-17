using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Player : Sprite {
        private Texture2D activeTexture;
        private readonly Camera mainCamera;
        private readonly Texture2D playerRight;
        private Texture2D playerLeft, playerDown;
        private readonly World world;

        public Player(Vector2 position, World world) : base("data/playerDown.png", true) {
            SetXY(position.x, position.y);
            mainCamera = new Camera(0, -20, Globals.WIDTH, Globals.HEIGHT);
            playerRight = Texture2D.GetInstance("data/playerRight.png", true);
            playerLeft = Texture2D.GetInstance("data/playerLeft.png", true);
            playerDown = Texture2D.GetInstance("data/playerDown.png", true);
            activeTexture = playerRight;
            this.world = world;
            AddChild(mainCamera);
        }

        protected override void RenderSelf(GLContext glContext) {
            base.RenderSelf(glContext);
            glContext.SetColor(255, 255, 255, 255);
            // DRAW UI
            if(!world.GotEnoughDiamonds) {
                var n1 = world.Level.DiamondsNeeded % 10;
                var n2 = world.Level.DiamondsNeeded / 10 % 10;
                drawUINumber(UITextures.YELLOWS[n2], 0*64, glContext); // required diamonds
                drawUINumber(UITextures.YELLOWS[n1], 1*64, glContext); // required diamonds
            } else {
                drawUINumber(UITextures.WHITES[10], 0*64, glContext); // required diamonds
                drawUINumber(UITextures.WHITES[10], 1*64, glContext); // required diamonds
            }
            drawUINumber(UITextures.WHITES[10], 2*64, glContext);
            var dv1 = world.CurrentDiamondValue % 10;
            var dv2 = world.CurrentDiamondValue / 10 % 10;
            drawUINumber(UITextures.WHITES[dv2], 3*64, glContext); //current diamond value
            drawUINumber(UITextures.WHITES[dv1], 4*64, glContext); //current diamond value
            drawUINumber(UITextures.BLACK, 5*64, glContext);
            
            //DIAMONDS
            var d1 = world.DiamondsCollected % 10;
            var d2 = world.DiamondsCollected / 10 % 10;
            drawUINumber(UITextures.YELLOWS[d2], 6*64, glContext);
            drawUINumber(UITextures.YELLOWS[d1], 7*64, glContext);
            drawUINumber(UITextures.BLACK, 8*64, glContext);
            
            //TIME
            var t1 = Mathf.RoundToInt(world.TimeLeft) % 10;
            var t2 = Mathf.RoundToInt(world.TimeLeft) / 10 % 10;
            var t3 = Mathf.RoundToInt(world.TimeLeft) / 100 % 10;
            drawUINumber(UITextures.WHITES[t3], 9*64, glContext);
            drawUINumber(UITextures.WHITES[t2], 10*64, glContext);
            drawUINumber(UITextures.WHITES[t1], 11*64, glContext);
            drawUINumber(UITextures.BLACK, 12*64, glContext);
            
            //SCORE
            var s1 = world.Score % 10;
            var s2 = world.Score / 10 % 10;
            var s3 = world.Score / 100 % 10;
            var s4 = world.Score / 1000 % 10;
            var s5 = world.Score / 10000 % 10;
            var s6 = world.Score / 100000 % 10;
            drawUINumber(UITextures.WHITES[s6], 13*64, glContext);
            drawUINumber(UITextures.WHITES[s5], 14*64, glContext);
            drawUINumber(UITextures.WHITES[s4], 15*64, glContext);
            drawUINumber(UITextures.WHITES[s3], 16*64, glContext);
            drawUINumber(UITextures.WHITES[s2], 17*64, glContext);
            drawUINumber(UITextures.WHITES[s1], 118*64, glContext);
        }

        private void drawUINumber(Texture2D tex, float offset, GLContext glContext) {
            tex.Bind();
            float[] verts = {
                0 - Globals.WIDTH / 2f + offset, 0 - Globals.HEIGHT / 2f + 20,
                Globals.TILE_SIZE - Globals.WIDTH / 2f + offset, 0 - Globals.HEIGHT / 2f + 20,
                Globals.TILE_SIZE - Globals.WIDTH / 2f + offset, 32 - Globals.HEIGHT / 2f + 20,
                0 - Globals.WIDTH / 2f + offset, 32 - Globals.HEIGHT / 2f + 20 
            };
            glContext.DrawQuad(verts, Globals.QUAD_UV);
        }
    }
    
}