using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace polyhedronGenerator.scripts {
    public class MeshBuilder {
        public List<Vector3> vectorList = new();
        private Vector3[] vertices = Array.Empty<Vector3>();
        public List<List<int>> faces = new();
        public List<Vector4> tangentList = new();
        private Vector4[] tangents = Array.Empty<Vector4>();
        public List<Vector3> normalList = new();
        private Vector3[] normals = Array.Empty<Vector3>();
        public List<Vector2> uvCoordinates1 = new();
        private Vector2[] uv1 = Array.Empty<Vector2>();
        private List<Vector2> uvCoordinates2 = new();
        private Vector2[] uv2 = Array.Empty<Vector2>();
        private List<List<int>> triangleList = new();
        public int i;

        public List<List<int>> edges() {
            var edges = new List<List<int>>();
            foreach (var face in faces) {
                var exists = false;
                for (int verticeIndex = 0; verticeIndex < face.Count - 1; verticeIndex++) {
                    exists = false;
                    foreach (var existingEdge in edges) {
                        if (edgeExists(existingEdge, face[verticeIndex], face[verticeIndex + 1])) {
                            exists = true;
                            break;
                        }
                    }

                    if (exists) {
                        continue;
                    }


                    var edge = new List<int> {
                        face[verticeIndex],
                        face[verticeIndex + 1]
                    };
                    edges.Add(edge);
                }

                exists = false;
                foreach (var existingEdge in edges) {
                    if (edgeExists(existingEdge, face[^1], face[0])) {
                        exists = true;
                        break;
                    }
                }

                if (!exists) {
                    edges.Add(new List<int> {
                        face[^1],
                        face[0]
                    });
                }
            }

            return edges;
        }

        private static bool edgeExists(List<int> existingEdge, int verticeIndex, int verticeIndex2) {
            return (existingEdge[0] == verticeIndex &&
                    existingEdge[1] == verticeIndex2) || (
                existingEdge[0] == verticeIndex2 &&
                existingEdge[1] == verticeIndex);
        }

        public Mesh build(string name, Mesh existing = null,
            bool splitMeshByEdgeCount = false,
            bool doubleSided = false
        ) {
            triangulateCreateNormalsAndUVs(splitMeshByEdgeCount, doubleSided);

            int subMeshIndex = 0;
            if (existing != null) {
                if (vertices.Length != vectorList.Count) {
                    vertices = new Vector3[vectorList.Count];
                    tangents = new Vector4[vectorList.Count];
                    normals = new Vector3[vectorList.Count];
                    uv1 = new Vector2[vectorList.Count];
                    uv2 = new Vector2[vectorList.Count];
                }

                vectorList.CopyTo(vertices);
                if (vertices.Length > 65534) {
                    existing.indexFormat = IndexFormat.UInt32;
                }
                else {
                    existing.indexFormat = IndexFormat.UInt16;
                }

                if (existing.vertexCount > vertices.Length) {
                    existing.SetTriangles(Array.Empty<int>(), 0);
                }

                existing.SetVertices(vertices);

                tangentList.CopyTo(tangents);
                existing.SetTangents(tangents);

                normalList.CopyTo(normals);
                existing.SetNormals(normals);

                uvCoordinates1.CopyTo(uv1);
                existing.SetUVs(0, uv1);
                uvCoordinates2.CopyTo(uv2);
                existing.SetUVs(1, uv2);

                subMeshIndex = 0;
                existing.subMeshCount = triangleList.Count;
                foreach (var submesh in triangleList) {
                    var triangles = new int[triangleList.Count];
                    submesh.CopyTo(triangles);
                    existing.SetTriangles(triangles, subMeshIndex);
                    subMeshIndex++;
                }

                existing.RecalculateBounds();

                return existing;
            }


            existing = new Mesh {
                name = name,
                vertices = vectorList.ToArray(),
                tangents = tangentList.ToArray(),
                normals = normalList.ToArray(),
                uv = uvCoordinates1.ToArray(),
                uv2 = uvCoordinates2.ToArray(),
            };
            subMeshIndex = 0;
            existing.subMeshCount = triangleList.Count;
            foreach (var submesh in triangleList) {
                var triangles = new int[submesh.Count];
                submesh.CopyTo(triangles);
                existing.SetTriangles(triangles, subMeshIndex);
                subMeshIndex++;
            }

            return existing;
        }


        private void pushNewTriangle(List<int> triangleList, List<Vector3> newVectorList,
            List<Vector3> newNormalList, Vector3 first, Vector3 second, Vector3 third, bool doubleSided) {
            var normal = Vector3.Cross(second - first, third - first).normalized;
            triangleList.Add(newVectorList.Count);
            newVectorList.Add(first);
            newNormalList.Add(normal);

            triangleList.Add(newVectorList.Count);
            newVectorList.Add(second);
            newNormalList.Add(normal);

            triangleList.Add(newVectorList.Count);
            newVectorList.Add(third);
            newNormalList.Add(normal);
            if (doubleSided) {
                triangleList.Add(newVectorList.Count);
                newVectorList.Add(third);
                newNormalList.Add(-normal);

                triangleList.Add(newVectorList.Count);
                newVectorList.Add(second);
                newNormalList.Add(-normal);

                triangleList.Add(newVectorList.Count);
                newVectorList.Add(first);
                newNormalList.Add(-normal);
            }
        }

        private void triangulate(List<int> resultTriangles, List<int> face, List<Vector3> newVectorList,
            List<Vector3> newNormalList, bool doubleSided) {
            var middle = Vector3.zero;
            foreach (var vertice in face) {
                middle += vectorList[vertice] / face.Count;
            }

            for (var faceIndex = 0; faceIndex < face.Count - 1; faceIndex++) {
                pushNewTriangle(resultTriangles, newVectorList, newNormalList,
                    vectorList[face[faceIndex]],
                    vectorList[face[faceIndex + 1]],
                    middle, doubleSided
                );
                var circlePos1 = faceIndex * 2 * Mathf.PI / face.Count;
                var circlePos2 = (faceIndex + 1) * 2 * Mathf.PI / face.Count;
                uvCoordinates1.Add(new Vector2((Mathf.Cos(circlePos1) + 1) / 2, (MathF.Sin(circlePos1) + 1) / 2f));
                uvCoordinates1.Add(new Vector2((Mathf.Cos(circlePos2) + 1) / 2, (MathF.Sin(circlePos2) + 1) / 2f));
                uvCoordinates1.Add(new Vector2(0.5f, 0.5f));
                if (doubleSided) {
                    uvCoordinates1.Add(new Vector2((Mathf.Cos(circlePos1) + 1) / 2, (MathF.Sin(circlePos1) + 1) / 2f));
                    uvCoordinates1.Add(new Vector2((Mathf.Cos(circlePos2) + 1) / 2, (MathF.Sin(circlePos2) + 1) / 2f));
                    uvCoordinates1.Add(new Vector2(0.5f, 0.5f));
                }
            }

            pushNewTriangle(resultTriangles, newVectorList, newNormalList,
                vectorList[face[^1]],
                vectorList[face[0]],
                middle, doubleSided
            );
            var lastCirclePos = (face.Count - 1) * 2 * Mathf.PI / face.Count;
            uvCoordinates1.Add(new Vector2((Mathf.Cos(lastCirclePos) + 1) / 2, (MathF.Sin(lastCirclePos) + 1) / 2));
            uvCoordinates1.Add(new Vector2((Mathf.Cos(0) + 1) / 2, (MathF.Sin(0) + 1) / 2));
            uvCoordinates1.Add(new Vector2(0.5f, 0.5f));
            if (doubleSided) {
                uvCoordinates1.Add(new Vector2((Mathf.Cos(lastCirclePos) + 1) / 2, (MathF.Sin(lastCirclePos) + 1) / 2));
                uvCoordinates1.Add(new Vector2((Mathf.Cos(0) + 1) / 2, (MathF.Sin(0) + 1) / 2));
                uvCoordinates1.Add(new Vector2(0.5f, 0.5f));
            }
        }

        public void reset(bool resetOnlyVertices) {
            i = 0;
            vectorList.Clear();
            tangentList.Clear();
            normalList.Clear();
            if (resetOnlyVertices) {
                return;
            }

            faces.Clear();
            uvCoordinates1.Clear();
            uvCoordinates2.Clear();
        }

        public void removeDuplicateVertices(float mergeDistance, bool spherize = false, float radius = 0f,float randomizeVertices=0f) {
            var vectorIndex = new Dictionary<Vector3, int>();
            var newVectors = new List<Vector3>();
            foreach (var vector in vectorList) {
                var found = false;
                var foundVector = Vector3.zero;
                var realVector = vector;
                if (spherize) {
                    realVector = realVector.normalized * radius;
                }

                foreach (var uniqueVector in newVectors) {
                    if (Vector3.Distance(realVector, uniqueVector) < mergeDistance) {
                        found = true;
                        foundVector = uniqueVector;
                        break;
                    }
                }

                if (!found) {
                    vectorIndex[realVector] = newVectors.Count;
                    newVectors.Add(realVector+UnityEngine.Random.insideUnitSphere.normalized*randomizeVertices);
                }
                else {
                    vectorIndex[realVector] = vectorIndex[foundVector];
                }
            }


            var newFaces = new List<List<int>>();
            foreach (var face in faces) {
                var newFace = new List<int>();
                var lastFace = -1;
                foreach (var vertice in face) {
                    var realVertice = vectorList[vertice];
                    if (spherize) {
                        realVertice = realVertice.normalized * radius;
                    }

                    var newFaceIndex = vectorIndex[realVertice];
                    if (newFaceIndex != lastFace) {
                        newFace.Add(newFaceIndex);
                    }

                    lastFace = newFaceIndex;
                }

                newFaces.Add(newFace);
            }

            vectorList = newVectors;
            faces = newFaces;
        }

        private void triangulateCreateNormalsAndUVs(bool splitMeshByEdgeCount, bool doubleSided) {
            var newTriangleList = new List<List<int>>();
            var newVectorList = new List<Vector3>();
            var newNormalList = new List<Vector3>();
            var subMeshesPerEdgeCount = new Dictionary<int, List<int>>();
            if (!splitMeshByEdgeCount) {
                subMeshesPerEdgeCount[0] = new List<int>();
                newTriangleList.Add(subMeshesPerEdgeCount[0]);
            }

            uvCoordinates1.Clear();
            foreach (var face in faces) {
                switch (face.Count) {
                    case 0: {
                        continue;
                    }
                    case 1: {
                        continue;
                    }
                    case 2: {
                        continue;
                    }
                    case 3: {
                        if (splitMeshByEdgeCount && !subMeshesPerEdgeCount.ContainsKey(3)) {
                            var newList = new List<int>();
                            subMeshesPerEdgeCount[3] = newList;
                            newTriangleList.Add(newList);
                        }

                        var subMesh = getSubMesh(splitMeshByEdgeCount, subMeshesPerEdgeCount, face);

                        pushNewTriangle(subMesh, newVectorList, newNormalList,
                            vectorList[face[0]],
                            vectorList[face[1]],
                            vectorList[face[2]], doubleSided
                        );
                        var bottom = (1 - (Mathf.Sqrt(3) / 2));
                        uvCoordinates1.Add(new Vector2(0, bottom));
                        uvCoordinates1.Add(new Vector2(0.5f, bottom + Mathf.Sqrt(3) / 2));
                        uvCoordinates1.Add(new Vector2(1, bottom));
                        if (doubleSided) {
                            uvCoordinates1.Add(new Vector2(0, bottom));
                            uvCoordinates1.Add(new Vector2(0.5f, bottom + Mathf.Sqrt(3) / 2));
                            uvCoordinates1.Add(new Vector2(1, bottom));
                        }

                        break;
                    }
                    case 4: {
                        if (splitMeshByEdgeCount && !subMeshesPerEdgeCount.ContainsKey(4)) {
                            var newList = new List<int>();
                            subMeshesPerEdgeCount[4] = newList;
                            newTriangleList.Add(newList);
                        }

                        var subMesh = getSubMesh(splitMeshByEdgeCount, subMeshesPerEdgeCount, face);

                        pushNewTriangle(subMesh, newVectorList, newNormalList,
                            vectorList[face[0]],
                            vectorList[face[1]],
                            vectorList[face[3]], doubleSided
                        );
                        pushNewTriangle(subMesh, newVectorList, newNormalList,
                            vectorList[face[1]],
                            vectorList[face[2]],
                            vectorList[face[3]], doubleSided
                        );
                        uvCoordinates1.Add(new Vector2(0, 0));
                        uvCoordinates1.Add(new Vector2(0, 1));
                        uvCoordinates1.Add(new Vector2(1, 0));

                        uvCoordinates1.Add(new Vector2(0, 1));
                        uvCoordinates1.Add(new Vector2(1, 1));
                        uvCoordinates1.Add(new Vector2(1, 0));
                        if (doubleSided) {
                            uvCoordinates1.Add(new Vector2(0, 0));
                            uvCoordinates1.Add(new Vector2(0, 1));
                            uvCoordinates1.Add(new Vector2(1, 0));

                            uvCoordinates1.Add(new Vector2(0, 1));
                            uvCoordinates1.Add(new Vector2(1, 1));
                            uvCoordinates1.Add(new Vector2(1, 0));
                        }

                        break;
                    }
                    default: {
                        if (splitMeshByEdgeCount && !subMeshesPerEdgeCount.ContainsKey(face.Count)) {
                            var newList = new List<int>();
                            subMeshesPerEdgeCount[face.Count] = newList;
                            newTriangleList.Add(newList);
                        }

                        var subMesh = getSubMesh(splitMeshByEdgeCount, subMeshesPerEdgeCount, face);
                        triangulate(subMesh, face, newVectorList, newNormalList, doubleSided);
                        break;
                    }
                }
            }

            triangleList = newTriangleList;
            vectorList = newVectorList;
            normalList = newNormalList;
        }

        private static List<int> getSubMesh(bool splitMeshByEdgeCount, Dictionary<int, List<int>> subMeshesPerEdgeCount,
            List<int> face) {
            if (splitMeshByEdgeCount) {
                return subMeshesPerEdgeCount[face.Count];
            }
            else {
                return subMeshesPerEdgeCount[0];
            }
        }
    }
}