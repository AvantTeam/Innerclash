using UnityEngine;
using Innerclash.Core;
using Innerclash.Utils;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Entity : MonoBehaviour {
        public Rigidbody2D Body { get; private set; }
        public Vector2 MoveAxis { get; private set; }

        [Header("Movement")]
        public GroundCheck ground;
        /// <summary> Force used to move </summary>
        public float moveForce = 2500f;
        /// <summary> Force added when turning around </summary>
        public float turnForce = 2000f;
        /// <summary> The velocity length at which this entity won't add a move force if the value is >= to this </summary>
        public float maxSpeed = 15f;

        /// <summary> The amount of ground tiles this entity is touching </summary>
        int hitCount;
        /// <summary> Temporary array for raycasting. This array is initialized at #Start() </summary>
        RaycastHit2D[] hits;

        public bool IsGrounded { get => hitCount > 0; }

        public bool IsMoving { get => MoveAxis.magnitude > 0.1f; }

        public bool IsTurning { get => (MoveAxis.x < 0f && Body.velocity.x > 0f) || (MoveAxis.x > 0f && Body.velocity.x < 0f); }

        void Start() {
            Body = GetComponent<Rigidbody2D>();
            MoveAxis = new Vector2();

            hits = new RaycastHit2D[Mathf.FloorToInt(ground.width / Context.Instance.tilemap.cellSize.x) + 1];
        }

        void FixedUpdate() {
            if(hits != null) {
                hitCount = Physics2D.RaycastNonAlloc(
                    (Vector2)transform.position + new Vector2(ground.offsetX - ground.width / 2f, ground.offsetY - 0.1f),
                    Vector2.right,
                    hits,
                    ground.width
                );

                for(int i = 0; i < hitCount; i++) {
                    Tilemaps.ApplyTile(hits[i].point, this);
                }
            }

            var axisX = new Vector2(MoveAxis.x, 0f);
            var velX = new Vector2(Body.velocity.x, 0f);
            
            if(velX.magnitude < maxSpeed) Body.AddForce(moveForce * Time.fixedDeltaTime * axisX, ForceMode2D.Force);

            if(IsGrounded) {
                if(!IsMoving && Body.velocity.x != 0f) {
                    Body.AddForce(turnForce * Time.fixedDeltaTime * -1f * velX.normalized, ForceMode2D.Force);
                } else if(IsTurning) {
                    Body.AddForce(turnForce * Time.fixedDeltaTime * axisX, ForceMode2D.Force);
                }
            }
        }

        public void Move(Vector2 axis) => MoveAxis = axis;

        void OnDrawGizmos() {
            Gizmos.DrawLine(transform.position + (Vector3)ground.LeftLimit, transform.position + (Vector3)ground.RightLimit);
        }

        [System.Serializable]
        public struct GroundCheck {
            public float offsetX;
            public float offsetY;
            public float width;

            public Vector2 LeftLimit { get => new Vector2(offsetX - width / 2f, offsetY); }
            public Vector2 RightLimit { get => new Vector2(offsetX + width / 2f, offsetY); }
        }
    }
}
