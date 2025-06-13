using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.operators {
    public class QuintoPolygon {
        public static MeshBuilder apply(MeshBuilder builder, float amount) {
            var quintoBuilder = new MeshBuilder();

            foreach (var face in builder.faces) {
                var faceCenter = Vector3.zero;
                foreach (var verticeIndex in face) {
                    var vertice = builder.vectorList[verticeIndex];
                    faceCenter += vertice / face.Count;
                }

                var middleFace = new List<int>();
                quintoBuilder.faces.Add(middleFace);
                    buildQuintoFace(builder, amount, face, 
                        face[^1],
                        face[0],
                        face[1],
                        quintoBuilder, middleFace, faceCenter);
                for (var faceIndex = 1; faceIndex < face.Count - 1; faceIndex++) {
                    buildQuintoFace(builder, amount, face, 
                        face[faceIndex-1],
                        face[faceIndex],
                        face[faceIndex+1],
                        quintoBuilder, middleFace, faceCenter);
                }
                    buildQuintoFace(builder, amount, face, 
                        face[^2],
                        face[^1],
                        face[0],
                        quintoBuilder, middleFace, faceCenter);
            }

            return quintoBuilder;
        }

        private static void buildQuintoFace(MeshBuilder builder, float amount, List<int> face,
            int faceIndexPrevious,
            int faceIndex,
            int faceIndexNext,
            MeshBuilder quintoBuilder, List<int> middleFace, Vector3 faceCenter) {
            var previousEdgeMiddle = Vector3.Lerp(
                builder.vectorList[faceIndexPrevious],
                builder.vectorList[faceIndex], 0.5f
            );
            var nextEdgeMiddle = Vector3.Lerp(
                builder.vectorList[faceIndexNext],
                builder.vectorList[faceIndex], 0.5f
            );
            var newFace = new List<int>();
            
            newFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(previousEdgeMiddle);
            
            newFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(builder.vectorList[faceIndex]);
            
            newFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(nextEdgeMiddle);
            
            newFace.Add(quintoBuilder.vectorList.Count);
            middleFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(Vector3.Lerp(nextEdgeMiddle,faceCenter,amount));
            
            newFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(Vector3.Lerp(previousEdgeMiddle,faceCenter,amount));
            quintoBuilder.faces.Add(newFace);
        }
    }
}