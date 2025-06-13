using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.operators {
    public class ChamferPolygon {
        public static MeshBuilder apply(MeshBuilder builder, float amount) {
            var chamferBuilder = new MeshBuilder();
            buildMiddle(builder, amount, chamferBuilder);
            buildChamfer(builder, amount, chamferBuilder);

            return chamferBuilder;
        }

        private static void buildChamfer(MeshBuilder builder, float amount, MeshBuilder chamferBuilder) {
            foreach (var edge in builder.edges()) {
                var edgeFaces = new List<List<int>>();
                foreach (var face in builder.faces) {
                    var found = true;
                    foreach (var edgeIndex in edge) {
                        if (!face.Contains(edgeIndex)) {
                            found = false;
                        }
                    }

                    if (found) {
                        edgeFaces.Add(face);
                    }
                }

                var faceVertices = new List<Vector3>();
                var edgeCenter = Vector3.zero;
                foreach (var edgeVertice in edge) {
                    edgeCenter += builder.vectorList[edgeVertice] / edge.Count;
                }

                var aligned = true;
                foreach (var edgeFace in edgeFaces) {
                    var faceCenter = Vector3.zero;
                    foreach (var verticeIndex in edgeFace) {
                        var vertice = builder.vectorList[verticeIndex];
                        faceCenter += vertice / edgeFace.Count;
                    }

                    var firstIndexInFace = edgeFace.IndexOf(edge[0]);
                    var secondIndexInFace = edgeFace.IndexOf(edge[1]);
                    aligned = true;
                    if (secondIndexInFace == 0 && firstIndexInFace == edgeFace.Count - 1) {
                    }
                    else if (firstIndexInFace == 0 && secondIndexInFace == edgeFace.Count - 1) {
                        aligned = false;
                    }
                    else if (firstIndexInFace > secondIndexInFace) {
                        aligned = false;
                    }

                    if (!aligned) {
                        faceVertices.Add(
                            Vector3.Lerp(builder.vectorList[edge[0]], faceCenter, amount));
                        faceVertices.Add(
                            Vector3.Lerp(builder.vectorList[edge[1]], faceCenter, amount));
                        faceVertices.Add(Vector3.Lerp(Vector3.zero,
                            builder.vectorList[edge[1]], 1 - amount));
                    }
                    else {
                        faceVertices.Add(
                            Vector3.Lerp(builder.vectorList[edge[1]], faceCenter, amount));
                        faceVertices.Add(
                            Vector3.Lerp(builder.vectorList[edge[0]], faceCenter, amount));
                        faceVertices.Add(Vector3.Lerp(Vector3.zero,
                            builder.vectorList[edge[0]], 1 - amount));
                    }
                }

                var newFaceCenter = Vector3.zero;

                foreach (var vertice in faceVertices) {
                    newFaceCenter += vertice / faceVertices.Count;
                }

                var newFace = new List<int>();
                foreach (var vertice in faceVertices) {
                    newFace.Add(chamferBuilder.vectorList.Count);
                    chamferBuilder.vectorList.Add(vertice);
                }

                chamferBuilder.faces.Add(newFace);
            }
        }

        private static void buildMiddle(MeshBuilder builder, float amount, MeshBuilder chamferBuilder) {
            foreach (var face in builder.faces) {
                var faceCenter = Vector3.zero;
                foreach (var verticeIndex in face) {
                    var vertice = builder.vectorList[verticeIndex];
                    faceCenter += vertice / face.Count;
                }

                var middleFace = new List<int>();
                chamferBuilder.faces.Add(middleFace);

                foreach (var verticeIndex in face) {
                    middleFace.Add(chamferBuilder.vectorList.Count);
                    chamferBuilder.vectorList.Add(
                        Vector3.Lerp(builder.vectorList[verticeIndex], faceCenter, amount));
                }
            }
        }
    }
}