// using System;
// using System.Collections.Generic;
// using polyhedronGenerator.scripts.operators;
// using polyhedronGenerator.scripts.solids;
// using UnityEditor;
// using UnityEngine;

// namespace polyhedronGenerator.scripts {
//     [RequireComponent(typeof(MeshFilter))]
//     public class PolyhedronGenerator : MonoBehaviour {
//         [Header("Generates meshes using conways polyhedron notation")] [Tooltip("Base mesh for generating")]
//         public PolyhedronBase polyhedronBaseForm;

//         [Tooltip("Operations are applied one after each other.")]
//         public List<Operation> operations = new();

//         public int radius = 1;
//         public bool liveUpdate = true;
//         public bool doubleSided;

//         [Tooltip("Moves all vertex positions by a random amount. Value is percentage of radius")]
//         [Range(0, 1)]
//         public float randomizeVertexPositions = 0f;

//         [Header("for Prisma")] public float height = 1f;
//         [Min(3)] public int sides = 3;
//         [Range(0, 93)] [Header("for Johnson")] public int index;

//         public void OnValidate() {
//             if (liveUpdate) {
//                 generate();
//             }
//         }
// #if UNITY_EDITOR
//         [ContextMenu("saveMesh()")]
//         private void saveMesh() {
//             if (GetComponent<MeshFilter>().sharedMesh == null) {
//                 generate();
//             }

//             ProjectWindowUtil.CreateAsset(GetComponent<MeshFilter>().sharedMesh, "polyhedron.asset");
//             AssetDatabase.SaveAssets();
//         }
// #endif

//         [ContextMenu("generate")]
//         public void generate() {
//             var currentBuilder = createBaseBuilder();
//             var replacedOperations = replaceMetaOperations(operations);

//             foreach (var op in replacedOperations) {
//                 currentBuilder = apply(op, currentBuilder);
//                 currentBuilder.removeDuplicateVertices(radius * 0.01f, op.spherize, radius);
//             }

//             if (randomizeVertexPositions != 0f) {
//                 currentBuilder.removeDuplicateVertices(radius * 0.01f, false, radius,
//                     radius * randomizeVertexPositions);
//             }

//             GetComponent<MeshFilter>().mesh = currentBuilder.build("polyhedron"
//                 , splitMeshByEdgeCount: true,
//                 doubleSided: doubleSided);
//         }

//         private MeshBuilder createBaseBuilder() {
//             switch (polyhedronBaseForm) {
//                 case PolyhedronBase.Icosahedron:
//                     return Icosahedron.generate(radius);
//                 case PolyhedronBase.Tetrahedron:
//                     return Tetrahedon.generate(radius);
//                 case PolyhedronBase.Cube:
//                     return Cube.generate(radius);
//                 case PolyhedronBase.Octahedron:
//                     return Octahedron.generate(radius);
//                 case PolyhedronBase.Dodecahedron:
//                     return Dodecahedron.generate(radius);
//                 case PolyhedronBase.Prisma:
//                     return Prisma.generate(radius, sides, height);
//                 case PolyhedronBase.AntiPrisma:
//                     var builder = AntiPrisma.generate(radius, sides, height);
//                     builder.removeDuplicateVertices(radius * 0.01f, false, radius);
//                     return builder;
//                 case PolyhedronBase.Johnson:
//                     return Johnson.generate(radius, index);
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//         }

//         private static List<Operation> replaceMetaOperations(List<Operation> operations) {
//             var replacedOperations = new List<Operation>();
//             var replaced = false;
//             foreach (var op in operations) {
//                 switch (op.op) {
//                     case Operations.Join:
//                         buildJoin(replacedOperations, op);
//                         replaced = true;
//                         continue;
//                     case Operations.Meta:
//                         buildMeta(replacedOperations, op);
//                         replaced = true;
//                         continue;
//                     case Operations.Ortho:
//                         buildOrtho(replacedOperations, op);
//                         replaced = true;
//                         continue;
//                     case Operations.Bevel:
//                         buildBevel(replacedOperations, op);
//                         replaced = true;
//                         continue;
//                     case Operations.Ambo:
//                         buildAmbo(replacedOperations, op);
//                         replaced = true;
//                         continue;
//                     default:
//                         replacedOperations.Add(op);
//                         break;
//                 }
//             }

//             if (replaced) {
//                 return replaceMetaOperations(replacedOperations);
//             }

//             return replacedOperations;
//         }

//         private static void buildJoin(List<Operation> replacedOperations, Operation op) {
//             replacedOperations.Add(new Operation {
//                 op = Operations.Dual,
//                 spherize = op.spherize,
//             });
//             replacedOperations.Add(new Operation {
//                 op = Operations.Ambo,
//                 spherize = op.spherize,
//                 amount = op.amount,
//             });
//             replacedOperations.Add(new Operation {
//                 op = Operations.Dual,
//                 spherize = op.spherize,
//             });
//         }

