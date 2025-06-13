using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.solids {
    public class Tetrahedon {
        public static MeshBuilder generate(int radius) {
            var builder = new MeshBuilder();

            builder.vectorList.Add(new Vector3(-1 , 0, -1f/Mathf.Sqrt(2)).normalized*radius);
            builder.vectorList.Add(new Vector3(+1 , 0, -1f/Mathf.Sqrt(2)).normalized*radius);
            builder.vectorList.Add(new Vector3(0 , -1, 1f/Mathf.Sqrt(2)).normalized*radius);
            builder.vectorList.Add(new Vector3(0 , +1, 1f/Mathf.Sqrt(2)).normalized*radius);


            builder.faces.Add(new List<int> {
                0,1,2
            });
              builder.faces.Add(new List<int> { 0, 3, 1 });
            builder.faces.Add(new List<int> { 3,0,2 });
            builder.faces.Add(new List<int> { 1,3,2 });
            return builder;
        }
    }
}