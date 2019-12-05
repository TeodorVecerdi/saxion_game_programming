using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Game.Utils {
    [XmlRoot(ElementName="randomObject")]
    public class GameLevelRandomObject {
        [XmlAttribute(AttributeName="id")]
        public int Id { get; set; }
    }

    [XmlRoot(ElementName="randomObjectProb")]
    public class GameLevelRandomObjectProb {
        [XmlAttribute(AttributeName="prob")]
        public int Prob { get; set; }
    }

    [XmlRoot(ElementName="level")]
    public class GameLevel {
        [XmlElement(ElementName="randomObject")]
        public List<GameLevelRandomObject> RandomObject { get; set; }
        [XmlElement(ElementName="randomObjectProb")]
        public List<GameLevelRandomObjectProb> RandomObjectProb { get; set; }
        [XmlAttribute(AttributeName="name")]
        public string Name { get; set; }
        [XmlAttribute(AttributeName="description")]
        public string Description { get; set; }
        [XmlAttribute(AttributeName="tiledLevelPath")]
        public string TiledLevelPath { get; set; }
        [XmlAttribute(AttributeName="width")]
        public int Width { get; set; }
        [XmlAttribute(AttributeName="height")]
        public int Height { get; set; }
        [XmlAttribute(AttributeName="magicWallMillingTime")]
        public int MagicWallMillingTime { get; set; }
        [XmlAttribute(AttributeName="amoebaSlowGrowthTime")]
        public int AmoebaSlowGrowthTime { get; set; }
        [XmlAttribute(AttributeName="intialDiamondValue")]
        public int IntialDiamondValue { get; set; }
        [XmlAttribute(AttributeName="extraDiamondValue")]
        public int ExtraDiamondValue { get; set; }
        [XmlAttribute(AttributeName="randomSeed")]
        public int RandomSeed { get; set; }
        [XmlAttribute(AttributeName="diamondsNeeded")]
        public int DiamondsNeeded { get; set; }
        [XmlAttribute(AttributeName="caveTime")]
        public int CaveTime { get; set; }
        [XmlAttribute(AttributeName="color1")]
        public int Color1 { get; set; }
        [XmlAttribute(AttributeName="color2")]
        public int Color2 { get; set; }
        [XmlAttribute(AttributeName="amoebaMaxSize")]
        public int AmoebaMaxSize { get; set; }
    }

    public class Level {
        public static TiledTileset TiledTileset = TiledTilesetLoader.LoadTileset("data/Levels/BoulderDash.tsx");
        public GameLevel GameLevel;
        public TiledLevelMap TiledLevel;
        public Tileset Tileset;

        public Level(string path) {
            GameLevel = GameLevelLoader.LoadGameLevel(path);
            TiledLevel = TiledLevelLoader.LoadTiledMap(GameLevel.TiledLevelPath);
            Tileset = new Tileset(TiledTileset);
        }
    }

    public static class GameLevelLoader {
        public static GameLevel LoadGameLevel(string path) {
            using (var stream = File.OpenRead(path)) {
                var serializer = new XmlSerializer(typeof(GameLevel));
                return serializer.Deserialize(stream) as GameLevel;
            }
        }
    }
}