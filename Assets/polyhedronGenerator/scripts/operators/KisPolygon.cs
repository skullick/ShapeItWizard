using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.operators {
    public class KisPolygon {
        public static MeshBuilder apply(MeshBuilder builder, float amount, float radius, int degree) {
            var kisBuilder = new MeshBuilder();
            foreach (var face in builder.faces) {
                if (degree != 0 && face.Count != degree) {
                    var newFace = new List<int>();
                    kisBuilder.faces.Add(newFace);
                    foreach (var verticeIndex in face) {
                        newFace.Add(kisBuilder.vectorList.Count);
                        kisBuilder.vectorList.Add(builder.vectorList[verticeIndex]);
                    }

                    continue;
                }

                var faceCenter = Vector3.zero;
                foreach (var verticeIndex in face) {
                    var vertice = builder.vectorList[verticeIndex];
                    faceCenter += vertice / face.Count;
                }

                var middleIndex = kisBuilder.vectorList.Count;
                kisBuilder.vectorList.Add(faceCenter.normalized * amount * radius);

                for (var verticeIndex = 0; verticeIndex < face.Count - 1; verticeIndex++) {
                    var vertice = builder.vectorList[face[verticeIndex]];
                    kisBuilder.faces.Add(new List<int>
                        { kisBuilder.vectorList.Count, kisBuilder.vectorList.Count + 1, middleIndex });
                    kisBuilder.vectorList.Add(vertice);
                }

                kisBuilder.faces.Add(new List<int> { kisBuilder.vectorList.Count, middleIndex + 1, middleIndex });
                kisBuilder.vectorList.Add(builder.vectorList[face[^1]]);
            }

            return kisBuilder;
        }
    }
}