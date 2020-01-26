using System;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class DiamondSpawner : GameObject {
        public bool UpdatedThisFrame = false;
        private const int animationFrames = 5;
        private const float uvSize = 0.2F;
        private readonly Texture2D mainTexture;
        private int currentFrame;
        private bool isDone;
        public DiamondSpawner(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public DiamondSpawner(float x, float y, int color1, int color2) {
            SetXY(x, y);
            mainTexture = Texture2D.GetInstance("data/tiles/diamondSpawner.png");
            try {
                var target = Misc.ApplyLevelColor("data/tiles/diamondSpawner.png", color1, color2);
                mainTexture.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/diamondSpawner.png");
                throw;
            }
        }

        private void Update() {
            if(!isDone)
                currentFrame += 1;
            if (currentFrame >= animationFrames) isDone = true;
        }

        protected override void RenderSelf(GLContext glContext) {
            float[] uv = {currentFrame * uvSize, 0, (currentFrame + 1) * uvSize, 0, (currentFrame + 1) * uvSize, 1, currentFrame * uvSize, 1};
            mainTexture.Bind();
            glContext.SetColor(0xff, 0xff, 0xff, 0xff);
            glContext.DrawQuad(Globals.QUAD_VERTS, uv);
            glContext.SetColor(0xff, 0xff, 0xff, 0xff);
            mainTexture.Unbind();
        }
    }
}