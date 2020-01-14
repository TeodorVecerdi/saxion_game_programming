using System;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Firefly : GameObject {
        private const int animationFrames = 8;
        private const float uvSize = 0.125f;
        private readonly Texture2D mainTexture;
        private int currentFrame;
        public Vector2Int Direction = Vector2Int.left;
        public bool UpdatedThisFrame = false;
        public Firefly(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Firefly(float x, float y, int color1, int color2) {
            SetXY(x, y);
            mainTexture = Texture2D.GetInstance("data/tiles/firefly.png", true);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/firefly.png", color1, color2);
                mainTexture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file data/tiles/firefly.png");
                throw e;
            }
        }

        private void Update() {
            currentFrame += 1;
            currentFrame %= animationFrames;
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