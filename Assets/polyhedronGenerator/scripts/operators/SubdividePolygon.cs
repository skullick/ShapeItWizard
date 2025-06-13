using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.operators {
    public class SubdividePolygon : MonoBehaviour {
        public static MeshBuilder apply(MeshBuilder builder, float amount, int degree) {
            var subdivideBuilder = new MeshBuilder();

            foreach (var face in builder.faces) {
                if (degree != 0 && face.Count != degree) {
                    var newFace = new List<int>();
                    subdivideBuilder.faces.Add(newFace);
                    foreach (var verticeIndex in face) {
                        newFace.Add(subdivideBuilder.vectorList.Count);
                        subdivideBuilder.vectorList.Add(builder.vectorList[verticeIndex]);
                    }

                    continue;
                }

                if (face.Count == 3) {
                    subdivide(builder, face, subdivideBuilder);
                }
                else {
                    triangulate(face,builder,subdivideBuilder);
                }
            }

            return subdivideBuilder;
        }

        private static void subdivide(MeshBuilder builder, List<int> face, MeshBuilder subdivideBuilder) {
            var first = builder.vectorList[face[0]];
            var second = builder.vectorList[face[1]];
            var third = builder.vectorList[face[2]];
            subdivideBuilder.vectorList.Add(first);
            subdivideBuilder.vectorList.Add(second);
            subdivideBuilder.vectorList.Add(third);
            subdivideBuilder.vectorList.Add(Vector3.Lerp(first, second, 0.5f));
            subdivideBuilder.vectorList.Add(Vector3.Lerp(second, third, 0.5f));
            subdivideBuilder.vectorList.Add(Vector3.Lerp(first, third, 0.5f));
            var index = subdivideBuilder.vectorList.Count - 6;

            subdivideBuilder.faces.Add(new List<int> { index, index + 3, index + 5 });
            subdivideBuilder.faces.Add(new List<int> { index + 3, index + 1, index + 4 });
            subdivideBuilder.faces.Add(new List<int> { index + 3, index + 4, index + 5 });
            subdivideBuilder.faces.Add(new List<int> { index + 5, index + 4, index + 2 });
        }

        private static void triangulate(
            List<int> face, MeshBuilder builder, MeshBuilder subDivideBuilder) {
            var middle = Vector3.zero;
            foreach (var vertice in face) {
                middle += builder.vectorList[vertice] / face.Count;
            }

            for (var faceIndex = 0; faceIndex < face.Count - 1; faceIndex++) {
                pushNewTriangle(subDivideBuilder,
                    builder.vectorList[face[faceIndex]],
                    builder.vectorList[face[faceIndex + 1]],
                    middle
                );
            }

            pushNewTriangle(subDivideBuilder,
                builder.vectorList[face[^1]],
                builder.vectorList[face[0]],
                middle
            );
        }

        private static void pushNewTriangle(MeshBuilder subdivideBuilder,
            Vector3 first, Vector3 second, Vector3 third) {
            var newFace = new List<int>();
            newFace.Add(subdivideBuilder.vectorList.Count);
            subdivideBuilder.vectorList.Add(first);

            newFace.Add(subdivideBuilder.vectorList.Count);
            subdivideBuilder.vectorList.Add(second);

            newFace.Add(subdivideBuilder.vectorList.Count);
            subdivideBuilder.vectorList.Add(third);
            subdivideBuilder.faces.Add(newFace);
        }
    }
}