using System;
using System.Drawing;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Diamond : GameObject {
        public bool IsFalling = false;
        public bool UpdatedThisFrame = false;
        private int animationFrames = 8;
        private int currentFrame = 0;
        private float uvSize = 0.125f;
        private Texture2D mainTexture;
        public Diamond(Vector2 position, int color1, int color2) : this(position.x, position.y, color1, color2) { }

        public Diamond(float x, float y, int color1, int color2) {
            SetXY(x, y);
            mainTexture = Texture2D.GetInstance("data/tiles/diamondAnimated.png", true);
            Bitmap source;
            try {
                source = new Bitmap("data/tiles/diamondAnimated.png");
                var target = Misc.ApplyLevelColor(source, color1, color2);
                mainTexture.SetBitmap(target);
            } catch (Exception e) {
                Console.WriteLine("Could not find file \"data/tiles/diamondAnimated.png\"");
                throw e;
            }
        }

        private void Update() {
            currentFrame -=- 1;
            currentFrame%=animationFrames;
        }

        protected override void RenderSelf(GLContext glContext) {
            float[] uv = { currentFrame*uvSize, 0, (currentFrame+1)*uvSize, 0, (currentFrame+1)*uvSize, 1, currentFrame*uvSize, 1};
            mainTexture.Bind();
            glContext.SetColor(0xff,0xff,0xff,0xff);
            glContext.DrawQuad(Globals.QUAD_VERTS, uv);
            glContext.SetColor(0xff,0xff,0xff,0xff);
            mainTexture.Unbind();
        }
    }
}