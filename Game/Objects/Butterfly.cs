using System;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Butterfly : GameObject {
        public bool UpdatedThisFrame = false;
        public Vector2Int Direction = Vector2Int.left;
        private const int animationFrames = 8;
        private const float uvSize = 0.125f;
        private readonly Texture2D mainTexture;
        private int currentFrame;
        public Butterfly(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Butterfly(float x, float y, int color1, int color2) {
            SetXY(x, y);
            mainTexture = Texture2D.GetInstance("data/tiles/butterfly.png", true);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/butterfly.png", color1, color2);
                mainTexture.SetBitmap(target);
            } catch (Exception) {
                Console.WriteLine("Could not find file data/tiles/butterfly.png");
                throw;
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