//         private static void buildOrtho(List<Operation> replacedOperations, Operation op) {
//             replacedOperations.Add(new Operation {
//                 op = Operations.Join,
//                 spherize = op.spherize,
//                 amount = op.amount
//             });
//             replacedOperations.Add(new Operation {
//                 op = Operations.Join,
//                 spherize = op.spherize,
//                 amount = op.amount
//             });
//         }

//         private static void buildAmbo(List<Operation> replacedOperations, Operation op) {
//             replacedOperations.Add(new Operation {
//                 op = Operations.Truncate,
//                 spherize = op.spherize,
//                 amount = 5f
//             });
//         }

//         private static void buildBevel(List<Operation> replacedOperations, Operation op) {
//             replacedOperations.Add(new Operation {
//                 op = Operations.Truncate,
//                 spherize = op.spherize,
//                 amount = 10f
//             });
//             replacedOperations.Add(new Operation {
//                 op = Operations.Ambo,
//                 spherize = op.spherize,
//             });
//         }

//         private static void buildMeta(List<Operation> replacedOperations, Operation op) {
//             replacedOperations.Add(new Operation {
//                 op = Operations.Kis,
//                 spherize = op.spherize,
//                 amount = op.amount,
//                 degree = 3
//             });
//             replacedOperations.Add(new Operation {
//                 op = Operations.Join,
//                 spherize = op.spherize,
//                 amount = op.amount
//             });
//         }

//         private MeshBuilder apply(Operation op, MeshBuilder currentBuilder) {
//             return op.op switch {
//                 Operations.Dual => DualPolygon.apply(currentBuilder),
//                 Operations.Truncate => TruncatePolygon.apply(currentBuilder, (op.amount + 10f) / 40f),
//                 Operations.Kis => KisPolygon.apply(currentBuilder, op.amount, radius, op.degree),
//                 Operations.Gyro => GyroPolygon.apply(currentBuilder, (op.amount + 10f) / 20f),
//                 Operations.Chamfer => ChamferPolygon.apply(currentBuilder, (op.amount + 10f) / 20f),
//                 Operations.Quinto => QuintoPolygon.apply(currentBuilder, (op.amount + 10f) / 20f),
//                 Operations.Inset => InsetPolygon.apply(currentBuilder, (op.amount) / 11f, op.degree),
//                 Operations.Whirl => WhirlPolygon.apply(currentBuilder, (op.amount + 10f) / 40f),
//                 Operations.Subdivide => SubdividePolygon.apply(currentBuilder, op.amount, op.degree),
//                 _ => currentBuilder
//             };
//         }
//     }
// }


using System;
using System.Collections.Generic;
using polyhedronGenerator.scripts.operators;
using polyhedronGenerator.scripts.solids;
using UnityEditor;
using UnityEngine;

namespace polyhedronGenerator.scripts {
    [RequireComponent(typeof(MeshFilter))]
    public class PolyhedronGenerator : MonoBehaviour {
        [Header("Generates meshes using conways polyhedron notation")] [Tooltip("Base mesh for generating")]
        public PolyhedronBase polyhedronBaseForm;

        [Tooltip("Operations are applied one after each other.")]
        public List<Operation> operations = new();

        public int radius = 1;
        public bool liveUpdate = true;
        public bool doubleSided;

        [Tooltip("Moves all vertex positions by a random amount. Value is percentage of radius")]
        [Range(0, 1)]
        public float randomizeVertexPositions = 0f;

        [Header("for Prisma")] public float height = 1f;
        [Min(3)] public int sides = 3;
        [Range(0, 93)] [Header("for Johnson")] public int index;

        private void Start() {
            // Generate mesh at runtime in builds
            if (GetComponent<MeshFilter>().sharedMesh == null) {
                generate();
                Debug.Log($"[PolyhedronGenerator] Generated mesh for {gameObject.name}");
            }
        }

