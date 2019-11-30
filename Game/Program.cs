using System;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GXPEngine;
using Newtonsoft.Json;

namespace GXPEngineTest {
    public class Program : Game {
        public Program() : base(pWidth: Globals.WIDTH, pHeight: Globals.HEIGHT, pFullScreen: Globals.FULLSCREEN, pVSync: Globals.VSYNC) {
            
            Canvas canvas = new Canvas(800, 600);

            //add some content
            canvas.graphics.FillRectangle(new SolidBrush(Color.Red), new Rectangle(0, 0, 400, 300));
            canvas.graphics.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(400, 0, 400, 300));
            canvas.graphics.FillRectangle(new SolidBrush(Color.Yellow), new Rectangle(0, 300, 400, 300));
            canvas.graphics.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(400, 300, 400, 300));
            //add canvas to display list
            AddChild(canvas);
        }
        public static void Main(string[] args) {
            new Program().Start();
        }
    }

    public class TestSerialize {
        public int width;
        public int height;
        public bool fullscreen;
        public bool vsync;
        public TestSerialize() {}
        public TestSerialize(int width, int height, bool fullscreen, bool vsync) {
            this.width = width;
            this.height = height;
            this.fullscreen = fullscreen;
            this.vsync = vsync;
        }

        public void SaveJson(string FileName) {
            using (var writer = new StreamWriter(FileName)) {
                var json = JsonConvert.SerializeObject(this);
                Console.WriteLine(json);
                writer.Write(json);
                writer.Flush();
            }
        }
        public void SaveXML(string FileName)
        {
            using (var writer = new System.IO.StreamWriter(FileName))
            {
                var serializer = new XmlSerializer(this.GetType());
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
        public static TestSerialize LoadXML(string FileName)
        {
            using (var stream = System.IO.File.OpenRead(FileName))
            {
                var serializer = new XmlSerializer(typeof(TestSerialize));
                return serializer.Deserialize(stream) as TestSerialize;
            }
        }
    }
}