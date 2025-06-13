using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.solids {
    public class Prisma {
        public static MeshBuilder generate(float radius, int faces, float height) {
            var builder = new MeshBuilder();

            var topFace = new List<int>();
            var bottomFace = new List<int>();

            builder.faces.Add(topFace);
            builder.faces.Add(bottomFace);

            Vector3 point;
            for (var i = 0; i < faces-1; i++) {
                var sideFace = new List<int>();
                point = Quaternion.AngleAxis(360f / faces * i, Vector3.up) * Vector3.forward * radius -
                            (Vector3.up * height / 2f);
                bottomFace.Add(builder.vectorList.Count);
                sideFace.Add(builder.vectorList.Count);
                builder.vectorList.Add(point);
                
                point = Quaternion.AngleAxis(360f / faces * i, Vector3.up) * Vector3.forward * radius +
                        (Vector3.up * height / 2f);
                sideFace.Add(builder.vectorList.Count);
                topFace.Add(builder.vectorList.Count);
                builder.vectorList.Add(point);
                sideFace.Add(builder.vectorList.Count + 1);
                sideFace.Add(builder.vectorList.Count);
                sideFace.Reverse();
                
                builder.faces.Add(sideFace);
            }

            var lastFace = new List<int>();
            lastFace.AddRange(new List<int>{builder.vectorList.Count,0,1,builder.vectorList.Count+1});
            builder.faces.Add(lastFace);
            point = Quaternion.AngleAxis(360f / faces * (faces-1), Vector3.up) * Vector3.forward * radius -
                        (Vector3.up * height / 2f);
                bottomFace.Add(builder.vectorList.Count);
            builder.vectorList.Add(point);
            point = Quaternion.AngleAxis(360f / faces * (faces-1), Vector3.up) * Vector3.forward * radius +
                    (Vector3.up * height / 2f);
            topFace.Add(builder.vectorList.Count);
            builder.vectorList.Add(point);

            bottomFace.Reverse();

            return builder;
        }
    }
}