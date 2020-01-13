using System;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Butterfly : GameObject {
        private readonly int animationFrames = 8;
        private int currentFrame;
        public bool IsFalling = false;
        private readonly Texture2D mainTexture;
        public bool UpdatedThisFrame = false;
        private readonly float uvSize = 0.125f;
        public Butterfly(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Butterfly(float x, float y, int color1, int color2) {
            SetXY(x, y);
            mainTexture = Texture2D.GetInstance("data/tiles/butterflyAnimated.png", true);
            try {
                var target = Misc.ApplyLevelColor("data/tiles/butterflyAnimated.png", color1, color2);
                mainTexture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file \"data/tiles/butterflyAnimated.png\"");
                throw e;
            }
        }

        private void Update() {
            currentFrame -= -1;
            currentFrame %= animationFrames;
        }

        protected override void RenderSelf(GLContext glContext) {
            float[] uv = {currentFrame * uvSize, 0, (currentFrame + 1) * uvSize, 0, (currentFrame + 1) * uvSize, 1, currentFrame * uvSize, 1};
            mainTexture.Bind();

//            glContext.SetColor((byte) ((color >> 16)&0xff),(byte) ((color >> 8)&0xff),(byte) (color&0xff),0xff);
            glContext.SetColor(0xff, 0xff, 0xff, 0xff);
            glContext.DrawQuad(Globals.QUAD_VERTS, uv);
            glContext.SetColor(0xff, 0xff, 0xff, 0xff);
            mainTexture.Unbind();
        }
    }
}