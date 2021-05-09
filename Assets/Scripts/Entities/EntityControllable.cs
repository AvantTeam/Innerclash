using UnityEngine;
using Innerclash.Core;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityControllable : MonoBehaviour {
        public float speed = 16f;
        public float accel = 2f;
        public float jumpHeight = 3f;

        [SerializeField] private Transform ground;
        public float groundWidth = 1f;
        public TileBehavior TileOn { get; private set; }

        private Rigidbody2D body;
        public Vector2 RelVelocity { get; set; }
        public bool Grounded { get; private set; }
        public bool Moving { get; private set; }

        private void Start() {
            body = gameObject.GetComponent<Rigidbody2D>();
            Grounded = false;
        }

        private void Update() {
            RaycastHit2D hit = Physics2D.BoxCast(ground.position, new Vector2(groundWidth, 0.01f), 0f, Vector2.down, 0.01f, LayerMask.GetMask("Ground"));
            Grounded = hit.transform != null;
            TileOn = hit.transform != null ? hit.transform.GetComponent<TileBehavior>() : null;
            
            body.drag = Drag();
            body.velocity += RelVelocity * Time.deltaTime;
        }

        public void Move(float x) {
            Moving = x != 0f;
            if(Moving) {
                body.velocity = Vector2.MoveTowards(body.velocity, new Vector2(body.velocity.x + x * speed * SpeedMultiplier() * Time.deltaTime, body.velocity.y), accel * 60f * Time.deltaTime);
            }
        }

        public void Jump() {
            if(Grounded) {
                body.velocity += new Vector2(0f, Mathf.Sqrt(2f * -Physics2D.gravity.y * (jumpHeight * JumpMultiplier())));
            }
        }

        protected float SpeedMultiplier() {
            return Grounded ? GroundSpeedMultiplier() : 0.3f;
        }

        protected float GroundSpeedMultiplier() {
            return TileOn == null ? 1f : TileOn.speedMult;
        }

        protected float Drag() {
            return TileOn == null ? 0f : (Moving ? 0f : TileOn.drag);
        }

        protected float JumpMultiplier() {
            return TileOn == null ? 1f : TileOn.jumpMult;
        }
    }
}
