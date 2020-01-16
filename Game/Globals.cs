namespace Game {
    public static class Globals {
        public const float ASPECT_RATIO = 1.7777777f;
        public const bool USE_ASPECT_RATIO = true;
        public const int WIDTH = 1280;
        private const int H_MAIN = 768;
        private const int H_ASPECT = (int) (WIDTH / ASPECT_RATIO);
        public const bool FULLSCREEN = false;
        public const bool VSYNC = false;
        public const bool PIXEL_ART = true;
        public const float TILE_SIZE = 64f;

        public static float[] QUAD_VERTS = {0, 0, TILE_SIZE, 0, TILE_SIZE, TILE_SIZE, 0, TILE_SIZE};
        public static float[] QUAD_UV = {0, 0, 1, 0, 1, 1, 0, 1};
        public static int HEIGHT => USE_ASPECT_RATIO ? H_ASPECT : H_MAIN;
    }
}