namespace Game {
    public static class Globals {
        public const float ASPECT_RATIO = 1.7777777f;
        public const bool USE_ASPECT_RATIO = false;
        public const int WIDTH = 1048;
        private const int H_MAIN = 786;
        private const int H_ASPECT = (int) (WIDTH / ASPECT_RATIO);
        public const bool FULLSCREEN = false;
        public const bool VSYNC = false;
        public const bool PIXEL_ART = true;
        public static int HEIGHT => USE_ASPECT_RATIO ? H_ASPECT : H_MAIN;
        public static float TILE_SIZE = 64f;
    }
}