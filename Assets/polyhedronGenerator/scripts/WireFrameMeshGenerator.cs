using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace polyhedronGenerator.scripts {
    public enum WireFrameType {
        TWO_PLANE,
        POLYGON
    }

    [RequireComponent(typeof(MeshFilter))]
    public class WireFrameMeshGenerator : MonoBehaviour {
        public Mesh mesh;

        [Tooltip("MeshFilter parameter takes precedence")]
        public MeshFilter meshFilter;

        public float width = 1f;
        public WireFrameType type;

        [Header("For wireframe type Polygon")] [Min(3)]
        public int polygonEdgeCount = 3;


#if UNITY_EDITOR
        [ContextMenu("saveMesh")]
        public void saveMesh() {
            if (GetComponent<MeshFilter>().sharedMesh == null) {
                Debug.LogWarning("Could not safe, no mesh present.");
            }

            ProjectWindowUtil.CreateAsset(GetComponent<MeshFilter>().sharedMesh, "wireframe.asset");
            AssetDatabase.SaveAssets();
        }
#endif

        [ContextMenu("Generate")]
        public void generate() {
            var meshBuilder = new MeshBuilder();
            var wireFrameMesh = meshFilter?.sharedMesh ?? mesh;
            for (var i = 0; i < wireFrameMesh.triangles.Length; i += 3) {
                var firstVec = wireFrameMesh.vertices[wireFrameMesh.triangles[i]];
                var secondVec = wireFrameMesh.vertices[wireFrameMesh.triangles[i + 1]];
                var thirdVec = wireFrameMesh.vertices[wireFrameMesh.triangles[i + 2]];

                createLine(secondVec, firstVec, meshBuilder);
                createLine(secondVec, thirdVec, meshBuilder);
                createLine(firstVec, thirdVec, meshBuilder);
            }

            GetComponent<MeshFilter>().mesh =
                meshBuilder.build("wireframe", splitMeshByEdgeCount: false, doubleSided: false);
        }

        private void createLine(Vector3 secondVec, Vector3 firstVec, MeshBuilder meshBuilder) {
            var delta = (secondVec - firstVec).normalized;
            var pushQuad = createPushQuadPosition(width, delta, Vector3.up, firstVec, secondVec);
            if (type == WireFrameType.TWO_PLANE) {
                pushNextQuad(meshBuilder, pushQuad, 1f, 0f, 0f);
                pushUvAndTriangles(meshBuilder);
                pushNormalsAndTangents(meshBuilder, pushQuad.deltaDirection, pushQuad.quadDirection);
                pushNextQuad(meshBuilder, pushQuad, 1f, 90f, 0f);
                pushUvAndTriangles(meshBuilder);
                pushNormalsAndTangents(meshBuilder, pushQuad.deltaDirection, pushQuad.quadDirection);
            }

            if (type == WireFrameType.POLYGON) {
                var sideLength = (2f * Mathf.Tan(Mathf.PI / polygonEdgeCount)) * 0.25f;
                for (var i = 0; i < polygonEdgeCount; i++) {
                    pushNextQuad(meshBuilder, pushQuad, sideLength, i * 360f / polygonEdgeCount, 1);
                    pushUvAndTriangles(meshBuilder);
                    pushNormalsAndTangents(meshBuilder, pushQuad.deltaDirection, pushQuad.quadDirection);
                }
            }
        }

        private struct PushQuadPosition {
            public Vector3 position;
            public Vector3 nextPosition;
            public Vector3 quadDelta;
            public Vector3 quadDirection;
            public Vector3 deltaDirection;
            public float width;
        }

        private static void pushNextQuad(MeshBuilder builder,
            PushQuadPosition position,
            float quadWidthFactor, float planeAngle, float distanceToBase = 0f
        ) {
            var scaledDirection = Quaternion.AngleAxis(planeAngle, position.deltaDirection) * position.quadDirection *
                                  (position.width * quadWidthFactor);
            var base1 = position.position - Quaternion.AngleAxis(planeAngle, position.deltaDirection) *
                position.quadDelta * (distanceToBase * position.width) / 2;
            var base2 = position.nextPosition - Quaternion.AngleAxis(planeAngle, position.deltaDirection) *
                position.quadDelta * (distanceToBase * position.width) / 2;


            builder.vectorList.Add(base1 + scaledDirection);
            builder.vectorList.Add(base1 - scaledDirection);
            builder.vectorList.Add(base2 + scaledDirection);
            builder.vectorList.Add(base2 - scaledDirection);
        }

        private static PushQuadPosition createPushQuadPosition(float width, Vector3 deltaDirection,
            Vector3 crossProductBase, Vector3 position, Vector3 nextPosition, PushQuadPosition? existing = null) {
            var nextQuadDirection = Vector3.Cross(crossProductBase, deltaDirection).normalized;
            var nextQuadDelta = Vector3.Cross(nextQuadDirection, deltaDirection).normalized;
            if (existing == null) {
                return new PushQuadPosition {
                    position = position,
                    nextPosition = nextPosition,
                    quadDelta = nextQuadDelta,
                    quadDirection = nextQuadDirection,
                    width = width,
                    deltaDirection = deltaDirection
                };
            }

            var quadPosition = existing.Value;
            quadPosition.position = position;
            quadPosition.quadDelta = nextQuadDelta;
            quadPosition.quadDirection = nextQuadDirection;
            quadPosition.width = width;
            quadPosition.deltaDirection = deltaDirection;
            return quadPosition;
        }

        private static void pushNormalsAndTangents(MeshBuilder builder, Vector3 deltaDirection, Vector3 quadDirection) {
            builder.normalList.Add(Vector3.Cross(deltaDirection, quadDirection).normalized);
            builder.normalList.Add(Vector3.Cross(deltaDirection, quadDirection).normalized);
            builder.normalList.Add(Vector3.Cross(-deltaDirection, quadDirection).normalized);
            builder.normalList.Add(Vector3.Cross(-deltaDirection, quadDirection).normalized);
        }

        private static void pushUvAndTriangles(MeshBuilder builder) {
            var index = builder.vectorList.Count - 4;
            builder.faces.Add(new List<int> { index, index + 1, index + 2, index + 3 });
            builder.faces.Add(new List<int> { index + 3, index + 2, index + 1, index });
        }
    }
}