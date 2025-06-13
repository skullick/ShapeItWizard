using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace polyhedronGenerator.scripts.operators {
    public class DualPolygon {
        public static MeshBuilder apply(MeshBuilder builder) {
            var dualBuilder = new MeshBuilder();
            var neighbourFaces = new List<int>();
            for (var i = 0; i < builder.vectorList.Count; i++) {
                findNeighbourFaces(builder, i, neighbourFaces);

                var centers = calcFaceCenters(builder, neighbourFaces);
                var newFaceCenter = Vector3.zero;
                foreach (var vertice in centers) {
                    newFaceCenter += vertice / centers.Count;
                }

                var currentVector = builder.vectorList[i];
                var newFace = new List<int>();
                foreach (var center in orderAntiClockwise(centers, newFaceCenter, currentVector)) {
                    newFace.Add(dualBuilder.vectorList.Count);
                    dualBuilder.vectorList.Add(center);
                }

                neighbourFaces.Clear();

                dualBuilder.faces.Add(newFace);
            }

            return dualBuilder;
        }

        private static void findNeighbourFaces(MeshBuilder builder, int verticeIndex, List<int> neighbourFaces) {
            var faceIndex = 0;
            foreach (var face in builder.faces) {
                foreach (var vertice in face) {
                    if (vertice == verticeIndex) {
                        neighbourFaces.Add(faceIndex);
                    }
                }

                faceIndex++;
            }
        }

        private static List<Vector3> calcFaceCenters(MeshBuilder builder, List<int> neighbourFaces) {
            var centers = new List<Vector3>();
            foreach (var neighbour in neighbourFaces) {
                var center = Vector3.zero;
                var neighbourFace = builder.faces[neighbour];
                foreach (var vertice in neighbourFace) {
                    center += builder.vectorList[vertice] / neighbourFace.Count;
                }

                centers.Add(center);
            }

            return centers;
        }

        private static List<Vector3> orderAntiClockwise(List<Vector3> vectors, Vector3 centerOfFace, Vector3 faceNormal) {
            
            return vectors.OrderBy((v) =>
                360 - Vector3.SignedAngle(v-centerOfFace, vectors[0]-centerOfFace, faceNormal)
            ).ToList();
        }
    }
}