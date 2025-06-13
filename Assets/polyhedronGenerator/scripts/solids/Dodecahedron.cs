using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.solids {
    public class Dodecahedron {
        public static MeshBuilder generate(int radius) {
            var builder = new MeshBuilder();

            builder.vectorList.Add(new Vector3(1.21412f, 0f, 1.58931f).normalized * radius);
            builder.vectorList.Add(new Vector3(0.375185f, 1.1547f, 1.58931f).normalized * radius);
            builder.vectorList.Add(new Vector3(-0.982247f, 0.713644f, 1.58931f).normalized * radius);
            builder.vectorList.Add(new Vector3(-0.982247f, -0.713644f, 1.58931f).normalized * radius);
            builder.vectorList.Add(new Vector3(0.375185f, -1.1547f, 1.58931f).normalized * radius);
            builder.vectorList.Add(new Vector3(1.96449f, 0, 0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(0.607062f, 1.86835f, 0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(-1.58931f, 1.1547f, 0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(-1.58931f, -1.1547f, 0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(0.607062f, -1.86835f, 0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(1.58931f, 1.1547f, -0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(-0.607062f, 1.86835f, -0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(-1.96449f, 0, -0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(-0.607062f, -1.86835f, -0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(1.58931f, -1.1547f, -0.375185f).normalized * radius);
            builder.vectorList.Add(new Vector3(0.982247f, 0.713644f, -1.58931f).normalized * radius);
            builder.vectorList.Add(new Vector3(-0.375185f, 1.1547f, -1.58931f).normalized * radius);
            builder.vectorList.Add(new Vector3(-1.21412f, 0, -1.58931f).normalized * radius);
            builder.vectorList.Add(new Vector3(-0.375185f, -1.1547f, -1.58931f).normalized * radius);
            builder.vectorList.Add(new Vector3(0.982247f, -0.713644f, -1.58931f).normalized * radius);

            builder.faces.Add(new List<int> { 0, 1, 2, 3, 4 });
            builder.faces.Add(new List<int> { 0, 5, 10, 6, 1 });
            builder.faces.Add(new List<int> { 1, 6, 11, 7, 2 });
            builder.faces.Add(new List<int> { 2, 7, 12, 8, 3 });
            builder.faces.Add(new List<int> { 3, 8, 13, 9, 4 });
            builder.faces.Add(new List<int> { 4, 9, 14, 5, 0 });
            builder.faces.Add(new List<int> { 15, 10, 5, 14, 19 });
            builder.faces.Add(new List<int> { 16, 11, 6, 10, 15 });
            builder.faces.Add(new List<int> { 17, 12, 7, 11, 16 });
            builder.faces.Add(new List<int> { 18, 13, 8, 12, 17 });
            builder.faces.Add(new List<int> { 19, 14, 9, 13, 18 });
            builder.faces.Add(new List<int> { 19, 18, 17 ,16,15});

            return builder;
        }
    }
}