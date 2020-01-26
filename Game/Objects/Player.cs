using System;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Player : GameObject {
        private const int animationFrames = 8;
        private const float uvSize = 0.125F;
        private readonly Texture2D playerIdle;
        private readonly Texture2D playerIdleBlink;
        private readonly Texture2D playerIdleBlinkTap;
        private readonly Texture2D playerIdleTap;
        private readonly Texture2D playerLeft;
        private readonly Texture2D playerRight;
        private int currentDirection = -1;
        private int currentFrame;
        private bool idleBlink;
        private bool idleTap;
        private bool isIdle = true;
        private Texture2D mainTexture;

        public Player(Vector2 position, int color1, int color2) {
            SetXY(position.x, position.y);
            #region Load Textures
            playerRight = Texture2D.GetInstance("data/tiles/playerRight.png", true);
            playerLeft = Texture2D.GetInstance("data/tiles/playerLeft.png", true);
            playerIdleBlinkTap = Texture2D.GetInstance("data/tiles/playerIdleBlinkTap.png", true);
            playerIdleBlink = Texture2D.GetInstance("data/tiles/playerIdleBlink.png", true);
            playerIdleTap = Texture2D.GetInstance("data/tiles/playerIdleTap.png", true);
            playerIdle = Texture2D.GetInstance("data/tiles/playerIdle.png", true);
            
            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerRight.png", color1, color2);
                playerRight.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/playerRight.png");
                throw;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerLeft.png", color1, color2);
                playerLeft.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/playerLeft.png");
                throw;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerIdleBlinkTap.png", color1, color2);
                playerIdleBlinkTap.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/playerIdleBlinkTap.png");
                throw;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerIdleBlink.png", color1, color2);
                playerIdleBlink.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/playerIdleBlink.png");
                throw;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerIdleTap.png", color1, color2);
                playerIdleTap.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/playerIdleTap.png");
                throw;
            }

            try {
                var target = Misc.ApplyLevelColor("data/tiles/playerIdle.png", color1, color2);
                playerIdle.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/playerIdle.png");
                throw;
            }
            #endregion
            mainTexture = playerIdle;
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