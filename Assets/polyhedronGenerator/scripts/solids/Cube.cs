using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.solids {
    public class Cube  {

        public static MeshBuilder generate(int radius) {
            var builder = new MeshBuilder();
            var a = radius/2f;

            builder.vectorList.Add(new Vector3(-a,-a,-a).normalized * radius);
            builder.vectorList.Add(new Vector3(-a,-a,a).normalized * radius);
            builder.vectorList.Add(new Vector3(a,-a,a).normalized * radius);
            builder.vectorList.Add(new Vector3(a,-a,-a).normalized * radius);

            builder.vectorList.Add(new Vector3(-a,a,-a).normalized * radius);
            builder.vectorList.Add(new Vector3(-a,a,a).normalized * radius);
            builder.vectorList.Add(new Vector3(a,a,a).normalized * radius);
            builder.vectorList.Add(new Vector3(a,a,-a).normalized * radius);


            builder.faces.Add(new List<int> { 3,2,1,0 });
            builder.faces.Add(new List<int> { 0, 4, 7,3 });
            builder.faces.Add(new List<int> { 4, 5, 6,7 });
            builder.faces.Add(new List<int> { 2,6,5,1 });
            builder.faces.Add(new List<int> { 0, 1, 5,4 });
            builder.faces.Add(new List<int> { 2, 3, 7,6 });
            return builder;
        }
    }
}