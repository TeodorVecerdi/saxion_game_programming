using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GXPEngine.Core;

namespace Game.Utils {
    public static class Loader {
        private static T LoadXml<T>(string path) where T : class {
            using (var stream = File.OpenRead(path)) {
                var serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(stream) as T;
            }
        }

        public static XmlGameLevel LoadGameLevel(string path) {
            return LoadXml<XmlGameLevel>(path);
        }

        public static XmlTiledLevelMap LoadTiledMap(string path) {
            return LoadXml<XmlTiledLevelMap>(path);
        }

        public static XmlTiledTileset LoadTileset(string path) {
            return LoadXml<XmlTiledTileset>(path);
        }

        public static XmlTiles LoadTilesetDefinition(string path) {
            return LoadXml<XmlTiles>(path);
        }
    }

    #region XmlGameLevel Classes
        // @formatter:off
        [XmlRoot(ElementName = "randomObject")]
        public class XmlGameLevelRandomObject {
            [XmlAttribute(AttributeName = "id")] public int Id { get; set; }}

        [XmlRoot(ElementName = "randomObjectProb")]
        public class XmlGameLevelRandomObjectProb {
            [XmlAttribute(AttributeName = "prob")] public int Prob { get; set; }}

        [XmlRoot(ElementName = "level")]
        public class XmlGameLevel {
            [XmlElement(ElementName = "randomObject")] public List<XmlGameLevelRandomObject> RandomObject { get; set; }
            [XmlElement(ElementName = "randomObjectProb")] public List<XmlGameLevelRandomObjectProb> RandomObjectProb { get; set; }
            [XmlAttribute(AttributeName = "name")] public string Name { get; set; }
            [XmlAttribute(AttributeName = "description")] public string Description { get; set; }
            [XmlAttribute(AttributeName = "tiledLevelPath")] public string TiledLevelPath { get; set; }
            [XmlAttribute(AttributeName = "width")] public int Width { get; set; }
            [XmlAttribute(AttributeName = "height")] public int Height { get; set; }
            [XmlAttribute(AttributeName = "magicWallMillingTime")] public int MagicWallMillingTime { get; set; }
            [XmlAttribute(AttributeName = "amoebaSlowGrowthTime")] public int AmoebaSlowGrowthTime { get; set; }
            [XmlAttribute(AttributeName = "intialDiamondValue")] public int IntialDiamondValue { get; set; }
            [XmlAttribute(AttributeName = "extraDiamondValue")] public int ExtraDiamondValue { get; set; }
            [XmlAttribute(AttributeName = "randomSeed")] public int RandomSeed { get; set; }
            [XmlAttribute(AttributeName = "diamondsNeeded")] public int DiamondsNeeded { get; set; }
            [XmlAttribute(AttributeName = "caveTime")] public int CaveTime { get; set; }
            [XmlAttribute(AttributeName = "color1")] public int Color1 { get; set; }
            [XmlAttribute(AttributeName = "color2")] public int Color2 { get; set; }
            [XmlAttribute(AttributeName = "amoebaMaxSize")] public int AmoebaMaxSize { get; set; }
            [XmlAttribute(AttributeName = "nextLevelPath")] public string NextLevelPath {get; set;}}

// @formatter:on
    #endregion

    #region XmlTiledLevel Classes
        // @formatter:off
        [XmlRoot(ElementName = "tileset")]
        public class XmlTiledLevelTileset {
            [XmlAttribute(AttributeName = "firstgid")]
            public string Firstgid { get; set; }
            [XmlAttribute(AttributeName = "source")]
            public string Source { get; set; }}

        [XmlRoot(ElementName = "data")]
        public class XmlTiledLevelData {
            [XmlAttribute(AttributeName = "encoding")]
            public string Encoding { get; set; }
            [XmlText] public string Text { get; set; }}

        [XmlRoot(ElementName = "layer")]
        public class XmlTiledLevelLayer {
            [XmlElement(ElementName = "data")] public XmlTiledLevelData XmlTiledLevelData { get; set; }
            [XmlAttribute(AttributeName = "id")] public string Id { get; set; }
            [XmlAttribute(AttributeName = "name")] public string Name { get; set; }
            [XmlAttribute(AttributeName = "width")]
            public string Width { get; set; }
            [XmlAttribute(AttributeName = "height")]
            public string Height { get; set; }}

