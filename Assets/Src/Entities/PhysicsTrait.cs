using UnityEngine;
using Innerclash.Core;
using Innerclash.Utils;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicsTrait : MonoBehaviour {
        public Rigidbody2D Body { get; private set; }
        public Vector2 MoveAxis { get; private set; }

        [Header("Movement")]
        public GroundCheck ground;
        /// <summary> Force used to move </summary>
        public float moveForce = 2500f;
        /// <summary> The velocity length at which this entity won't add a move force if the value is >= to this </summary>
        public float maxSpeed = 15f;
        /// <summary> The force used when jumping on ground </summary>
        public float jumpForce = 360f;
        /// <summary> The force used to continue the jump mid-air </summary>
        public float jumpContinuousForce = 2400f;
        /// <summary> The maximum duration of jump continuation in seconds </summary>
        public float jumpDuration = 0.5f;

        int hitCount;
        RaycastHit2D[] hits;

        bool shouldStop = false;
        bool jumped = false;

        /// <summary> Whether its #ground check is colliding with a tile </summary>
        public bool IsGrounded { get => hitCount > 0; }
        /// <summary> Whether it has a movement target direction </summary>
        public bool IsMoving { get => MoveAxis.magnitude > 0.1f; }
        public bool WasMoving { get; private set; }
        /// <summary> Whether its movement target direction clashes with the velocity's direction </summary>
        public bool IsTurning { get => (MoveAxis.x < 0f && Body.velocity.x > 0f) || (MoveAxis.x > 0f && Body.velocity.x < 0f); }
        public bool IsJumping { get; private set; }
        public float JumpTime { get; private set; }

        void Start() {
            Body = GetComponent<Rigidbody2D>();
            MoveAxis = new Vector2();
            WasMoving = false;

            hits = new RaycastHit2D[Mathf.CeilToInt(ground.width / GameController.Instance.tilemap.cellSize.x) + 1];
        }

        void FixedUpdate() {
            var axis = new Vector2(MoveAxis.x, 0f);
            var velX = new Vector2(Body.velocity.x, 0f);
            if(velX.magnitude < maxSpeed || IsTurning) {
                Body.AddForce(moveForce * Time.fixedDeltaTime * axis, ForceMode2D.Force);
            }

            if(shouldStop) {
                Body.velocity = new Vector2(0f, Body.velocity.y);
                shouldStop = false;
            }

            if(hits != null) {
                hitCount = Physics2D.RaycastNonAlloc(
                    (Vector2)transform.position + new Vector2(ground.offsetX - ground.width / 2f, ground.offsetY - 0.01f),
                    Vector2.right,
                    hits,
                    ground.width,
                    LayerMask.GetMask("Tilemap")
                );

                for(int i = 0; i < hitCount; i++) {
                    WasMoving |= Tilemaps.ApplyTile(hits[i].point, this);
                }

                if(!shouldStop && WasMoving) shouldStop = true;
            }

            if(jumped) {
                jumped = false;
                IsJumping = true;
                JumpTime = 0f;

                Body.AddForce(new Vector2(0f, jumpForce) * Time.fixedDeltaTime, ForceMode2D.Impulse);
            } else if(IsJumping) {
                JumpTime += Time.fixedDeltaTime;

                if(JumpTime >= jumpDuration) {
                    IsJumping = false;
                    JumpTime = 0f;
                } else {
                    float alpha = 1f - JumpTime / jumpDuration;
                    Body.AddForce(alpha * new Vector2(0f, jumpContinuousForce) * Time.fixedDeltaTime, ForceMode2D.Force);
                }
            } else {
                JumpTime = 0f;
            }
        }

        public void Move(Vector2 axis) {
            WasMoving = false;
            MoveAxis = axis;
        }

        public void Jump(bool jump) {
            if(IsGrounded) {
                jumped = jump;
            } else if(IsJumping && JumpTime < jumpDuration) {
                IsJumping = jump;
            }
        }

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
