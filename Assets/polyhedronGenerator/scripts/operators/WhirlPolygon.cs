using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.operators {
    public class WhirlPolygon {
        public static MeshBuilder apply(MeshBuilder builder, float amount) {
            var whirlBuilder = new MeshBuilder();

            foreach (var face in builder.faces) {
                var faceCenter = Vector3.zero;
                foreach (var verticeIndex in face) {
                    var vertice = builder.vectorList[verticeIndex];
                    faceCenter += vertice / face.Count;
                }

                var middleFace = new List<int>();
                whirlBuilder.faces.Add(middleFace);
                    buildWhirlFace(builder, amount, face, 
                        face[^1],
                        face[0],
                        face[1],
                        face[2],
                        whirlBuilder, middleFace,faceCenter);
                for (var faceIndex = 1; faceIndex < face.Count - 2; faceIndex++) {
                    buildWhirlFace(builder, amount, face, 
                        face[faceIndex-1],
                        face[faceIndex],
                        face[faceIndex+1],
                        face[faceIndex+2],
                        whirlBuilder, middleFace,faceCenter);
                }
                    buildWhirlFace(builder, amount, face, 
                        face[^3],
                        face[^2],
                        face[^1],
                        face[0],
                        whirlBuilder, middleFace,faceCenter);
                    buildWhirlFace(builder, amount, face, 
                        face[^2],
                        face[^1],
                        face[0],
                        face[1],
                        whirlBuilder, middleFace,faceCenter);
            }

            return whirlBuilder;
        }

        private static void buildWhirlFace(MeshBuilder builder, float amount, List<int> face,
            int faceIndexPrevious,
            int faceIndex,
            int faceIndexNext,
            int faceIndexNextNext,
            MeshBuilder quintoBuilder, List<int> middleFace,
            Vector3 faceCenter) {
            var previousEdgeMiddle = Vector3.Lerp(
                builder.vectorList[faceIndexPrevious],
                builder.vectorList[faceIndex], amount
            );
            var previousEdgeMid= Vector3.Lerp(
                builder.vectorList[faceIndexPrevious],
                builder.vectorList[faceIndex], 1-amount
            );
            var nextEdgeMiddle = Vector3.Lerp(
                builder.vectorList[faceIndexNext],
                builder.vectorList[faceIndex], 1f - amount
            );
            var nextNextEdgeMiddle = Vector3.Lerp(
                builder.vectorList[faceIndexNext],
                builder.vectorList[faceIndexNextNext],  amount
            );
            var vertice = builder.vectorList[faceIndex];
            var previousVertice = builder.vectorList[faceIndexPrevious];
            var nextVertice = builder.vectorList[faceIndexNext];
            var newFace = new List<int>();

            newFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(previousEdgeMiddle);
            
            newFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(previousEdgeMid);

            newFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(vertice);

            newFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(nextEdgeMiddle);
            
            newFace.Add(quintoBuilder.vectorList.Count);
            
            quintoBuilder.vectorList.Add(Vector3.Lerp(vertice,faceCenter,amount*2));

            newFace.Add(quintoBuilder.vectorList.Count);
            middleFace.Add(quintoBuilder.vectorList.Count);
            quintoBuilder.vectorList.Add(Vector3.Lerp(previousVertice,faceCenter,amount*2));

        quintoBuilder.faces.Add(newFace);
        }
    }
}