using UnityEngine;
using Innerclash.Core;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityControllable : MonoBehaviour {
        public EntityType Type { get; set; }

        public float Speed { get => Type.speed; }
        public float Accel { get => Type.accel; }
        public float JumpHeight { get => Type.jumpHeight; }
        public float MidAirAccel { get => Type.midAirAccel; }

        [SerializeField] private Transform ground;
        public float groundWidth = 1f;
        public LayerMask groundMask;
        public ScriptedTile TileOn { get; private set; }

        private Rigidbody2D body;
        public bool Grounded { get; private set; }
        public bool Moving { get; private set; }
        public bool Jumping { get; private set; }

        public float SpeedMultiplier { get => Grounded ? GroundSpeedMultiplier : AirSpeedMultiplier; }
        public float GroundSpeedMultiplier { get => TileOn == null ? 1f : TileOn.speedMult; }
        public float AirSpeedMultiplier { get => 1f; }
        public float AccelMultiplier { get => Grounded ? 1f : AirAccelMultiplier; }
        public float AirAccelMultiplier { get => MidAirAccel; }
        public float Drag { get => TileOn == null ? 0f : ((Moving || Jumping) ? 0f : TileOn.drag); }
        public float JumpMultiplier { get => TileOn == null ? 1f : TileOn.jumpMult; }

        private Vector2 moveTarget = Vector2.zero;

        private void Start() {
            body = gameObject.GetComponent<Rigidbody2D>();
            Grounded = false;
            TileOn = null;
        }

        private void Update() {
            RaycastHit2D hit = Physics2D.BoxCast(ground.position, new Vector2(groundWidth, 0.01f), 0f, Vector2.down, 0.01f, groundMask);
            Grounded = hit.transform != null;
            TileOn = Logic.Instance.tilemap.GetTile(new Vector3Int((int)ground.position.x, (int)(ground.position.y - 0.01f), 0)) as ScriptedTile;
        }

        private void FixedUpdate() {
            body.drag = Drag;

            if(Jumping) {
                body.velocity += new Vector2(0f, Mathf.Sqrt(2f * -Physics2D.gravity.y * (JumpHeight * JumpMultiplier)));
                Jumping = false;
            }

            body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, moveTarget.x, Accel * AccelMultiplier * Time.fixedDeltaTime), body.velocity.y);
        }

        public void Move(float x) {
            Moving = x != 0f;
            moveTarget = new Vector2(x * Speed * SpeedMultiplier, 0f);
        }

        public void Jump() {
            Jumping = true;
        }
    }
}
