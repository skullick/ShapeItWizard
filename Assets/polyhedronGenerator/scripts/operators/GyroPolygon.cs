using System.Collections.Generic;
using UnityEngine;

namespace polyhedronGenerator.scripts.operators {
    public class GyroPolygon {
        public static MeshBuilder apply(MeshBuilder builder, float amount) {
            var gyroBuilder = new MeshBuilder();

            foreach (var face in builder.faces) {
                var faceCenter = Vector3.zero;
                foreach (var verticeIndex in face) {
                    var vertice = builder.vectorList[verticeIndex];
                    faceCenter += vertice / face.Count;
                }

                var faceCenterIndex = gyroBuilder.vectorList.Count;
                gyroBuilder.vectorList.Add(faceCenter);

                var current = builder.vectorList[face[0]];
                var previous = builder.vectorList[face[^1]];
                var next = builder.vectorList[face[1]];
                makeGyroFace(amount, gyroBuilder, current, next, faceCenterIndex, previous);
                
                for (var verticeIndex = 1; verticeIndex < face.Count - 1; verticeIndex++) {
                    current = builder.vectorList[face[verticeIndex]];
                    previous = builder.vectorList[face[verticeIndex-1]];
                    next = builder.vectorList[face[verticeIndex+1]];
                    
                    makeGyroFace(amount, gyroBuilder, current, next, faceCenterIndex, previous);
                }
                current = builder.vectorList[face[^1]];
                previous = builder.vectorList[face[^2]];
                next = builder.vectorList[face[0]];
                makeGyroFace(amount, gyroBuilder, current, next, faceCenterIndex, previous);

            }

            return gyroBuilder;
        }

        private static void makeGyroFace(float amount, MeshBuilder gyroBuilder, Vector3 current, Vector3 next,
            int faceCenterIndex, Vector3 previous) {
            
            var newFace = new List<int> { gyroBuilder.vectorList.Count };
            gyroBuilder.vectorList.Add(current);
            
            newFace.Add(gyroBuilder.vectorList.Count);
            gyroBuilder.vectorList.Add(Vector3.Lerp(current,next,amount));
            
            newFace.Add(faceCenterIndex);
            
            newFace.Add(gyroBuilder.vectorList.Count);
            gyroBuilder.vectorList.Add(Vector3.Lerp(previous,current,amount));
            
            newFace.Add(gyroBuilder.vectorList.Count);
            gyroBuilder.vectorList.Add(Vector3.Lerp(previous,current,1-amount));
            
            gyroBuilder.faces.Add(newFace);
        }
    }
}