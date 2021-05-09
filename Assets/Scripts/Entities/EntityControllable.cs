using UnityEngine;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityControllable : MonoBehaviour {
        public float speed = 16f;
        public float accel = 2f;
        public float jumpHeight = 3f;

        public Transform ground;
        public float groundWidth = 1f;

        private Rigidbody2D body;
        public Vector2 RelVelocity { get; set; }
        public bool Grounded { get; private set; }
        private float jumpStart;

        private void Start() {
            body = gameObject.GetComponent<Rigidbody2D>();
            Grounded = false;
        }

        private void Update() {
            RaycastHit2D hit = Physics2D.BoxCast(ground.position, new Vector2(groundWidth, 0.01f), 0f, Vector2.down, 0.01f, LayerMask.GetMask("Ground"));
            Grounded = hit.transform != null;

            body.velocity += RelVelocity * Time.deltaTime;
        }

        public void Move(float x) {
            body.velocity = Vector2.Lerp(body.velocity, body.velocity + new Vector2(x * speed, 0f) * SpeedMultiplier() * Time.deltaTime, accel);
        }

        public void Jump() {
            if(Grounded) {
                body.velocity += new Vector2(0f, Mathf.Sqrt(2f * -Physics2D.gravity.y * jumpHeight));
            }
        }

        protected float SpeedMultiplier() {
            return Grounded ? 1f : 0.3f;
        }
    }
}
