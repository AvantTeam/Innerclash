using UnityEngine;
using Innerclash.Core;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityControllable : MonoBehaviour {
        public float speed = 16f;
        public float accel = 2f;
        public float jumpHeight = 3f;
        [Range(0, 1)] public float midAirAccel = 0.2f;

        [SerializeField] private Transform ground;
        public float groundWidth = 1f;
        public LayerMask groundMask;
        public TileBehavior TileOn { get; private set; }

        private Rigidbody2D body;
        public Vector2 RelVelocity { get; set; }
        public bool Grounded { get; private set; }
        public bool Moving { get; private set; }

        private Vector2 moveTarget = Vector2.zero;

        private void Start() {
            body = gameObject.GetComponent<Rigidbody2D>();
            Grounded = false;
        }

        private void Update() {
            RaycastHit2D hit = Physics2D.BoxCast(ground.position, new Vector2(groundWidth, 0.01f), 0f, Vector2.down, 0.01f, groundMask);
            Grounded = hit.transform != null;
            TileOn = hit.transform != null ? hit.transform.GetComponent<TileBehavior>() : null;

            body.drag = Drag();
            body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, moveTarget.x, accel * AccelMultiplier() * Time.deltaTime), body.velocity.y);
            body.velocity += RelVelocity * Time.deltaTime;
        }

        public void Move(float x) {
            Moving = x != 0f;

            /*if(!Grounded) x *= AccelMultiplier();

            Vector2 target = new Vector2(body.velocity.x, 0f);
            Vector2.SmoothDamp(new Vector2(body.velocity.x, 0f), new Vector2(x * speed * SpeedMultiplier(), 0f), ref target, 1f / accel, speed * SpeedMultiplier(), Time.deltaTime);

            body.velocity = new Vector2(target.x, body.velocity.y);*/
            moveTarget = new Vector2(x * speed * SpeedMultiplier(), 0f);
        }

        public void Jump() {
            if(Grounded) {
                body.velocity += new Vector2(0f, Mathf.Sqrt(2f * -Physics2D.gravity.y * (jumpHeight * JumpMultiplier())));
            }
        }

        protected float SpeedMultiplier() {
            return Grounded ? GroundSpeedMultiplier() : AirSpeedMultiplier();
        }

        protected float GroundSpeedMultiplier() {
            return TileOn == null ? 1f : TileOn.speedMult;
        }

        protected float AirSpeedMultiplier() {
            return 1f;
        }

        protected float AccelMultiplier() {
            return Grounded ? 1f : AirAccelMultiplier();
        }

        protected float AirAccelMultiplier() {
            return midAirAccel;
        }

        protected float Drag() {
            return TileOn == null ? 0f : (Moving ? 0f : TileOn.drag);
        }

        protected float JumpMultiplier() {
            return TileOn == null ? 1f : TileOn.jumpMult;
        }
    }
}
