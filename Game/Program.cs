using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Game;
using GXPEngine;
using GXPEngine.Core;
using Newtonsoft.Json;

namespace Game {
    public class Program : GXPEngine.Game {
        public Program() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC,
            pPixelArt: Globals.PIXEL_ART) {
            ShowMouse(true);
//            var t1 = new GameBackgroundTest(8, 8, 16, Texture2D.GetInstance("data/TileMap_World.png", true));
            var world = new GameBackgroundTest(16, 16, Globals.TILE_SIZE, Texture2D.GetInstance("data/TileMap_World.png", true));
//            var camera = new Camera(-4 * 16, -4 * 16, Globals.WIDTH, Globals.HEIGHT);
            var player = new Player();
//            t1.AddChild(camera);

//            t1.AddChild(canvas);
            TiledMapParser.Map map = TiledMapParser.MapParser.ReadMap("data/Level1.tmx");
            Debug.Log(map.Layers[0].Data.innerXML);
//            Debug.Log(map);
            AddChild(world);
            AddChild(player);

//            AddChild(t1);
        }

        public static void Main(string[] args) {
            new Program().Start();
        }
    }

    [Serializable]
    public struct TileAtlasTile {
        public string TileName;
        public int Column;
        public int Row;

        public TileAtlasTile(string tileName, int column, int row) {
            TileName = tileName;
            Column = column;
            Row = row;
        }
    }

    public class TestSerialize {
        public int Columns;
        public int Rows;
        public string TileAtlasTexture;
        public List<TileAtlasTile> TileAtlasTiles;

        public void SaveJson(string FileName) {
            using (var writer = new StreamWriter(FileName)) {
                var json = JsonConvert.SerializeObject(this);
                writer.Write(json);
                writer.Flush();
                Debug.Log("Vector2=" + new Vector2(16, 8));
            }
        }

        public void SaveXML(string FileName) {
            using (var writer = new StreamWriter(FileName)) {
                var serializer = new XmlSerializer(GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static TestSerialize LoadJson(string FileName) {
            using (var stream = File.OpenText(FileName)) {
                var obj = JsonConvert.DeserializeObject<TestSerialize>(stream.ReadToEnd());
                return obj;
            }
        }

        public static TestSerialize LoadXML(string FileName) {
            using (var stream = File.OpenRead(FileName)) {
                var serializer = new XmlSerializer(typeof(TestSerialize));
                return serializer.Deserialize(stream) as TestSerialize;
            }
        }
    }
}