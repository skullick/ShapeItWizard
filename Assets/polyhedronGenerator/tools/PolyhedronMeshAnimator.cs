using polyhedronGenerator.scripts;
using UnityEngine;

namespace polyhedronGenerator.tools {
    public class PolyhedronMeshAnimator : MonoBehaviour {
        public PolyhedronGenerator meshGenerator;
        public int operationsSlot = 0;
        public float amountMin = -10f;
        public float amountMax = 10f;
        public float speed = 0.1f;
        public bool alternating;
        private float currentValue = -10f;
        private float direction = 1f;

        public void FixedUpdate() {
            var operation = meshGenerator.operations[operationsSlot];
            if (alternating) {
                if (currentValue < amountMin || currentValue > amountMax) {
                    direction = -direction;
                }

                if (currentValue < amountMin) {
                    currentValue = amountMin;
                }

                if (currentValue > amountMax) {
                    currentValue = amountMax;
                }
            }
            else {
                if (currentValue < amountMin || currentValue > amountMax) {
                    currentValue = amountMin;
                }
            }

            currentValue += speed * direction;

            operation.amount = currentValue;
            meshGenerator.operations[operationsSlot] = operation;
            meshGenerator.generate();
        }
    }
}