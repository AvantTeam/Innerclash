using UnityEngine;

namespace Innerclash.Entity {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Entity : MonoBehaviour {
        public Rigidbody2D Body { get; private set; }

        private void Start() {
            Body = GetComponent<Rigidbody2D>();
        }
    }
}
