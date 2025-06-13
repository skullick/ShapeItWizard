using System;
using UnityEngine;

namespace polyhedronGenerator.scripts {
    [Serializable]
    public struct Operation {
        public Operations op;
        [Range(-10, 10f)] public float amount;
        public bool spherize;
        [Tooltip("Specified the degree of the operation for (kis,inset). Only faces with degree edges will be considered for the operation.")]
        public int degree;
    }
}