        public void OnValidate() {
            if (liveUpdate && Application.isEditor) {
                generate();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("saveMesh()")]
        private void saveMesh() {
            if (GetComponent<MeshFilter>().sharedMesh == null) {
                generate();
            }

            var mesh = GetComponent<MeshFilter>().sharedMesh;
            // Clone mesh to avoid modifying the runtime instance
            var savedMesh = Instantiate(mesh);
            savedMesh.name = "polyhedron";
            ProjectWindowUtil.CreateAsset(savedMesh, "Assets/Polyhedrons/polyhedron.asset");
            AssetDatabase.SaveAssets();
            Debug.Log($"[PolyhedronGenerator] Saved mesh to Assets/Polyhedrons/polyhedron.asset");
        }
#endif

        [ContextMenu("generate")]
        public void generate() {
            var currentBuilder = createBaseBuilder();
            var replacedOperations = replaceMetaOperations(operations);

            foreach (var op in replacedOperations) {
                currentBuilder = apply(op, currentBuilder);
                currentBuilder.removeDuplicateVertices(radius * 0.01f, op.spherize, radius);
            }

            if (randomizeVertexPositions != 0f) {
                currentBuilder.removeDuplicateVertices(radius * 0.01f, false, radius,
                    radius * randomizeVertexPositions);
            }

            var mesh = currentBuilder.build("polyhedron",
                splitMeshByEdgeCount: true,
                doubleSided: doubleSided);
            GetComponent<MeshFilter>().mesh = mesh;
            Debug.Log($"[PolyhedronGenerator] Mesh generated for {gameObject.name}");
        }

        private MeshBuilder createBaseBuilder() {
            switch (polyhedronBaseForm) {
                case PolyhedronBase.Icosahedron:
                    return Icosahedron.generate(radius);
                case PolyhedronBase.Tetrahedron:
                    return Tetrahedon.generate(radius);
                case PolyhedronBase.Cube:
                    return Cube.generate(radius);
                case PolyhedronBase.Octahedron:
                    return Octahedron.generate(radius);
                case PolyhedronBase.Dodecahedron:
                    return Dodecahedron.generate(radius);
                case PolyhedronBase.Prisma:
                    return Prisma.generate(radius, sides, height);
                case PolyhedronBase.AntiPrisma:
                    var builder = AntiPrisma.generate(radius, sides, height);
                    builder.removeDuplicateVertices(radius * 0.01f, false, radius);
                    return builder;
                case PolyhedronBase.Johnson:
                    return Johnson.generate(radius, index);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static List<Operation> replaceMetaOperations(List<Operation> operations) {
            var replacedOperations = new List<Operation>();
            var replaced = false;
            foreach (var op in operations) {
                switch (op.op) {
                    case Operations.Join:
                        buildJoin(replacedOperations, op);
                        replaced = true;
                        continue;
                    case Operations.Meta:
                        buildMeta(replacedOperations, op);
                        replaced = true;
                        continue;
                    case Operations.Ortho:
                        buildOrtho(replacedOperations, op);
                        replaced = true;
                        continue;
                    case Operations.Bevel:
                        buildBevel(replacedOperations, op);
                        replaced = true;
                        continue;
                    case Operations.Ambo:
                        buildAmbo(replacedOperations, op);
                        replaced = true;
                        continue;
                    default:
                        replacedOperations.Add(op);
                        break;
                }
            }

            if (replaced) {
                return replaceMetaOperations(replacedOperations);
            }

            return replacedOperations;
        }

        private static void buildJoin(List<Operation> replacedOperations, Operation op) {
            replacedOperations.Add(new Operation {
                op = Operations.Dual,
                spherize = op.spherize,
            });
            replacedOperations.Add(new Operation {
                op = Operations.Ambo,
                spherize = op.spherize,
                amount = op.amount,
            });
            replacedOperations.Add(new Operation {
                op = Operations.Dual,
                spherize = op.spherize,
            });
        }

        private static void buildOrtho(List<Operation> replacedOperations, Operation op) {
            replacedOperations.Add(new Operation {
                op = Operations.Join,
                spherize = op.spherize,
                amount = op.amount
            });
            replacedOperations.Add(new Operation {
                op = Operations.Join,
                spherize = op.spherize,
                amount = op.amount
            });
        }

        private static void buildAmbo(List<Operation> replacedOperations, Operation op) {
            replacedOperations.Add(new Operation {
                op = Operations.Truncate,
                spherize = op.spherize,
                amount = 5f
            });
        }

        private static void buildBevel(List<Operation> replacedOperations, Operation op) {
            replacedOperations.Add(new Operation {
                op = Operations.Truncate,
                spherize = op.spherize,
                amount = 10f
            });
            replacedOperations.Add(new Operation {
                op = Operations.Ambo,
                spherize = op.spherize,
            });
        }

        private static void buildMeta(List<Operation> replacedOperations, Operation op) {
            replacedOperations.Add(new Operation {
                op = Operations.Kis,
                spherize = op.spherize,
                amount = op.amount,
                degree = 3
            });
            replacedOperations.Add(new Operation {
                op = Operations.Join,
                spherize = op.spherize,
                amount = op.amount
            });
        }

        private MeshBuilder apply(Operation op, MeshBuilder currentBuilder) {
            return op.op switch {
                Operations.Dual => DualPolygon.apply(currentBuilder),
                Operations.Truncate => TruncatePolygon.apply(currentBuilder, (op.amount + 10f) / 40f),
                Operations.Kis => KisPolygon.apply(currentBuilder, op.amount, radius, op.degree),
                Operations.Gyro => GyroPolygon.apply(currentBuilder, (op.amount + 10f) / 20f),
                Operations.Chamfer => ChamferPolygon.apply(currentBuilder, (op.amount + 10f) / 20f),
                Operations.Quinto => QuintoPolygon.apply(currentBuilder, (op.amount + 10f) / 20f),
                Operations.Inset => InsetPolygon.apply(currentBuilder, (op.amount) / 11f, op.degree),
                Operations.Whirl => WhirlPolygon.apply(currentBuilder, (op.amount + 10f) / 40f),
                Operations.Subdivide => SubdividePolygon.apply(currentBuilder, op.amount, op.degree),
                _ => currentBuilder
            };
        }
    }
}