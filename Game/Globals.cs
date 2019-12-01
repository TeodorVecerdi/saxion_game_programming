using System.ComponentModel;

namespace GXPEngineTest {
    public static class Globals {
        public const float ASPECT_RATIO = 1.7777777f;
        public const bool USE_ASPECT_RATIO = false;
        public const int WIDTH = 800;
        public const int H_MAIN = 600;
        public const int H_ASPECT = (int)(WIDTH / ASPECT_RATIO);
        public static int HEIGHT {
            get {
                if (USE_ASPECT_RATIO) return H_ASPECT;
                return H_MAIN;
            }
        }
        public const bool FULLSCREEN = false;
        public const bool VSYNC = false;
        public const bool PIXEL_ART = true;
    }
}