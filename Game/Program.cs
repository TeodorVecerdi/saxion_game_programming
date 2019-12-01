using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GXPEngine;
using GXPEngine.Core;
using Newtonsoft.Json;

namespace GXPEngineTest {
    public class Program : GXPEngine.Game {
        public Program() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC,
            pPixelArt: Globals.PIXEL_ART) {
            targetFps = 1000;
            ShowMouse(true);
            
            var ed = new EasyDraw(Globals.WIDTH, Globals.HEIGHT, false);
            ed.name = "UI_EasyDraw";
            var t1 = new TestObject(1024, 1024, 1, Texture2D.GetInstance("data/TileMap_World.png", true));
//            var t2 = new TestObject(3, 3, 64f, Texture2D.GetInstance("data/checkers.png", true));
//            var t3 = new TestObject(2, 2, 48f, Texture2D.GetInstance("data/checkers.png", true));
//            t2.Move(128f, 128f);
//            t3.Move(256f, 256f);

//            Canvas canvas = new Canvas(800, 600);
//            canvas.graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(0, 0, 400, 300));
//            canvas.graphics.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(400, 0, 400, 300));
//            canvas.graphics.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(0, 300, 400, 300));
//            canvas.graphics.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(400, 300, 400, 300));
//            AddChild(canvas);
            AddChild(ed);
            AddChild(t1);
            GetChildren().ForEach((child) => {
                Console.WriteLine(child.name);
            });
            //            AddChild(t2);
//            AddChild(t3);
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
        public string TileAtlasTexture;
        public int Columns;
        public int Rows;
        public List<TileAtlasTile> TileAtlasTiles;
        
        public TestSerialize() { }
        public void SaveJson(string FileName) {
            using (var writer = new StreamWriter(FileName)) {
                var json = JsonConvert.SerializeObject(this);
                writer.Write(json);
                writer.Flush();
                Logger.Log("Vector2=" + new Vector2(16, 8));
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