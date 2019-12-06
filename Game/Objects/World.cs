using System;
using System.Collections.Generic;
using Game.Utils;
using GXPEngine;
using GXPEngine.Core;

namespace Game {
    public class World : GameObject {
        private readonly int height;
        private readonly Mesh mesh;
        private readonly float tsize;
        private readonly int width;
        private Level level;
        public bool Invalidated = false;

        public World(Level level, string texturePath) : this(level, Texture2D.GetInstance(texturePath, true)) { }

        public World(Level level, Texture2D texture) {
            if (game == null) throw new Exception("GameObjects cannot be created before creating a Game instance.");
            mesh = new Mesh(texture);
            width = level.Width;
            height = level.Height;
            tsize = Globals.TILE_SIZE;
            this.level = level;
            GenerateMesh();
        }

        private void GenerateMesh() {
            var vertList = new List<Vector3>();
            var uvList = new List<Vector2>();
            var indiceList = new List<int>();
            var squareCount = 0;
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++) {
                if(Level.Tileset.Tiles[level.Tiles[x, y]].Name == "Miner") continue;
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
                var uv = Level.Tileset.Tiles[level.Tiles[x, y]].UV;
                uvList.Add(new Vector2(uv.left, uv.bottom));
                uvList.Add(new Vector2(uv.right, uv.bottom));
                uvList.Add(new Vector2(uv.right, uv.top));
                uvList.Add(new Vector2(uv.left, uv.top));
                squareCount++;
            }

            mesh.Vertices = vertList;
            mesh.Indices = indiceList;
            mesh.Uvs = uvList;
        }

        public void Update() {
            for (int x = 0; x < level.Width; x++) {
                for (int y = 0; y < level.Height; y++) {
                    if (level.Tiles[x, y] == TileType.Boulder && y < level.Height-1 && level.Tiles[x, y+1] == TileType.Empty) {
                        level.Tiles[x, y + 1] = TileType.Boulder;
                        level.Tiles[x, y] = TileType.Empty;
                        Invalidated = true;
                    }
                }
            }
            if(Invalidated) RebuildMesh();
        }

        public void RebuildMesh() {
            mesh.Clear();
            GenerateMesh();
        }

        protected override void RenderSelf(GLContext glContext) {
            if (game == null) return;
            glContext.SetColor(255, 255, 255, 255);
            glContext.DrawMesh(mesh);
            glContext.SetColor(255, 255, 255, 255);
        }
    }
}