using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unicessing
{
    public class UShape
    {
        public enum CloseType
        {
            NONE,
            CLOSE,
        }

        public enum ShapeKind
        {
            NONE,
            POINT,
            LINE,
            RECT,
            ELLIPSE,
            //ARC,
            SPHERE,
            BOX,
            CUSTOM,
        }

        public enum VertexType
        {
            NONE,
            POINTS,
            LINES,
            LINE_STRIP,
            CURVE_LINES,
            CURVE_LINE_STRIP,
            TRIANGLES,
            TRIANGLE_FAN,
            TRIANGLE_STRIP,
            QUADS,
            QUAD_STRIP,
            MESH,
        }

        VertexType vertexType = VertexType.NONE;
        ShapeKind shapeKind = ShapeKind.NONE;

        UGraphics g;
        Mesh mesh;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();
        MaterialPropertyBlock materialPB = new MaterialPropertyBlock();
        int wireframeSubmeshIndex = -1;

        public UGraphics.UStyle style;
        bool isEnableStyle = false;
        public bool isClockwise = false;
        public int curveDivision = 16;

        List<UShape> children = new List<UShape>();

        public UShape(UGraphics g)
        {
            this.g = g;
            style = new UGraphics.UStyle(g);
            vertexType = VertexType.NONE;
            shapeKind = ShapeKind.NONE;
            isClockwise = !g.isP5;
        }

        #region Processing Members
        public void enableStyle() { isEnableStyle = true; }
        public void disableStyle() { isEnableStyle = false; }

        private UGraphics.UStyle nowStyle
        {
            get { return isEnableStyle ? style : g.getStyle(); }
        }

        public Color nowColor
        {
            get { return isFillShape ? nowStyle.fillColor : nowStyle.strokeColor; }
        }

        public bool isFillShape { get { return vertexType >= VertexType.TRIANGLES; } }
        public bool is2DShape { get { return shapeKind <= ShapeKind.ELLIPSE; } }

        public void beginShape(VertexType type = VertexType.LINE_STRIP)
        {
            shapeKind = ShapeKind.NONE;
            vertexType = type;
            vertices.Clear();
            uv.Clear();
            triangles.Clear();
            colors.Clear();
            //mesh = null;
            clearMesh();
            wireframeSubmeshIndex = -1;
        }

        #region Wireframe
        private void setStrokeMeshIndices(MeshTopology topology, int subMeshIndex = 0)
        {
            Debug.Assert(!isFillShape);
            int[] indices = new int[vertices.Count];
            for (int i = 0; i < vertices.Count; i++) { indices[i] = i; }
            mesh.colors = colors.ToArray();
            mesh.SetIndices(indices, topology, subMeshIndex);
        }

        void setWireframeMeshIndices(int[] indices, int subMeshIndex, MeshTopology topology = MeshTopology.Lines)
        {
            if (indices == null) indices = mesh.GetIndices(0);
            mesh.SetIndices(indices, topology, subMeshIndex);
            wireframeSubmeshIndex = subMeshIndex;
        }

        void setWireframeMeshTriangles(int[] triangles, int subMeshIndex)
        {
            Debug.Assert(isFillShape);

            int[] indices = new int[2 * triangles.Length];
            int i = 0;
            for (int t = 0; t < triangles.Length; t += 3)
            {
                indices[i++] = triangles[t];
                indices[i++] = triangles[t + 1];
                indices[i++] = triangles[t + 1];
                indices[i++] = triangles[t + 2];
                indices[i++] = triangles[t + 2];
                indices[i++] = triangles[t];
            }

            setWireframeMeshIndices(indices, subMeshIndex);
        }
        #endregion

        public void endShape(int closeType) { endShape((CloseType)closeType); }
        public void endShapeClose() { endShape(CloseType.CLOSE); }
        public void endShape(CloseType closeType = CloseType.NONE)
        {
            shapeKind = ShapeKind.CUSTOM;

            if (closeType == CloseType.CLOSE)
            {
                Debug.Assert(vertices.Count > 0);
                if(vertexType == VertexType.CURVE_LINES)
                {
                }
                else if (vertexType == VertexType.CURVE_LINE_STRIP)
                {
                    curveVertex(vertices[0].x, vertices[0].y, vertices[0].z);
                    curveVertex(vertices[0].x, vertices[0].y, vertices[0].z);
                }
                else vertex(vertices[0].x, vertices[0].y, vertices[0].z, uv[0].x, uv[0].y);
            }

            switch (vertexType)
            {
                case VertexType.CURVE_LINES:
                    ConvertCurveLines();
                    break;
                case VertexType.CURVE_LINE_STRIP:
                    ConvertCurveLineStrip();
                    break;
            }

            clearMesh();
            mesh.vertices = vertices.ToArray();

            switch (vertexType)
            {
                case VertexType.POINTS:
                    mesh.name = "Points";
                    setStrokeMeshIndices(MeshTopology.Points);
                    break;
                case VertexType.LINES:
                    mesh.name = "Lines";
                    setStrokeMeshIndices(MeshTopology.Lines);
                    break;
                case VertexType.LINE_STRIP:
                    mesh.name = "LineStrip";
                    setStrokeMeshIndices(MeshTopology.LineStrip);
                    break;
                case VertexType.CURVE_LINES:
                    mesh.name = "CurveLines";
                    setStrokeMeshIndices(MeshTopology.Lines);
                    break;
                case VertexType.CURVE_LINE_STRIP:
                    mesh.name = "CurveLineStrip";
                    setStrokeMeshIndices(MeshTopology.LineStrip);
                    break;
                case VertexType.TRIANGLES:
                    mesh.name = "Triangles";
                    for (int i = 2; i < vertices.Count; i += 3) {
                        triangles.Add(i);
                        triangles.Add(i - 2);
                        triangles.Add(i - 1);
                    }
                    break;
                case VertexType.TRIANGLE_FAN:
                    mesh.name = "TriangleFan";
                    for (int i = 2; i < vertices.Count; i++)
                    {
                        triangles.Add(0);
                        triangles.Add(i - 1);
                        triangles.Add(i);
                    }
                    break;
                case VertexType.TRIANGLE_STRIP:
                    mesh.name = "TriangleStrip";
                    int d = 1;
                    for (int i = 2; i < vertices.Count; i++)
                    {
                        triangles.Add(i - d * 2);
                        triangles.Add(i - 1);
                        d = (i + 1) & 1;
                        triangles.Add(i - d * 2);
                    }
                    break;
                case VertexType.QUADS:
                    mesh.name = "Quads";
                    for (int i = 0; i < vertices.Count; i += 4)
                    {
                        triangles.Add(i + 0);
                        triangles.Add(i + 1);
                        triangles.Add(i + 2);
                        triangles.Add(i + 2);
                        triangles.Add(i + 3);
                        triangles.Add(i + 0);
                    }
                    break;
                case VertexType.QUAD_STRIP:
                    mesh.name = "QuadStrip";
                    for (int i = 3; i < vertices.Count; i += 2)
                    {
                        triangles.Add(i - 3);
                        triangles.Add(i - 2);
                        triangles.Add(i - 1);
                        triangles.Add(i - 1);
                        triangles.Add(i - 2);
                        triangles.Add(i + 0);
                    }
                    break;
            }

            if (isFillShape) {
                mesh.subMeshCount = 2;
                var tri = triangles.ToArray();
                setMeshTriangles(tri, uv.ToArray(), 0);
                mesh.RecalculateNormals();
                setWireframeMeshTriangles(tri, 1);
            } else { mesh.uv = uv.ToArray(); }

            recalc(mesh, false);

            vertices.Clear();
            uv.Clear();
            triangles.Clear();
            colors.Clear();
        }

        public void vertex(float x, float y, float z = 0.0f) { vertex(x, y, z, 0.0f, 0.0f); }
        public void vertex(float x, float y, float u, float v) { vertex(x, y, 0.0f, u, v); }
        public void vertex(float x, float y, float z, float u, float v) { vertex(new Vector3(x, y, z), Vector2.zero); }
        public void vertex(Vector3 pos) { vertex(pos, Vector2.zero); }
        public void vertex(Vector3 pos, Vector2 uv)
        {
            vertices.Add(pos);
            this.uv.Add(uv);
            colors.Add(nowColor);
        }
        public int vertexCount { get { return vertices.Count; } }

        public void curveVertex(float x, float y, float z = 0.0f) { curveVertex(new Vector3(x, y, z)); }
        public void curveVertex(Vector3 pos)
        {
            UGraphics.assert(vertexType == VertexType.CURVE_LINES || vertexType == VertexType.CURVE_LINE_STRIP);
            vertices.Add(pos);
            colors.Add(nowColor);
        }

        private void ConvertCurveLines()
        {
            Vector3[] av = vertices.ToArray();
            //Vector2[] auv = uv.ToArray();
            Color[] ac = colors.ToArray();
            vertices.Clear(); uv.Clear(); colors.Clear();
            for (int i=0; i + 3 < av.Length; i += 4)
            {
                catmullRomVertex(av[i], av[i + 1], av[i + 2], av[i + 3], ac[i], ac[i + 1]);
            }
        }

        private void ConvertCurveLineStrip()
        {
            Vector3[] av = vertices.ToArray();
            //Vector2[] auv = uv.ToArray();
            Color[] ac = colors.ToArray();
            vertices.Clear(); uv.Clear(); colors.Clear();
            for (int i = 0; i + 3 < av.Length; i++)
            {
                catmullRomVertex(av[i], av[i + 1], av[i + 2], av[i + 3], ac[i], ac[i + 1]);
            }
        }

        private void catmullRomVertex(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Color col1, Color col2)
        {
            if (curveDivision <= 3) curveDivision = 3;

            float invDiv = 1.0f / curveDivision;
            float t = 0;
            Vector3 pos = new Vector3();
            Color col = Color.white;
            for (int i = 0; i <= curveDivision; i++, t += invDiv)
            {
                if (vertexType == VertexType.CURVE_LINES && i > 1)
                {
                    vertices.Add(pos);
                    //this.uv.Add(uv);
                    colors.Add(col);
                }
                pos.x = catmullRom(v0.x, v1.x, v2.x, v3.x, t);
                pos.y = catmullRom(v0.y, v1.y, v2.y, v3.y, t);
                pos.z = catmullRom(v0.z, v1.z, v2.z, v3.z, t);
                vertices.Add(pos);
                //this.uv.Add(uv);
                col = Color.Lerp(col1, col2, t);
                colors.Add(col);
            }
        }

        private float catmullRom(float p0, float p1, float p2, float p3, float t)
        {
            float v0 = (p2 - p0) * 0.5f;
            float v1 = (p3 - p1) * 0.5f;
            float t2 = t * t;
            float t3 = t2 * t;
            return ((p1 - p2) * 2.0f + v0 + v1) * t3 + ((p2 - p1) * 3.0f - 2.0f * v0 - v1) * t2 + v0 * t + p1;
        }
        #endregion

        #region Processing Extra Members
        public void fill() { style.isFill = true; }
        public void fill(float gray, float alpha = 255) { fill(g.color(gray, alpha)); }
        public void fill(float r, float g, float b, float a = 255) { fill(this.g.color(r, g, b, a)); }
        public void fill(Color col) { style.fillColor = col; style.isFill = true; }
        public void noFill() { style.isFill = false; }
        public void stroke() { style.isStroke = true; }
        public void stroke(float gray, float alpha = 255) { stroke(g.color(gray, alpha)); }
        public void stroke(float r, float g, float b, float a = 255) { stroke(this.g.color(r, g, b, a)); }
        public void stroke(Color col) {
            style.strokeColor = col;
            style.isStroke = true;
            if (ShapeKind.NONE!=shapeKind && !isFillShape && mesh.vertexCount > 0)
            {
                Color32[] colors = new Color32[mesh.vertexCount];
                Color32 col32 = col;
                for (int i = 0; i<colors.Length; i++) colors[i] = col32;
                mesh.colors32 = colors;
            }
        }
        public void noStroke() { style.isStroke = false; }
        public void lights() { style.isLighting = true; }
        public void noLights() { style.isLighting = false; }

        public void texture(Texture tex) { style.texture = tex; }
        public void blendMode(UMaterials.BlendMode mode) { style.blendMode = mode; }

        public void addChild(UShape shape)
        {
            if (this == shape) { Debug.LogError("addChild() can not make myself a child."); return; }
            children.Add(shape);
        }
        public void removeChild(UShape shape) { children.Remove(shape); }
        public void removeChild(int index) { children.RemoveAt(index); }

        public void saveAssetDB(Mesh mesh)
        {
#if UNITY_EDITOR
            var dbAssetName = "Assets/Unicessing/Models/" + mesh.name + ".asset";
            //AssetDatabase.DeleteAsset(dbAssetName);
            AssetDatabase.CreateAsset(mesh, dbAssetName);
            AssetDatabase.SaveAssets();
#endif
        }

        public void recalc(Mesh mesh, bool isUpdateNormal = true)
        {
			if (isUpdateNormal && isFillShape) {
				mesh.RecalculateNormals();
			}
            //mesh.RecalculateBounds();
            // mesh.Optimize();
        }

        private void clearMesh()
        {
            if (!mesh) mesh = new Mesh();
            else mesh.Clear();
        }

        private Material findMaterial()
        {
			if (isFillShape) {
                if(nowStyle.isLighting) return UMaterials.getFillMaterial(nowStyle.blendMode);
                else return UMaterials.getFillUnlitMaterial(nowStyle.blendMode);
            } else {
                return UMaterials.getStrokeMaterial(nowStyle.blendMode);
			}
        }

        public void draw(Matrix4x4 matrix, Color col, Texture tex, int layer, Camera camera) // for Update
        {
            Material material = findMaterial();
            materialPB.Clear();
            materialPB.SetColor("_Color", col);
            if (isFillShape && tex != null) { materialPB.SetTexture("_MainTex", tex); }

            UGraphics.UStyle nstyle = nowStyle;
            for (int i=0; i<mesh.subMeshCount; i++)
            {
                if(i == wireframeSubmeshIndex)
                {
                    if (!nstyle.isStroke) continue;
                    material = UMaterials.getFillUnlitMaterial(nstyle.blendMode);
                    materialPB.Clear();
                    materialPB.SetColor("_Color", nstyle.strokeColor);
                }
                else if (isFillShape && !nstyle.isFill) continue;
                Graphics.DrawMesh(mesh, matrix, material, layer, camera, i, materialPB);
            }

            foreach(UShape shape in children) { shape.draw(matrix, col, tex, layer, camera); }
        }

        public void draw(Matrix4x4 matrix, Camera camera) // for Update
        {
            UGraphics.UStyle style = nowStyle;
            draw(matrix, nowColor, style.texture, style.layer, camera);
        }

        public void drawNow(Matrix4x4 matrix, Color col) // for OnPostRender / OnRenderObject
        {
            Material material = findMaterial();
            material.color = col;
            material.SetPass(0);
            Graphics.DrawMeshNow(mesh, matrix);
            foreach (UShape shape in children) { shape.drawNow(matrix, col); }
        }
        #endregion

        #region Create Shapes
        public Mesh createPoint(float x, float y, float z)
		{
			shapeKind = ShapeKind.LINE;
			vertexType = VertexType.LINES;

            clearMesh();
            mesh.name = "Point";

			mesh.vertices = new Vector3[]{
				new Vector3 (x, y, z),
			};

			int[] indices = { 0 };
			mesh.SetIndices(indices, MeshTopology.Points, 0);

			//recalc(mesh);
			return mesh;
		}

		public Mesh createLine(float x1, float y1, float z1, float x2, float y2, float z2)
		{
			shapeKind = ShapeKind.LINE;
			vertexType = VertexType.LINES;

            clearMesh();
            mesh.name = "Line";

			mesh.vertices = new Vector3[]{
				new Vector3 (x1, y1, z1),
				new Vector3 (x2, y2, z2),
			};

			int[] indices = { 0, 1 };
			mesh.SetIndices(indices, MeshTopology.Lines, 0);

			//recalc(mesh);
			return mesh;
		}

		public Mesh createRect(float x, float y, float w, float h)
        {
            shapeKind = ShapeKind.RECT;
			vertexType = VertexType.MESH;

            mesh = createQuad(x, y, x + w, y, x + w, y + h, x, y + h); // �����
            //mesh = createQuad(x, y - h, x + w, y - h, x + w, y, x, y); // �����
            mesh.name = "Rect";
            return mesh;
        }

        public Mesh createQuad(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4, bool isClockwise = true)
        {
            shapeKind = ShapeKind.RECT;
			vertexType = VertexType.MESH;

            clearMesh();
            mesh.name = "Quad";

            mesh.vertices = new Vector3[]{
                new Vector3 (x1, y1, 0.0f),
                new Vector3 (x2, y2, 0.0f),
                new Vector3 (x3, y3, 0.0f),
                new Vector3 (x4, y4, 0.0f),
            };

            var uv = new Vector2[]{
                new Vector2 (0.0f, 0.0f),
                new Vector2 (1.0f, 0.0f),
                new Vector2 (1.0f, 1.0f),
                new Vector2 (0.0f, 1.0f),
            };

            int[] triangles = new int[] { 2, 1, 0, 0, 3, 2 };
            int[] wireIndices = new int[] { 2, 1, 0, 3, 2 };

            mesh.subMeshCount = 2;
            setMeshTriangles(triangles, uv, 0);
            recalc(mesh);
            //setWireframeMeshTriangles(triangles, 1);
            setWireframeMeshIndices(wireIndices, 1, MeshTopology.LineStrip);

            return mesh;
        }

        private void setMeshTriangles(int[] triangles, Vector2[] uv, int subMesh)
        {
            bool isRev = isFillShape && !isClockwise;
            if (isRev) { System.Array.Reverse(triangles); }
            mesh.SetTriangles(triangles, subMesh);

            if (uv!=null)
            {
                if (isRev) { for (int i=0; i<uv.Length; i++) { uv[i].y = 1 - uv[i].y; } }
                mesh.uv = uv;
            }
        }

        public Mesh createEllipse(float x, float y, float w, float h)
        {
            shapeKind = ShapeKind.ELLIPSE;
			vertexType = VertexType.MESH;

            w *= 0.5f; h *= 0.5f;
            //x += w; y += h;

            clearMesh();
            mesh.name = "Ellipse";

            const int div = 32;
            Vector3[] vertices = new Vector3[div + 2];
            Vector2[] uv = new Vector2[div + 2];
            int[] triangles = new int[(div + 1) * 3];
            int[] wireIndices = new int[div + 1];
            float d = 0.0f;
            float dstep = (Mathf.PI * 2.0f) / (float)div;

            vertices[0].Set(x, y, 0);
            uv[0].Set(0.5f, 0.5f);
            for (int i = 1; i <= div + 1; i++)
            {
                float rx = Mathf.Cos(d);
                float ry = Mathf.Sin(d);
                vertices[i].Set(x + rx * w, y + ry * h, 0);
                uv[i].Set(0.5f + rx * 0.5f, 0.5f + ry * 0.5f);
                d += dstep;
                if (i < div + 1)
                {
                    int ti = (i - 1) * 3;
                    triangles[ti] = 0;
                    triangles[ti + 1] = i + 1;
                    triangles[ti + 2] = i;
                }
                wireIndices[i - 1] = i;
            }
            mesh.vertices = vertices;
            mesh.subMeshCount = 2;
            setMeshTriangles(triangles, uv, 0);
            recalc(mesh);
            //setWireframeMeshTriangles(triangles, 1);
            setWireframeMeshIndices(wireIndices, 1, MeshTopology.LineStrip);

            return mesh;
        }

        public Mesh createBox(float w, float h, float d)
        {
            shapeKind = ShapeKind.BOX;
			vertexType = VertexType.MESH;

            clearMesh();
            mesh.name = "Box";

            w *= 0.5f; h *= 0.5f; d *= 0.5f;

            Vector3 p0 = new Vector3(-w, -h, d);
            Vector3 p1 = new Vector3(w, -h, d);
            Vector3 p2 = new Vector3(w, -h, -d);
            Vector3 p3 = new Vector3(-w, -h, -d);
            Vector3 p4 = new Vector3(-w, h, d);
            Vector3 p5 = new Vector3(w, h, d);
            Vector3 p6 = new Vector3(w, h, -d);
            Vector3 p7 = new Vector3(-w, h, -d);
            Vector3[] vertices = new Vector3[]
            {
	            p0, p1, p2, p3, // Bottom
	            p7, p4, p0, p3, // Left
	            p4, p5, p1, p0, // Front
	            p6, p7, p3, p2, // Back
	            p5, p6, p2, p1, // Right
	            p7, p6, p5, p4  // Top
            };

            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;
            Vector3[] normales = new Vector3[]
            {
	            down, down, down, down,     // Bottom
 	            left, left, left, left,     // Left
	            front, front, front, front, // Front
	            back, back, back, back,     // Back
	            right, right, right, right, // Right
	            up, up, up, up              // Top
            };

            Vector2 _00 = new Vector2(0f, 0f);
            Vector2 _10 = new Vector2(1f, 0f);
            Vector2 _01 = new Vector2(0f, 1f);
            Vector2 _11 = new Vector2(1f, 1f);
            Vector2[] uv = new Vector2[]
            {
	            _11, _01, _00, _10, // Bottom
	            _11, _01, _00, _10, // Left
	            _11, _01, _00, _10, // Front
	            _11, _01, _00, _10, // Back
	            _11, _01, _00, _10, // Right
	            _11, _01, _00, _10, // Top
            };

            int[] triangles = new int[]
            {
	            // Bottom
	            3, 1, 0,
                3, 2, 1,
 
	            // Left
	            3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	            // Front
	            3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	            // Back
	            3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	            // Right
	            3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	            // Top
	            3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
                3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
            };

            int[] wireIndices = new int[]
            {
                0, 1, 1, 2, 2, 3, 3, 0,
                0, 23, 1, 22, 2, 21, 3, 20,
                20, 21, 21, 22, 22, 23, 23, 20,
            };

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.subMeshCount = 2;
            setMeshTriangles(triangles, uv, 0);
            //setWireframeMeshTriangles(triangles, 1);
            setWireframeMeshIndices(wireIndices, 1);

            recalc(mesh, false);
            return mesh;
        }

        public Mesh createSphere(float w, float h, float d, int ures = 30, int vres = 30)
        {
            shapeKind = ShapeKind.SPHERE;
			vertexType = VertexType.MESH;

            clearMesh();
            mesh.name = "Sphere";

            w *= 0.5f; h *= 0.5f; d *= 0.5f;

            int nbLong = ures; // |
            int nbLat = vres;  // -
            if (nbLong < 3) nbLong = 3;
            if (nbLat < 3) nbLat = 3;

            Vector3[] vertices = new Vector3[(nbLong + 1) * nbLat + 2];
            float _pi = Mathf.PI;
            float _2pi = _pi * 2f;
            vertices[0] = Vector3.up * h;
            for (int lat = 0; lat < nbLat; lat++)
            {
                float a1 = _pi * (float)(lat + 1) / (nbLat + 1);
                float sin1 = Mathf.Sin(a1);
                float cos1 = Mathf.Cos(a1);

                for (int lon = 0; lon <= nbLong; lon++)
                {
                    float a2 = _2pi * (float)(lon == nbLong ? 0 : lon) / nbLong;
                    float sin2 = Mathf.Sin(a2);
                    float cos2 = Mathf.Cos(a2);

                    vertices[lon + lat * (nbLong + 1) + 1] = new Vector3(sin1 * cos2 * w, cos1 * h, sin1 * sin2 * d);
                }
            }
            vertices[vertices.Length - 1] = Vector3.up * -h;

            Vector3[] normales = new Vector3[vertices.Length];
            for (int n = 0; n < vertices.Length; n++)
            {
                normales[n] = vertices[n].normalized;
            }

            Vector2[] uv = new Vector2[vertices.Length];
            uv[0] = Vector2.up;
            uv[uv.Length - 1] = Vector2.zero;
            for (int lat = 0; lat < nbLat; lat++)
            {
                for (int lon = 0; lon <= nbLong; lon++)
                {
                    uv[lon + lat * (nbLong + 1) + 1] = new Vector2((float)lon / nbLong, 1f - (float)(lat + 1) / (nbLat + 1));
                }
            }

            int nbFaces = vertices.Length;
            int nbTriangles = nbFaces * 2;
            int nbIndexes = nbTriangles * 3;
            int[] triangles = new int[nbIndexes];

            //Top Cap
            int i = 0;
            for (int lon = 0; lon < nbLong; lon++)
            {
                triangles[i++] = lon + 2;
                triangles[i++] = lon + 1;
                triangles[i++] = 0;
            }

            //Middle
            for (int lat = 0; lat < nbLat - 1; lat++)
            {
                for (int lon = 0; lon < nbLong; lon++)
                {
                    int current = lon + lat * (nbLong + 1) + 1;
                    int next = current + nbLong + 1;

                    triangles[i++] = current;
                    triangles[i++] = current + 1;
                    triangles[i++] = next + 1;

                    triangles[i++] = current;
                    triangles[i++] = next + 1;
                    triangles[i++] = next;
                }
            }

            //Bottom Cap
            for (int lon = 0; lon < nbLong; lon++)
            {
                triangles[i++] = vertices.Length - 1;
                triangles[i++] = vertices.Length - (lon + 2) - 1;
                triangles[i++] = vertices.Length - (lon + 1) - 1;
            }

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.subMeshCount = 2;
            setMeshTriangles(triangles, uv, 0);
            setWireframeMeshTriangles(triangles, 1);

            recalc(mesh, false);
            return mesh;
        }
        #endregion
    }
}