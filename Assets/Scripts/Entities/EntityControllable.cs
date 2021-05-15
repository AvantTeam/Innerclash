using UnityEngine;
using Innerclash.Core;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityControllable : MonoBehaviour {
        [Header("Parameters")]
        public float moveSpeed = 10f;
        public float movementAccel = 2f, jumpHeight = 3f;
        [Range(0, 1)] public float midAirAccelMultiplier = 0.2f;

        [Header("Ground Check")]
        [SerializeField] private Transform ground;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundCheckWidth = 0.9f;

        public ScriptedTile TileOn { get; private set; }

        private Rigidbody2D body;
        public bool Grounded { get; private set; }
        public bool Moving { get; private set; }
        public bool Jumping { get; private set; }

        public float SpeedMultiplier { get => Grounded ? GroundSpeedMultiplier : AirSpeedMultiplier; }
        public float GroundSpeedMultiplier { get => TileOn == null ? 1f : TileOn.speedMult; }
        public float AirSpeedMultiplier { get => 1f; }
        public float AccelMultiplier { get => Grounded ? 1f : AirAccelMultiplier; }
        public float AirAccelMultiplier { get => midAirAccelMultiplier; }
        public float Drag { get => TileOn == null ? 0f : ((Moving || Jumping) ? 0f : TileOn.drag); }
        public float JumpMultiplier { get => TileOn == null ? 1f : TileOn.jumpMult; }

        private Vector2 moveTarget = Vector2.zero;

        private void Awake() {
            body = GetComponent<Rigidbody2D>();
            Grounded = false;
            TileOn = null;
        }

        private void Update() {
            RaycastHit2D hit = Physics2D.BoxCast(ground.position, new Vector2(groundCheckWidth, 0.01f), 0f, Vector2.down, 0.01f, groundMask);
            Grounded = hit.transform != null;
            TileOn = Logic.Instance.ForegroundTileAt(ground.position + Vector3.down * 0.01f) as ScriptedTile;
        }

        private void FixedUpdate() {
            body.drag = Drag;

            if(Jumping) {
                body.AddForce(Vector3.up * Mathf.Sqrt(2f * -Physics2D.gravity.y * (jumpHeight * JumpMultiplier)), ForceMode2D.Impulse);
                Jumping = false;
            }

            body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, moveTarget.x, movementAccel * AccelMultiplier * Time.fixedDeltaTime), body.velocity.y);
        }

        public void Move(float x) {
            Moving = x != 0f;
            moveTarget = new Vector2(x * moveSpeed * SpeedMultiplier, 0f);
        }

        public void Jump() {
            Jumping = true;
        }
    }
}
