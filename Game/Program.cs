using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using GXPEngine;
using GXPEngine.Core;
using Newtonsoft.Json;
using Rectangle = GXPEngine.Core.Rectangle;

namespace GXPEngineTest {
    public class Program : GXPEngine.Game {
        public Program() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC,
            pPixelArt: Globals.PIXEL_ART) {
            ShowMouse(true);

//            var ed = new EasyDraw(Globals.WIDTH, Globals.HEIGHT, false);
//            ed.name = "UI_EasyDraw";
            var t1 = new TestObject(8, 8, 16, Texture2D.GetInstance("data/TileMap_World.png", true));
            var world = new TestObject(16, 16, 64, Texture2D.GetInstance("data/TileMap_World.png", true));
            world.disableMovement = true;
            var camera = new Camera(-4 * 16, -4 * 16, Globals.WIDTH, Globals.HEIGHT);
            t1.AddChild(camera);

            var UI = new Window(0, 0, Globals.WIDTH, Globals.HEIGHT, new Camera(0, 0, Globals.WIDTH, Globals.HEIGHT));
            OnAfterRender += UI.RenderWindow;
            Canvas canvas = new Canvas(800, 600);
            canvas.graphics.DrawLine(new Pen(new SolidBrush(Color.Chartreuse), 2), Globals.WIDTH / 2f, 0, Globals.WIDTH / 2f, Globals.HEIGHT);
            canvas.graphics.DrawLine(new Pen(new SolidBrush(Color.Chartreuse), 2), 0, Globals.HEIGHT / 2f, Globals.WIDTH, Globals.HEIGHT / 2f);

//            t1.AddChild(canvas);
//            canvas.AddChild(UI.camera);
            camera.AddChild(canvas);
//            canvas.AddChild(camera);
            AddChild(world);
            AddChild(t1);
            AddChild(canvas);
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