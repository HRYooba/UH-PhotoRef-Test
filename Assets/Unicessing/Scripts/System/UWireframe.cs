using UnityEngine;
using System.Collections;

namespace Unicessing
{
    public class UWireframe : MonoBehaviour
    {
        public Mesh mesh;

        void Awake()
        {
            RemakeMesh();
        }

        public void RemakeMesh(Mesh baseMesh = null)
        {
            if (!baseMesh)
            {
                MeshFilter filter = gameObject.GetComponent<MeshFilter>();
                if (filter) { baseMesh = filter.sharedMesh; }
            }

            if (baseMesh)
            {
                mesh = new Mesh();
                mesh.vertices = baseMesh.vertices;
                mesh.subMeshCount = baseMesh.subMeshCount;
                for (int i = 0; i < baseMesh.subMeshCount; i++)
                {
                    mesh.SetIndices(makeTriangleIndices(baseMesh.GetTriangles(i)), MeshTopology.Lines, i);
                }
                //mesh.RecalculateBounds();
                //mesh.Optimize();
            }
        }

        int[] makeTriangleIndices(int[] triangles)
        {
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
            return indices;
        }
    }
}