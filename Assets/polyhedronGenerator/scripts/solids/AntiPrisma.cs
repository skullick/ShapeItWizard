using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.solids {
    public class AntiPrisma {
        public static MeshBuilder generate(float radius, int edges, float height) {
            var builder = new MeshBuilder();

            var topFace = new List<int>();
            var bottomFace = new List<int>();

            builder.faces.Add(topFace);
            builder.faces.Add(bottomFace);

            for (var i = 0; i < edges; i++) {
                var point1 = Quaternion.AngleAxis(360f / edges * i, Vector3.up) * Vector3.forward * radius -
                            (Vector3.up * height / 2f);
                
                var point2 = Quaternion.AngleAxis(360f / edges * (i+1), Vector3.up) * Vector3.forward * radius -
                            (Vector3.up * height / 2f);
                
                var upperPoint1 = Quaternion.AngleAxis(360f / edges * i, Vector3.up) * Vector3.forward * radius +
                            (Vector3.up * height / 2f);
                
                var upperPoint2 = Quaternion.AngleAxis(360f / edges * (i+1), Vector3.up) * Vector3.forward * radius +
                            (Vector3.up * height / 2f);

                var point3 = Vector3.Lerp(upperPoint1, upperPoint2, 0.5f);
                var upperPoint3 = Quaternion.AngleAxis(360f / edges * (i+2), Vector3.up) * Vector3.forward * radius +
                            (Vector3.up * height / 2f);
                var point4 = Vector3.Lerp(upperPoint2, upperPoint3, 0.5f);
               
                var sideFace = new List<int>();
                var sideFace2 = new List<int>();
                sideFace.Add(builder.vectorList.Count);
                bottomFace.Add(builder.vectorList.Count);
                builder.vectorList.Add(point1);
                
                sideFace.Add(builder.vectorList.Count);
                builder.vectorList.Add(point2);
                
                topFace.Add(builder.vectorList.Count);
                sideFace.Add(builder.vectorList.Count);
                sideFace2.Add(builder.vectorList.Count);
                builder.vectorList.Add(point3);
                
                
                sideFace2.Add(builder.vectorList.Count);
                builder.vectorList.Add(point2);
                
                sideFace2.Add(builder.vectorList.Count);
                builder.vectorList.Add(point4);

                builder.faces.Add(sideFace);
                builder.faces.Add(sideFace2);
            }
            bottomFace.Reverse();

            return builder;
        }
    }
}