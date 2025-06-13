using UnityEngine;

namespace polyhedronGenerator.scripts {
    public enum Operations {
        Dual,
        Truncate,
        [Tooltip("Ambo ignores amount")]
        Ambo,
        Kis,
        Join,
        Meta,
        Gyro,
        Chamfer,
        Ortho,
        [Tooltip("Bevel ignores amount")]
        Bevel,
        Quinto,
        Whirl,
        Inset,
        Subdivide
    }
}