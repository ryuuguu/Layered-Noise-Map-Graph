using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AnthillPlan.LayeredNM {
    public class HeightMapToMesh {
        static public void Height2Tris(in float[,] aMap, GameObject mapHolder, float scale, float xzScale, bool unshare) { 
            int size = aMap.GetLength(0);
            var verts = new List<Vector3>();
            var tris = new List<int>();
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    //Add each new vertex in the plane
                    verts.Add(new Vector3(i * xzScale, aMap[i , j ], j * xzScale)*scale);
                    
                    if (i == 0 || j == 0) continue;
                    //Adds the index of the three vertices in order to make up each of the two tris
                    tris.Add(size * (i - 1) + j - 1); //Top left A
                    tris.Add(size * (i - 1) + j); //Top right B
                    tris.Add(size * i + j); //Bottom right  C

                    tris.Add(size * i + j); //Bottom right D
                    tris.Add(size * i + j - 1); //Top right E
                    tris.Add(size * (i - 1) + j - 1); //Top left  F
                }
            }

            Vector3[] vertices;
            int[] triangles;
            Vector2[] uv;
            if (unshare) {
                vertices = new Vector3[tris.Count];
                triangles = new int[tris.Count];
                uv = new Vector2[tris.Count];

                for (var v = 0; v < tris.Count; v++) {
                    vertices[v] = verts[tris[v]];
                    triangles[v] = v;
                    uv[v] = new Vector2(vertices[v].x/size,vertices[v].y/size);
                }
            } else {
                vertices = verts.ToArray();
                triangles = tris.ToArray();
                uv = new Vector2[vertices.Length];
                for (var v = 0; v < vertices.Length; v++) {
                    uv[v] = new Vector2(vertices[v].x/size,vertices[v].z/size);
                }
            }
            
            var meshFilter = mapHolder.GetComponent<MeshFilter>();
            var meshCollider = mapHolder.GetComponent<MeshCollider>();
            Mesh procMesh = new Mesh();
            if (verts.Count > 65535) {
                procMesh.indexFormat = IndexFormat.UInt32;
            }
            
            procMesh.vertices = vertices;
            procMesh.triangles = triangles;
            procMesh.uv = uv;
            procMesh.RecalculateBounds();
            procMesh.RecalculateNormals();
            procMesh.RecalculateTangents();
            meshFilter.sharedMesh = procMesh;
            var meshId = procMesh.GetInstanceID();
            Physics.BakeMesh(meshId, false);
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
    }
}