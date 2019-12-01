using System.Collections.Generic;
using System.Linq;

namespace GXPEngine.Core {
    public class Mesh {
        private List<Vector3> vertices;
        private List<Vector2> uvs;
        private List<int> indices;
        
        private Vector3[] vertexArray;
        private Vector2[] uvArray;
        private int[] indexArray;

        private Texture2D texture;

        public List<Vector3> Vertices {
            get => vertices;
            set {
                vertices = new List<Vector3>(value);
                vertexArray = value.ToArray();
            }
        }
        public List<Vector2> Uvs {
            get => uvs;
            set {
                uvs = new List<Vector2>(value);
                uvArray = value.ToArray();
            }
        }
        public List<int> Indices {
            get => indices;
            set {
                indices = new List<int>(value);
                indexArray = value.ToArray();
            }
        }
        public Vector3[] VertexArray => vertexArray;
        public Vector2[] UvArray => uvArray;
        public int[] IndexArray => indexArray;

        public Texture2D Texture {
            get => texture;
            set => texture = value;
        }

        public Mesh(Texture2D texture) : this(texture, new List<Vector3>(), new List<Vector2>(), new List<int>()) {
        }

        public Mesh(string texturePath) : this(Texture2D.GetInstance(texturePath, true), new List<Vector3>(), new List<Vector2>(), new List<int>()) {
        }
        
        public Mesh(Texture2D texture, Vector3[] vertices, Vector2[] uvs, int[] indices) : this(texture, new List<Vector3>(vertices), new List<Vector2>(uvs), new List<int>(indices)){
        }
        
        public Mesh(string texturePath, Vector3[] vertices, Vector2[] uvs, int[] indices) : this(Texture2D.GetInstance(texturePath, true), new List<Vector3>(vertices), new List<Vector2>(uvs), new List<int>(indices)){
        }

        public Mesh(Texture2D texture, List<Vector3> vertices, List<Vector2> uvs, List<int> indices) {
            this.texture = texture;
            Vertices = vertices;
            Uvs = uvs;
            Indices = indices;
        }
        
        public void Clear() {
            vertices.Clear();
            uvs.Clear();
            indices.Clear();
            vertexArray = null;
            uvArray = null;
            indexArray = null;
        }
    }
}