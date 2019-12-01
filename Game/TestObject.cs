using System;
using System.Collections.Generic;
using GXPEngine;
using GXPEngine.Core;
using GXPEngine.Graphics;

namespace GXPEngineTest {
    public class TestObject : GameObject {
        private Mesh mesh;
        private int width;
        private int height;
        private float tsize;

        public TestObject(int width, int height, float tsize, string texturePath) : this(width, height, tsize, Texture2D.GetInstance(texturePath, true)) {
        }

        public TestObject(int width, int height, float tsize, Texture2D texture) {
            if (game == null) {
                throw new Exception("GameObjects cannot be created before creating a Game instance.");
            }
            
            mesh = new Mesh(texture);

            this.width = width;
            this.height = height;
            this.tsize = tsize;
            GenerateMesh();
        }

        private void GenerateMesh() {
            List<Vector3> vertList = new List<Vector3>();
            List<Vector2> uvList = new List<Vector2>();
            List<int> indiceList = new List<int>();
            int squareCount = 0;
            for (var x = 0; x < width; x++) {
                for (var y = 0; y < height; y++) {
                    var _y = y + 1;
                    vertList.Add(new Vector3(x*tsize, _y*tsize, 0));
                    vertList.Add(new Vector3(x*tsize + tsize, _y*tsize, 0));
                    vertList.Add(new Vector3(x*tsize + tsize, _y*tsize - tsize, 0));
                    vertList.Add(new Vector3(x*tsize, _y*tsize - tsize, 0));

                    indiceList.Add(squareCount * 4);
                    indiceList.Add(squareCount * 4 + 1);
                    indiceList.Add(squareCount * 4 + 3);
                    indiceList.Add(squareCount * 4 + 1);
                    indiceList.Add(squareCount * 4 + 2);
                    indiceList.Add(squareCount * 4 + 3);
                    
                    uvList.Add(new Vector2(0f, 1f));
                    uvList.Add(new Vector2(1f, 1f));
                    uvList.Add(new Vector2(1f, 0f));
                    uvList.Add(new Vector2(0f, 0f));
                    squareCount++;
                }
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