        [XmlRoot(ElementName = "map")]
        public class XmlTiledLevelMap {
            [XmlElement(ElementName = "tileset")] public XmlTiledLevelTileset XmlTiledLevelTileset { get; set; }
            [XmlElement(ElementName = "layer")] public XmlTiledLevelLayer XmlTiledLevelLayer { get; set; }
            [XmlAttribute(AttributeName = "version")]
            public string Version { get; set; }
            [XmlAttribute(AttributeName = "tiledversion")]
            public string Tiledversion { get; set; }
            [XmlAttribute(AttributeName = "orientation")]
            public string Orientation { get; set; }
            [XmlAttribute(AttributeName = "renderorder")]
            public string Renderorder { get; set; }
            [XmlAttribute(AttributeName = "compressionlevel")]
            public string Compressionlevel { get; set; }
            [XmlAttribute(AttributeName = "width")]
            public int Width { get; set; }
            [XmlAttribute(AttributeName = "height")]
            public int Height { get; set; }
            [XmlAttribute(AttributeName = "tilewidth")]
            public int Tilewidth { get; set; }
            [XmlAttribute(AttributeName = "tileheight")]
            public int Tileheight { get; set; }
            [XmlAttribute(AttributeName = "infinite")]
            public int Infinite { get; set; }
            [XmlAttribute(AttributeName = "nextlayerid")]
            public string Nextlayerid { get; set; }
            [XmlAttribute(AttributeName = "nextobjectid")]
            public string Nextobjectid { get; set; }}

    // @formatter:on
    #endregion

    #region XmlTiledTileset Classes
    // @formatter:on
    [XmlRoot(ElementName = "image")]
    public class XmlTiledTilesetImage {
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }
        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }
    }

    [XmlRoot(ElementName = "tile")]
    public class XmlTiledTilesetTile {
        [XmlAttribute(AttributeName = "id")] public int Id { get; set; }
        [XmlAttribute(AttributeName = "type")] public string Type { get; set; }
    }

    [XmlRoot(ElementName = "tileset")]
    public class XmlTiledTileset {
        [XmlElement(ElementName = "image")] public XmlTiledTilesetImage XmlTiledTilesetImage { get; set; }
        [XmlElement(ElementName = "tile")] public List<XmlTiledTilesetTile> Tile { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "tiledversion")]
        public string Tiledversion { get; set; }
        [XmlAttribute(AttributeName = "name")] public string Name { get; set; }
        [XmlAttribute(AttributeName = "tilewidth")]
        public int Tilewidth { get; set; }
        [XmlAttribute(AttributeName = "tileheight")]
        public int Tileheight { get; set; }
        [XmlAttribute(AttributeName = "tilecount")]
        public int Tilecount { get; set; }
        [XmlAttribute(AttributeName = "columns")]
        public int Columns { get; set; }
        public int Rows => Tilecount % Columns == 0 ? Tilecount / Columns : Tilecount / Columns + 1;
    }

    [XmlRoot(ElementName = "tile")]
    public class XmlTile {
        [XmlAttribute(AttributeName = "id")] public int Id { get; set; }
        [XmlAttribute(AttributeName = "passable")]
        public int Passable { get; set; }
        [XmlAttribute(AttributeName = "rounded")]
        public int Rounded { get; set; }
        [XmlAttribute(AttributeName = "canFall")]
        public int CanFall { get; set; }
    }

    [XmlRoot(ElementName = "tiles")]
    public class XmlTiles {
        [XmlElement(ElementName = "tile")] public List<XmlTile> Tile { get; set; }
        [XmlAttribute(AttributeName = "parent")]
        public string Parent { get; set; }
    }

    // @formatter:on
    #endregion

    public class Tile {
        public bool CanFall;
        public int Id;
        public string Name;
        public bool Passable;
        public bool Rounded;
        public Rectangle UV;

        public Tile(int id, string name, bool passable, bool rounded, bool canFall) {
            Id = id;
            Name = name;
            Passable = passable;
            Rounded = rounded;
            CanFall = canFall;
            var col = id % 4;
            var row = id / 4;
            UV = new Rectangle(col * 0.25F, row * 0.25F, 0.25F, 0.25F);
        }
    }

    public class TiledTileset {
        public Dictionary<int, Tile> Tiles;

        public TiledTileset(XmlTiledTileset tileset, string tileDefinitionFile) {
            var xmlTiles = Loader.LoadTilesetDefinition(tileDefinitionFile);
            Tiles = new Dictionary<int, Tile>();
            Debug.Log("Building tiles: " + tileset.Name);
            foreach (var tile in tileset.Tile) {
                var tile2 = xmlTiles.Tile.Find(xmlTile => xmlTile.Id == tile.Id);
                Tiles.Add(tile.Id, new Tile(tile.Id, tile.Type, tile2.Passable == 1, tile2.Rounded == 1, tile2.CanFall == 1));
            }
        }
    }
}