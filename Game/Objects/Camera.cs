using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Camera : GameObject {
        private readonly World world;

        public Camera(World world) {
            this.world = world;
            AddChild(new GXPEngine.Camera(0, -20, Globals.WIDTH, Globals.HEIGHT));
        }

        public void SetPosition(Vector2 position) {
            position += new Vector2(Globals.WIDTH/2f, Globals.HEIGHT/2f);
            SetXY(position.x, position.y);
        }

        protected override void RenderSelf(GLContext glContext) {
            // DRAW UI
            if (!world.GotEnoughDiamonds) {
                drawUINumber(UITextures.YELLOWS[world.Level.DiamondsNeeded / 10 % 10], 0 * 64, glContext); // required diamonds
                drawUINumber(UITextures.YELLOWS[world.Level.DiamondsNeeded % 10], 1 * 64, glContext); // required diamonds
            } else {
                drawUINumber(UITextures.WHITES[10], 0 * 64, glContext); // required diamonds
                drawUINumber(UITextures.WHITES[10], 1 * 64, glContext); // required diamonds
            }

            drawUINumber(UITextures.WHITES[10], 2 * 64, glContext);
            drawUINumber(UITextures.WHITES[world.CurrentDiamondValue / 10 % 10], 3 * 64, glContext); //current diamond value
            drawUINumber(UITextures.WHITES[world.CurrentDiamondValue % 10], 4 * 64, glContext); //current diamond value
            drawUINumber(UITextures.BLACK, 5 * 64, glContext);

            //DIAMONDS
            drawUINumber(UITextures.YELLOWS[world.DiamondsCollected / 10 % 10], 6 * 64, glContext);
            drawUINumber(UITextures.YELLOWS[world.DiamondsCollected % 10], 7 * 64, glContext);
            drawUINumber(UITextures.BLACK, 8 * 64, glContext);

            //TIME
            var timeLeft = Mathf.RoundToInt(world.TimeLeft);
            drawUINumber(UITextures.WHITES[timeLeft / 100 % 10], 9 * 64, glContext);
            drawUINumber(UITextures.WHITES[timeLeft / 10 % 10], 10 * 64, glContext);
            drawUINumber(UITextures.WHITES[timeLeft % 10], 11 * 64, glContext);
            drawUINumber(UITextures.BLACK, 12 * 64, glContext);

            //SCORE
            drawUINumber(UITextures.WHITES[world.Score / 100000 % 10], 13 * 64, glContext);
            drawUINumber(UITextures.WHITES[world.Score / 10000 % 10], 14 * 64, glContext);
            drawUINumber(UITextures.WHITES[world.Score / 1000 % 10], 15 * 64, glContext);
            drawUINumber(UITextures.WHITES[world.Score / 100 % 10], 16 * 64, glContext);
            drawUINumber(UITextures.WHITES[world.Score / 10 % 10], 17 * 64, glContext);
            drawUINumber(UITextures.WHITES[world.Score % 10], 18 * 64, glContext);
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