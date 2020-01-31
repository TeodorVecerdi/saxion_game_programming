using System;
using System.Collections.Generic;
using GXPEngine;

namespace Game.Utils {
    public class Level {
        public static TiledTileset Tileset = new TiledTileset(Loader.LoadTileset("data/Levels/Tilesets/Main.tsx"), "data/Levels/Tilesets/Main.xml");
        public readonly int[,] OriginalTiles;
        public Vector2 PlayerStart;
        public string LevelPath;

        public int[,] Tiles;

        public Level(string path) {
            LevelPath = path;
            var gameLevel = Loader.LoadGameLevel(path);
            RandomObjects = new List<ValueTuple<int, int>>();
            for (var i = 0; i < gameLevel.RandomObject.Count; i++)
                RandomObjects.Add(ValueTuple<int,int>.Create(gameLevel.RandomObject[i].Id, gameLevel.RandomObjectProb[i].Prob));
            Name = gameLevel.Name;
            Description = gameLevel.Description;
            Width = gameLevel.Width;
            Height = gameLevel.Height;
            MagicWallMillingTime = gameLevel.MagicWallMillingTime;
            AmoebaSlowGrowthTime = gameLevel.AmoebaSlowGrowthTime;
            IntialDiamondValue = gameLevel.IntialDiamondValue;
            ExtraDiamondValue = gameLevel.ExtraDiamondValue;
            RandomSeed = gameLevel.RandomSeed;
            DiamondsNeeded = gameLevel.DiamondsNeeded;
            CaveTime = gameLevel.CaveTime;
            Color1 = gameLevel.Color1;
            Color2 = gameLevel.Color2;
            AmoebaMaxSize = gameLevel.AmoebaMaxSize;
            NextLevelPath = gameLevel.NextLevelPath;
            var XmlTiledLevel = Loader.LoadTiledMap(gameLevel.TiledLevelPath);
            Tiles = new int[Width, Height];
            OriginalTiles = new int[Width, Height];
            var tiles = XmlTiledLevel.XmlTiledLevelLayer.XmlTiledLevelData.Text.Split(',');
            for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++) {
                var currentTile = int.Parse(tiles[y * Width + x]) - 1;
                if (currentTile == 11) PlayerStart = new Vector2(x, y);
                Tiles[x, y] = currentTile;
                OriginalTiles[x, y] = currentTile;
            }
        }

        public int this[Vector2 position] {
            get => Tiles[(int) position.x, (int) position.y];
            set => Tiles[(int) position.x, (int) position.y] = value;
        }

        public int this[int x, int y] {
            get => Tiles[x, y];
            set => Tiles[x, y] = value;
        }

        public void Reset() {
            for (var i = 0; i < Height; i++)
            for (var j = 0; j < Width; j++)
                Tiles[j, i] = OriginalTiles[j, i];
        }

        #region GameLevel Variables
        public List<ValueTuple<int, int>> RandomObjects;
        public string Name;
        public string Description;
        public int Width;
        public int Height;
        public int MagicWallMillingTime;
        public int AmoebaSlowGrowthTime;
        public int IntialDiamondValue;
        public int ExtraDiamondValue;
        public int RandomSeed;
        public int DiamondsNeeded;
        public int CaveTime;
        public int Color1;
        public int Color2;
        public int AmoebaMaxSize;
        public string NextLevelPath;
        #endregion
    }
}