using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.IPAddress;

namespace Assets
{
    public static class Util
    {
        public static short ToBigEndian(this short value) => HostToNetworkOrder(value);
        public static int ToBigEndian(this int value) => HostToNetworkOrder(value);
        public static long ToBigEndian(this long value) => HostToNetworkOrder(value);
        public static short FromBigEndian(this short value) => NetworkToHostOrder(value);
        public static int FromBigEndian(this int value) => NetworkToHostOrder(value);
        public static long FromBigEndian(this long value) => NetworkToHostOrder(value);

        public static long GetArraySize(Array arr)
        {
            return arr.LongLength * Marshal.SizeOf(arr.GetType().GetElementType());
        }

        // Vertex position in 0-1 UV space
        // 4 points in the list for the square made of two triangles:
        //  0 *--* 1
        //    | /|
        //    |/ |
        //  3 *--* 2
        //public static Mesh CreateMesh(Vector2[] points)
        //{
        //    List<Vector2> ptList = new List<Vector2>(points);
        //    ptList.RemoveAt(ptList.Count - 1);

        //    int[] tris = new int[ptList.Count]; // Every 3 ints represents a triangle
        //    Triangulator tr = new Triangulator(ptList);
        //    int[] indices = tr.Triangulate();

        //    // Create the Vector3 vertices
        //    Vector3[] vertices = new Vector3[ptList.Count];
        //    for (int i = 0; i < vertices.Length; i++)
        //    {
        //        vertices[i] = new Vector3(ptList[i].x, 0, ptList[i].y);
        //    }

        //    Vector3[] normals = new Vector3[vertices.Length];
        //    for (int i = 0; i < normals.Length; i++)
        //    {
        //        normals[i] = Vector3.up;
        //    }

        //    Mesh mesh = new Mesh();
        //    mesh.vertices = vertices;
        //    mesh.triangles = indices;
        //    mesh.normals = normals;
        //    mesh.RecalculateNormals();
        //    mesh.RecalculateBounds();
        //    return mesh;
        //}


        public static Mesh CreateMesh(Vector2[] points)
        {
            List<List<Vector2>> ptLists = splitArray(points);

            List<Vector3> vers = new List<Vector3>();
            List<int> inds = new List<int>();
            List<Vector3> nors = new List<Vector3>();

            for (int i = 0; i < ptLists.Count; i++)
            {
                MeshData meshData = getMeshData(ptLists[i]);
                vers.AddRange(new List<Vector3>(meshData.vertices));
                inds.AddRange(new List<int>(meshData.indices));
                nors.AddRange(new List<Vector3>(meshData.normals));
            }
            //foreach (List<Vector2> item in ptLists)
            //{
            //    MeshData meshData = getMeshData(item);
            //    vers.AddRange(new List<Vector3>(meshData.vertices));
            //    inds.AddRange(new List<int>(meshData.indices));
            //    nors.AddRange(new List<Vector3>(meshData.normals));
            //}
            Mesh mesh = new Mesh();
            mesh.vertices = vers.ToArray();
            mesh.triangles = inds.ToArray();
            mesh.normals = nors.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
        static List<List<Vector2>> splitArray(Vector2[] points)
        {
            List<List<Vector2>> splits = new List<List<Vector2>>();
            int index = 0;
            Vector2 tempV2 = points[index];
            for (int i = 1; i < points.Length; i++)
            {
                if (points[i]==tempV2)
                {
                    List<Vector2> list = new List<Vector2>();
                    for (int j = index; j < i; j++)
                    {
                        list.Add(points[j]);
                    }
                    splits.Add(list);

                    if ((i + 1) < points.Length)
                    {
                        index = i + 1;
                        tempV2 = points[index];
                    }
                }
            }
            return splits;
        }

        static MeshData getMeshData(List<Vector2> ptList)
        {
            MeshData meshData = new MeshData();

            Triangulator tr = new Triangulator(ptList);
            int[] indices = tr.Triangulate();

            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[ptList.Count];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(ptList[i].x, 0, ptList[i].y);
            }

            Vector3[] normals = new Vector3[vertices.Length];
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = Vector3.up;
            }

            meshData.vertices = vertices;
            meshData.indices = indices;
            meshData.normals = normals;
            return meshData;
        }

        class MeshData
        {
            public Vector3[] vertices;
            public int[] indices;
            public Vector3[] normals;
        }
    }
}
