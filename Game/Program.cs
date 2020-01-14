using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Game.Utils;
using GXPEngine;
using Newtonsoft.Json;

namespace Game {
    public class Program : GXPEngine.Game {
        public Program() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC,
            pPixelArt: Globals.PIXEL_ART) {
            ShowMouse(true);
            targetFps = 24;
            var level1 = new Level("data/Levels/GameLevels/Level1.xml");
            var world = new World(level1);
            world.name = "World";
            Input.AddAxis("Horizontal", new List<int> {Key.A, Key.LEFT}, new List<int> {Key.D, Key.RIGHT});
            Input.AddAxis("Vertical", new List<int> {Key.W, Key.UP}, new List<int> {Key.S, Key.DOWN});
            AddChild(world);
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