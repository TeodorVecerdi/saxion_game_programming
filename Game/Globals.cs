namespace GXPEngineTest {
    public static class Globals {
        public const float ASPECT_RATIO = 1.7777777f;
        public const bool USE_ASPECT_RATIO = true;
        public const int WIDTH = 1920;
        public const int H_MAIN = 600;
        public const int H_ASPECT = (int) (WIDTH / ASPECT_RATIO);
        public const bool FULLSCREEN = true;
        public const bool VSYNC = false;
        public const bool PIXEL_ART = true;
        public static int HEIGHT => USE_ASPECT_RATIO ? H_ASPECT : H_MAIN;
    }
}