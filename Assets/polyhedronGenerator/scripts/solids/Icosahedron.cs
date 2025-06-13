using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.solids {
    public class Icosahedron  {

        public static MeshBuilder generate(int radius) {
            var builder = new MeshBuilder();
            var a = radius/2f;
            var c = (1 + Mathf.Sqrt(5)) / 2 * a;

            builder.vectorList.Add(new Vector3(-a , c , 0).normalized * radius);
            builder.vectorList.Add(new Vector3(a , c , 0).normalized * radius);
            builder.vectorList.Add(new Vector3(-a , -c , 0).normalized * radius);
            builder.vectorList.Add(new Vector3(a , -c , 0).normalized * radius);

            builder.vectorList.Add(new Vector3(0, -a , c ).normalized * radius);
            builder.vectorList.Add(new Vector3(0, a , c ).normalized * radius);
            builder.vectorList.Add(new Vector3(0, -a , -c ).normalized * radius);
            builder.vectorList.Add(new Vector3(0, a , -c ).normalized * radius);

            builder.vectorList.Add(new Vector3(c , 0, -a ).normalized * radius);
            builder.vectorList.Add(new Vector3(c , 0, a ).normalized * radius);
            builder.vectorList.Add(new Vector3(-c , 0, -a ).normalized * radius);
            builder.vectorList.Add(new Vector3(-c , 0, a ).normalized * radius);

            builder.faces.Add(new List<int> { 0, 11, 5 });
            builder.faces.Add(new List<int> { 0, 5, 1 });
            builder.faces.Add(new List<int> { 0, 1, 7 });
            builder.faces.Add(new List<int> { 0, 7, 10 });
            builder.faces.Add(new List<int> { 0, 10, 11 });

            builder.faces.Add(new List<int> { 1, 5, 9 });
            builder.faces.Add(new List<int> { 5, 11, 4 });
            builder.faces.Add(new List<int> { 11, 10, 2 });
            builder.faces.Add(new List<int> { 10, 7, 6 });
            builder.faces.Add(new List<int> { 7, 1, 8 });

            builder.faces.Add(new List<int> { 3, 9, 4 });
            builder.faces.Add(new List<int> { 3, 4, 2 });
            builder.faces.Add(new List<int> { 3, 2, 6 });
            builder.faces.Add(new List<int> { 3, 6, 8 });
            builder.faces.Add(new List<int> { 3, 8, 9 });

            builder.faces.Add(new List<int> { 4, 9, 5 });
            builder.faces.Add(new List<int> { 2, 4, 11 });
            builder.faces.Add(new List<int> { 6, 2, 10 });
            builder.faces.Add(new List<int> { 8, 6, 7 });
            builder.faces.Add(new List<int> { 9, 8, 1 });
            return builder;
        }
    }
}