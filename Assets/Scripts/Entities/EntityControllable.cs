using UnityEngine;
using Innerclash.Core;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityControllable : MonoBehaviour {
        public EntityType Type { get; set; }

        [SerializeField] private Transform ground;
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
        public float AirAccelMultiplier { get => Type.midAirAccel; }
        public float Drag { get => TileOn == null ? 0f : ((Moving || Jumping) ? 0f : TileOn.drag); }
        public float JumpMultiplier { get => TileOn == null ? 1f : TileOn.jumpMult; }

        private Vector2 moveTarget = Vector2.zero;

        private void Start() {
            body = GetComponent<Rigidbody2D>();
            Grounded = false;
            TileOn = null;

            Collider2D co = GetComponent<Collider2D>();
            if(co is BoxCollider2D box) {
                box.size = Type.hitSize;
            }else if(co is CapsuleCollider2D cap) {
                cap.size = Type.hitSize;
            }
        }

        private void Update() {
            RaycastHit2D hit = Physics2D.BoxCast(ground.position, new Vector2(Type.hitSize.x - 0.01f, 0.01f), 0f, Vector2.down, 0.01f, groundMask);
            Grounded = hit.transform != null;
            TileOn = Logic.Instance.tilemap.GetTile(Logic.Instance.tilemap.WorldToCell(ground.position + Vector3.down * 0.01f)) as ScriptedTile;
        }

        private void FixedUpdate() {
            body.drag = Drag;

            if(Jumping) {
                body.velocity += new Vector2(0f, Mathf.Sqrt(2f * -Physics2D.gravity.y * (Type.jumpHeight * JumpMultiplier)));
                Jumping = false;
            }

            body.velocity = new Vector2(Mathf.Lerp(body.velocity.x, moveTarget.x, Type.accel * AccelMultiplier * Time.fixedDeltaTime), body.velocity.y);
        }

        public void Move(float x) {
            Moving = x != 0f;
            moveTarget = new Vector2(x * Type.speed * SpeedMultiplier, 0f);
        }

        public void Jump() {
            Jumping = true;
        }
    }
}
