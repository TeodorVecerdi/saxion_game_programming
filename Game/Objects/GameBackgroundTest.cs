using System;
using System.Collections.Generic;
using Game.TileDefinition;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class GameBackgroundTest : GameObject {
        private readonly int height;
        private readonly Mesh mesh;
        private readonly float tsize;
        private readonly int width;

        public GameBackgroundTest(int width, int height, float tsize, string texturePath) : this(width, height, tsize, Texture2D.GetInstance(texturePath, true)) { }

        public GameBackgroundTest(int width, int height, float tsize, Texture2D texture) {
            if (game == null) throw new Exception("GameObjects cannot be created before creating a Game instance.");
            mesh = new Mesh(texture);
            this.width = width;
            this.height = height;
            this.tsize = tsize;
            GenerateMesh();
        }

        private void GenerateMesh() {
            var vertList = new List<Vector3>();
            var uvList = new List<Vector2>();
            var indiceList = new List<int>();
            var squareCount = 0;
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++) {
                var _y = y + 1;
                vertList.Add(new Vector3(x * tsize, _y * tsize, 0));
                vertList.Add(new Vector3(x * tsize + tsize, _y * tsize, 0));
                vertList.Add(new Vector3(x * tsize + tsize, _y * tsize - tsize, 0));
                vertList.Add(new Vector3(x * tsize, _y * tsize - tsize, 0));

                indiceList.Add(squareCount * 4);
                indiceList.Add(squareCount * 4 + 1);
                indiceList.Add(squareCount * 4 + 3);
                indiceList.Add(squareCount * 4 + 1);
                indiceList.Add(squareCount * 4 + 2);
                indiceList.Add(squareCount * 4 + 3);
                var uv = Rand.Range(0, 5) switch {
                    0 => World.Dirt.SpriteUV, 1 => World.Grass.SpriteUV, 2 => World.Sand.SpriteUV, 3 => World.Stone.SpriteUV, _ => World.Water.SpriteUV
                };
                uvList.Add(new Vector2(uv.xa, uv.yb));
                uvList.Add(new Vector2(uv.xb, uv.yb));
                uvList.Add(new Vector2(uv.xb, uv.ya));
                uvList.Add(new Vector2(uv.xa, uv.ya));
                squareCount++;
            }

            mesh.Vertices = vertList;
            mesh.Indices = indiceList;
            mesh.Uvs = uvList;
        }

        protected override void RenderSelf(GLContext glContext) {
            if (game == null) return;
            BlendMode.NORMAL.enable();
            glContext.SetColor(255, 255, 255, 255);
            glContext.DrawMesh(mesh);
            glContext.SetColor(1, 1, 1, 1);
            BlendMode.NORMAL.enable();
        }
    }
}