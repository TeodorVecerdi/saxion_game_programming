using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using GXPEngine.Core;

namespace Game.Utils {
    /* 
     Licensed under the Apache License, Version 2.0
     https://xmltocsharp.azurewebsites.net/
     http://www.apache.org/licenses/LICENSE-2.0
     */
    [XmlRoot(ElementName = "image")]
    public class TiledTilesetImage {
        [XmlAttribute(AttributeName = "source")]
        public string Source { get; set; }
        [XmlAttribute(AttributeName = "width")]
        public int Width { get; set; }
        [XmlAttribute(AttributeName = "height")]
        public int Height { get; set; }
    }

    [XmlRoot(ElementName = "tile")]
    public class TiledTilesetTile {
        [XmlAttribute(AttributeName = "id")] public int Id { get; set; }
        [XmlAttribute(AttributeName = "type")] public string Type { get; set; }
    }

    [XmlRoot(ElementName = "tileset")]
    public class TiledTileset {
        [XmlElement(ElementName = "image")] public TiledTilesetImage TiledTilesetImage { get; set; }
        [XmlElement(ElementName = "tile")] public List<TiledTilesetTile> Tile { get; set; }
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
        public int Rows => (Tilecount % Columns == 0 ? Tilecount / Columns : Tilecount / Columns + 1);

    }

    public class TiledTile {
        public int Id;
        public string Name;
        public Rectangle UV;

        public TiledTile(int id, string name) {
            Id = id;
            Name = name;
            int col = id % 4;
            int row = id / 4;
            UV = new Rectangle(col*0.25F, row*0.25F, 0.25F, 0.25F);
        }
    }

    public class Tileset {
        public Dictionary<int, TiledTile> Tiles;
        public Tileset(TiledTileset tileset) {
            Tiles = new Dictionary<int, TiledTile>();
            foreach (var tile in tileset.Tile) {
                Tiles.Add(tile.Id, new TiledTile(tile.Id, tile.Type));
            }
        }
    }

    public static class TiledTilesetLoader {
        public static TiledTileset LoadTileset(string path) {
            using (var stream = File.OpenRead(path)) {
                var serializer = new XmlSerializer(typeof(TiledTileset));
                return serializer.Deserialize(stream) as TiledTileset;
            }
        }
    }
}