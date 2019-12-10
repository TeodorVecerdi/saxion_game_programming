using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class Diamond : GameObject {
        private int animationFrames = 8;
        private int currentFrame = 0;
        private float uvSize = 0.125f;
        private Texture2D mainTexture;
        public Diamond(Vector2 position) : this(position.x, position.y) { }

        public Diamond(float x, float y) {
            SetXY(x, y);
            mainTexture = Texture2D.GetInstance("data/tiles/diamondAnimated.png", true);
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