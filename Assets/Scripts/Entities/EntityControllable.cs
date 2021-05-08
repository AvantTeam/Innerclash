using UnityEngine;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityControllable : MonoBehaviour {
        public float speed = 16f;
        public float jumpDuration = 0.5f, jumpHeight = 1f;
        public Transform ground;

        private Rigidbody2D body;
        public Vector2 RelVelocity { get; set; }
        public bool Grounded { get; private set; }
        private float jumpStart;

        private void Start() {
            body = gameObject.GetComponent<Rigidbody2D>();
            Grounded = false;
        }

        private void Update() {
            RaycastHit2D hit = Physics2D.Raycast(ground.position, Vector2.down, 0.01f, LayerMask.GetMask("Ground"));
            Grounded = hit.transform != null;

            body.velocity += RelVelocity * Time.deltaTime;
        }

        public void Move(float x) {
            body.velocity += new Vector2(x * speed * Time.deltaTime, 0f);
        }

        public void Jump() {
            if(Grounded) {
                jumpStart = Time.time;
                body.velocity = new Vector2(body.velocity.x, jumpHeight / jumpDuration);
            } else if(Time.time < jumpStart + jumpDuration) {
                body.velocity = new Vector2(body.velocity.x, jumpHeight / jumpDuration);
            }
        }
    }
}
