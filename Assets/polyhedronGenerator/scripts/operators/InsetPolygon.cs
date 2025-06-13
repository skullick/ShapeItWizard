using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.operators {
    public class InsetPolygon {
        public static MeshBuilder apply(MeshBuilder builder, float amount,int degree) {
            var insetBuilder = new MeshBuilder();

            foreach (var face in builder.faces) {
                
                if (degree != 0 && face.Count != degree) {
                    var newFace = new List<int>();
                    insetBuilder.faces.Add(newFace);
                    foreach (var verticeIndex in face) {
                        newFace.Add(insetBuilder.vectorList.Count);
                        insetBuilder.vectorList.Add(builder.vectorList[verticeIndex]);
                    }

                    continue;
                }
                var faceCenter = Vector3.zero;
                foreach (var verticeIndex in face) {
                    var vertice = builder.vectorList[verticeIndex];
                    faceCenter += vertice / face.Count;
                }

                var insetTarget = faceCenter / 2;
                var realAmount = amount;
                if (amount < 0) {
                    realAmount = -realAmount;
                    insetTarget += faceCenter ;
                }
                var middleFace = new List<int>();
                insetBuilder.faces.Add(middleFace);
                for (var faceIndex = 1; faceIndex < face.Count; faceIndex++) {
                    addInsetFace(builder, realAmount, insetBuilder, face, faceIndex - 1, faceIndex, insetTarget, middleFace);
                }

                addInsetFace(builder, realAmount, insetBuilder, face, face.Count-1, 0, insetTarget, middleFace);
            }

            return insetBuilder;
        }

        private static void addInsetFace(MeshBuilder builder, float amount, MeshBuilder insetBuilder, List<int> face,
            int previousIndex, int index,
            Vector3 insetTarget, List<int> middleFace) {
            var newFace = new List<int>();
            newFace.Add(insetBuilder.vectorList.Count);

            insetBuilder.vectorList.Add(builder.vectorList[face[previousIndex]]);
            newFace.Add(insetBuilder.vectorList.Count);

            insetBuilder.vectorList.Add(builder.vectorList[face[index]]);
            newFace.Add(insetBuilder.vectorList.Count);

            insetBuilder.vectorList.Add(
                Vector3.Lerp(builder.vectorList[face[index]], insetTarget, amount));
            newFace.Add(insetBuilder.vectorList.Count);

            middleFace.Add(insetBuilder.vectorList.Count);
            insetBuilder.vectorList.Add(
                Vector3.Lerp(builder.vectorList[face[previousIndex]], insetTarget, amount));

            insetBuilder.faces.Add(newFace);
        }
    }
}