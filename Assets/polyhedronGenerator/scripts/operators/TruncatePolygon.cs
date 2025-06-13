using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace polyhedronGenerator.scripts.operators {
    public class TruncatePolygon {
        public static MeshBuilder apply(MeshBuilder builder, float amount) {
            var truncateBuilder = new MeshBuilder();
            buildFaces(builder, amount, truncateBuilder);
            buildEdges(builder, truncateBuilder, amount);
            return truncateBuilder;
        }

        private static void buildEdges(MeshBuilder builder, MeshBuilder truncateBuilder, float amount) {
            var index = 0;
            foreach (var vertice in builder.vectorList) {
                var vertices = new List<Vector3>();
                var newFace = new List<int>();
                var edges = findEdges(builder, index);
                
                foreach (var edge in orderAntiClockwise(edges,vertice,vertice)) {
                    var newVertice = Vector3.Lerp(vertice, edge,  amount);
                    vertices.Add(newVertice);
                }

                foreach (var orderedVertice in vertices) {
                    newFace.Add(truncateBuilder.vectorList.Count);
                    truncateBuilder.vectorList.Add(orderedVertice);
                }

                index++;
                truncateBuilder.faces.Add(newFace);
            }
        }

        private static void buildFaces(MeshBuilder builder, float amount, MeshBuilder truncateBuilder) {
            foreach (var face in builder.faces) {
                var newFace = new List<int>();
                var previousFaceNormal = Vector3.Cross(
                    builder.vectorList[face[1]]- builder.vectorList[face[0]],
                    builder.vectorList[face[2]]- builder.vectorList[face[0]]
                    ).normalized;
                var newVertices = new List<Vector3>();
                var center = Vector3.zero;
                for (var verticeIndex = 0; verticeIndex < face.Count; verticeIndex++) {
                    var vertice = builder.vectorList[face[verticeIndex]];
                    center += vertice / face.Count();
                    var neighbourIndex = verticeIndex - 1;
                    if (neighbourIndex < 0) {
                        neighbourIndex = face.Count - 1;
                    }

                    var neighbour1 = builder.vectorList[face[neighbourIndex]];
                    neighbourIndex = verticeIndex + 1;
                    if (neighbourIndex == face.Count) {
                        neighbourIndex = 0;
                    }

                    var neighbour2 = builder.vectorList[face[neighbourIndex]];
                    newVertices.Add(Vector3.Lerp(vertice, neighbour1, amount));
                    newVertices.Add(Vector3.Lerp(vertice, neighbour2, amount));
                }
                foreach (var vertice in orderAntiClockwise(newVertices,center,previousFaceNormal)) {
                    newFace.Add(truncateBuilder.vectorList.Count);
                    truncateBuilder.vectorList.Add(vertice);
                }

                truncateBuilder.faces.Add(newFace);
            }
        }

        private static List<Vector3> orderAntiClockwise(List<Vector3> vectors, Vector3 centerOfFace, Vector3 faceNormal) {
            
            return vectors.OrderBy((v) =>
                360 - Vector3.SignedAngle(v-centerOfFace, vectors[0]-centerOfFace, faceNormal)
            ).ToList();
        }

        private static List<Vector3> findEdges(MeshBuilder builder, int verticeIndex) {
            var edges = new List<Vector3>();
            var possibleEdges = new Dictionary<int, int>();
            foreach (var face in builder.faces) {
                var isNeighbour = false;
                foreach (var vertice in face) {
                    if (vertice == verticeIndex) {
                        isNeighbour = true;
                    }
                }

                if (isNeighbour) {
                    foreach (var vertice in face) {
                        if (vertice != verticeIndex) {
                            possibleEdges[vertice] = possibleEdges.GetValueOrDefault(vertice, 0) + 1;
                        }
                    }
                }
            }

            foreach (var possibleEdge in possibleEdges) {
                if (possibleEdge.Value > 1) {
                    edges.Add(builder.vectorList[possibleEdge.Key]);
                }
            }

            return edges;
        }
    }
}