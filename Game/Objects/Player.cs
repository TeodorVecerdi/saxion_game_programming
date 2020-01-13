using System;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Player : GameObject {
        private readonly int animationFrames = 8;
        private readonly Camera mainCamera;
        private readonly Texture2D playerIdleBlinkTap;
        private readonly Texture2D playerIdleBlink;
        private readonly Texture2D playerIdleTap;
        private readonly Texture2D playerIdle;
        private readonly Texture2D playerLeft;
        private readonly Texture2D playerRight;
        private readonly float uvSize = 0.125F;
        private readonly World world;
        private int currentDirection = -1;
        private int currentFrame;
        private bool isIdle = true;
        private bool idleTap;
        private bool idleBlink;
        private Texture2D mainTexture;

        public Player(Vector2 position, World world, int color1, int color2) {
            SetXY(position.x, position.y);
            mainCamera = new Camera(0, -20, Globals.WIDTH, Globals.HEIGHT);
            playerRight = Texture2D.GetInstance("data/tiles/playerRight.png", true);
            playerLeft = Texture2D.GetInstance("data/tiles/playerLeft.png", true);
            playerIdleBlinkTap = Texture2D.GetInstance("data/tiles/playerIdleBlinkTap.png", true);
            playerIdleBlink = Texture2D.GetInstance("data/tiles/playerIdleBlink.png", true);
            playerIdleTap = Texture2D.GetInstance("data/tiles/playerIdleTap.png", true);
            playerIdle = Texture2D.GetInstance("data/tiles/playerIdle.png", true);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerRight.png", color1, color2);
                playerRight.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file data/tiles/playerRight.png");
                throw e;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerLeft.png", color1, color2);
                playerLeft.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file data/tiles/playerLeft.png");
                throw e;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerIdleBlinkTap.png", color1, color2);
                playerIdleBlinkTap.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file data/tiles/playerIdleBlinkTap.png");
                throw e;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerIdleBlink.png", color1, color2);
                playerIdleBlink.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file data/tiles/playerIdleBlink.png");
                throw e;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerIdleTap.png", color1, color2);
                playerIdleTap.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file data/tiles/playerIdleTap.png");
                throw e;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerIdle.png", color1, color2);
                playerIdle.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file data/tiles/playerIdle.png");
                throw e;
            }

            mainTexture = playerIdle;
            this.world = world;
            AddChild(mainCamera);
        }

        public int CurrentDirection {
            get => currentDirection;
            set {
                if (value != currentDirection) currentFrame = 0;
                isIdle = false;
                currentDirection = value;
            }
        }

        private void Update() {
            currentFrame += 1;
            currentFrame %= animationFrames;

            if (isIdle) {
                if (idleBlink && idleTap) mainTexture = playerIdleBlinkTap;
                else if (idleBlink) mainTexture = playerIdleBlink;
                else if (idleTap) mainTexture = playerIdleTap;
                else mainTexture = playerIdle;
            } else {
                if (currentDirection == -1) mainTexture = playerLeft;
                if (currentDirection == 1) mainTexture = playerRight;
            }
        }

        protected override void RenderSelf(GLContext glContext) {
            // RENDER PLAYER
            float[] uv = {currentFrame * uvSize, 0, (currentFrame + 1) * uvSize, 0, (currentFrame + 1) * uvSize, 1, currentFrame * uvSize, 1};
            mainTexture.Bind();
            glContext.SetColor(0xff, 0xff, 0xff, 0xff);
            glContext.DrawQuad(Globals.QUAD_VERTS, uv);
            glContext.SetColor(0xff, 0xff, 0xff, 0xff);
            mainTexture.Unbind();
            glContext.SetColor(255, 255, 255, 255);

            // DRAW UI
            if (!world.GotEnoughDiamonds) {
                var n1 = world.Level.DiamondsNeeded % 10;
                var n2 = world.Level.DiamondsNeeded / 10 % 10;
                drawUINumber(UITextures.YELLOWS[n2], 0 * 64, glContext); // required diamonds
                drawUINumber(UITextures.YELLOWS[n1], 1 * 64, glContext); // required diamonds
            } else {
                drawUINumber(UITextures.WHITES[10], 0 * 64, glContext); // required diamonds
                drawUINumber(UITextures.WHITES[10], 1 * 64, glContext); // required diamonds
            }

            drawUINumber(UITextures.WHITES[10], 2 * 64, glContext);
            var dv1 = world.CurrentDiamondValue % 10;
            var dv2 = world.CurrentDiamondValue / 10 % 10;
            drawUINumber(UITextures.WHITES[dv2], 3 * 64, glContext); //current diamond value
            drawUINumber(UITextures.WHITES[dv1], 4 * 64, glContext); //current diamond value
            drawUINumber(UITextures.BLACK, 5 * 64, glContext);

            //DIAMONDS
            var d1 = world.DiamondsCollected % 10;
            var d2 = world.DiamondsCollected / 10 % 10;
            drawUINumber(UITextures.YELLOWS[d2], 6 * 64, glContext);
            drawUINumber(UITextures.YELLOWS[d1], 7 * 64, glContext);
            drawUINumber(UITextures.BLACK, 8 * 64, glContext);

            //TIME
            var t1 = Mathf.RoundToInt(world.TimeLeft) % 10;
            var t2 = Mathf.RoundToInt(world.TimeLeft) / 10 % 10;
            var t3 = Mathf.RoundToInt(world.TimeLeft) / 100 % 10;
            drawUINumber(UITextures.WHITES[t3], 9 * 64, glContext);
            drawUINumber(UITextures.WHITES[t2], 10 * 64, glContext);
            drawUINumber(UITextures.WHITES[t1], 11 * 64, glContext);
            drawUINumber(UITextures.BLACK, 12 * 64, glContext);

            //SCORE
            var s1 = world.Score % 10;
            var s2 = world.Score / 10 % 10;
            var s3 = world.Score / 100 % 10;
            var s4 = world.Score / 1000 % 10;
            var s5 = world.Score / 10000 % 10;
            var s6 = world.Score / 100000 % 10;
            drawUINumber(UITextures.WHITES[s6], 13 * 64, glContext);
            drawUINumber(UITextures.WHITES[s5], 14 * 64, glContext);
            drawUINumber(UITextures.WHITES[s4], 15 * 64, glContext);
            drawUINumber(UITextures.WHITES[s3], 16 * 64, glContext);
            drawUINumber(UITextures.WHITES[s2], 17 * 64, glContext);
            drawUINumber(UITextures.WHITES[s1], 18 * 64, glContext);
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

        public void SetIdle(bool idle) {
            isIdle = idle;
            if (idle) {
                idleBlink = Rand.RangeInclusive(1, 4) == 1 ? !idleBlink : idleBlink;
                idleTap = Rand.RangeInclusive(1, 16) == 1 ? !idleTap : idleTap;
            }
        }
    }
}