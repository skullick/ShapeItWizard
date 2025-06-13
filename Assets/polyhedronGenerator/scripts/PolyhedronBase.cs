using UnityEngine;

namespace polyhedronGenerator.scripts {
    public enum PolyhedronBase {
        Tetrahedron,
        Cube,
        Octahedron,
        Icosahedron,
        Dodecahedron,
        [Tooltip("Use extra parameters")] Prisma,
        [Tooltip("Use extra parameters")] AntiPrisma,
        [Tooltip("Use extra parameters")] Johnson,
    }
}