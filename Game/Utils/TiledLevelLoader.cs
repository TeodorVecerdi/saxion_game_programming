
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace Game.Utils {
   /* 
    Licensed under the Apache License, Version 2.0
    https://xmltocsharp.azurewebsites.net/
    http://www.apache.org/licenses/LICENSE-2.0
    */
	[XmlRoot(ElementName="tileset")]
	public class TiledLevelTileset {
		[XmlAttribute(AttributeName="firstgid")]
		public string Firstgid { get; set; }
		[XmlAttribute(AttributeName="source")]
		public string Source { get; set; }
	}

	[XmlRoot(ElementName="data")]
	public class TiledLevelData {
		[XmlAttribute(AttributeName="encoding")]
		public string Encoding { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName="layer")]
	public class TiledLevelLayer {
		[XmlElement(ElementName="data")]
		public TiledLevelData TiledLevelData { get; set; }
		[XmlAttribute(AttributeName="id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName="name")]
		public string Name { get; set; }
		[XmlAttribute(AttributeName="width")]
		public string Width { get; set; }
		[XmlAttribute(AttributeName="height")]
		public string Height { get; set; }
	}

	[XmlRoot(ElementName="map")]
	public class TiledLevelMap {
		[XmlElement(ElementName="tileset")]
		public TiledLevelTileset TiledLevelTileset { get; set; }
		[XmlElement(ElementName="layer")]
		public TiledLevelLayer TiledLevelLayer { get; set; }
		[XmlAttribute(AttributeName="version")]
		public string Version { get; set; }
		[XmlAttribute(AttributeName="tiledversion")]
		public string Tiledversion { get; set; }
		[XmlAttribute(AttributeName="orientation")]
		public string Orientation { get; set; }
		[XmlAttribute(AttributeName="renderorder")]
		public string Renderorder { get; set; }
		[XmlAttribute(AttributeName="compressionlevel")]
		public string Compressionlevel { get; set; }
		[XmlAttribute(AttributeName="width")]
		public int Width { get; set; }
		[XmlAttribute(AttributeName="height")]
		public int Height { get; set; }
		[XmlAttribute(AttributeName="tilewidth")]
		public int Tilewidth { get; set; }
		[XmlAttribute(AttributeName="tileheight")]
		public int Tileheight { get; set; }
		[XmlAttribute(AttributeName="infinite")]
		public int Infinite { get; set; }
		[XmlAttribute(AttributeName="nextlayerid")]
		public string Nextlayerid { get; set; }
		[XmlAttribute(AttributeName="nextobjectid")]
		public string Nextobjectid { get; set; }
	}

	public static class TiledLevelLoader {
		public static TiledLevelMap LoadTiledMap(string path) {
			using (var stream = File.OpenRead(path)) {
				var serializer = new XmlSerializer(typeof(TiledLevelMap));
				return serializer.Deserialize(stream) as TiledLevelMap;
			}
		}
	}
}