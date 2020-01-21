using System.Collections.Generic;

namespace Game.Utils {
    public static class TileType {
        public const int
            Empty = 0,
            SteelWall = 1,
            Door = 2,
            Brick = 3,
            Boulder = 4,
            Dirt = 5,
            Firefly = 6,
            MagicWall = 7,
            Amoeba = 8,
            Diamond = 9,
            Butterfly = 10,
            Miner = 11;

        public const int DiamondSpawner = 0xff00;
        public static Dictionary<int, string> TileTypeToClassName = new Dictionary<int, string> {
                {1, "Game.SteelWall"},
                {2, "Game.Door"},
                {3, "Game.Brick"},
                {4, "Game.Boulder"},
                {5, "Game.Dirt"},
                {6, "Game.Firefly"},
                {7, "Game.MagicWall"},
                {8, "Game.Amoeba"},
                {9, "Game.Diamond"},
                {10, "Game.Butterfly"},
            };
    }
}