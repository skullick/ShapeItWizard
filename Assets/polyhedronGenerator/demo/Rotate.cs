using UnityEngine;

namespace soulercoaster.scripts {
    public class Rotate : MonoBehaviour {
        private float i;
        public float speed = 0.5f;
        public Vector3 axis = Vector3.up;

        private void FixedUpdate() {
            transform.rotation = Quaternion.AngleAxis(i, axis);
            i += speed;
        }
    }
}