using System.IO;
using System.Xml.Serialization;
using GXPEngine;
using GXPEngine.Core;
using Newtonsoft.Json;

namespace GXPEngineTest {
    public class Program : Game {
        public Program() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC,
            pPixelArt: Globals.PIXEL_ART) {
            Logger.Log("Game started");
            var t1 = new TestObject(8, 8, 32f, Texture2D.GetInstance("checkers.png", true));
            var t2 = new TestObject(3, 3, 64f, Texture2D.GetInstance("checkers.png", true));
            var t3 = new TestObject(2, 2, 48f, Texture2D.GetInstance("checkers.png", true));
            t2.Move(128f, 128f);
            t3.Move(256f, 256f);

//            Canvas canvas = new Canvas(800, 600);
//            canvas.graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(0, 0, 400, 300));
//            canvas.graphics.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(400, 0, 400, 300));
//            canvas.graphics.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(0, 300, 400, 300));
//            canvas.graphics.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(400, 300, 400, 300));
//            AddChild(canvas);
            AddChild(t1);
            AddChild(t2);
            AddChild(t3);
            Logger.LogWarn("Added canvas as child to game");
        }

        public static void Main(string[] args) {
            Logger.LogError("I HAS ERROR");
            var ts = new TestSerialize(800, 600, false, false);
            ts.SaveJson("test.json");
            new Program().Start();
        }
    }

    public class TestSerialize {
        public bool fullscreen;
        public int height;
        public bool vsync;
        public int width;
        public TestSerialize() { }

        public TestSerialize(int width, int height, bool fullscreen, bool vsync) {
            this.width = width;
            this.height = height;
            this.fullscreen = fullscreen;
            this.vsync = vsync;
        }

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