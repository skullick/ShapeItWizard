using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.solids {
    public class Octahedron {
        public static MeshBuilder generate(int radius) {
            var builder = new MeshBuilder();
            var a = radius / 2f;

            builder.vectorList.Add(new Vector3(-a, -a, 0));
            builder.vectorList.Add(new Vector3(a, -a, 0));
            builder.vectorList.Add(new Vector3(a, a, 0));
            builder.vectorList.Add(new Vector3(-a, a, 0));

            builder.vectorList.Add(new Vector3(0, 0, -radius / Mathf.Sqrt(2)));
            builder.vectorList.Add(new Vector3(0, 0, radius / Mathf.Sqrt(2)));


            builder.faces.Add(new List<int> { 0, 4, 1 });
            builder.faces.Add(new List<int> { 1, 5, 0 });
            
            builder.faces.Add(new List<int> { 1, 4, 2 });
            builder.faces.Add(new List<int> { 2, 5, 1 });
            
            builder.faces.Add(new List<int> { 2, 4, 3 });
            builder.faces.Add(new List<int> { 3, 5, 2 });
            builder.faces.Add(new List<int> { 3, 4, 0 });
            builder.faces.Add(new List<int> { 0, 5, 3 });
            
            return builder;
        }
    }
}