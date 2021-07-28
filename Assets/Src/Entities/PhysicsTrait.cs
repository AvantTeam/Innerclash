using UnityEngine;
using Innerclash.Core;
using Innerclash.Utils;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class PhysicsTrait : MonoBehaviour {
        [Header("Movement")]
        public GroundCheck ground;
        /// <summary> Force used to move </summary>
        public float moveForce = 2500f;
        /// <summary> The velocity length at which this entity will reduce the move force if the value is >= to this </summary>
        public float maxSpeed = 15f;
        /// <summary> The force used when jumping on ground </summary>
        public float jumpForce = 640f;
        /// <summary> The force used to continue the jump mid-air </summary>
        public float jumpContinuousForce = 2500f;
        /// <summary> The maximum duration of jump continuation in seconds </summary>
        public float jumpDuration = 0.5f;
        /// <summary> The entity can regain full jump impulse after waiting for this amount of time after last jump, in seconds </summary>
        public float jumpCooldown = 0.6f;
        /// <summary> The entity would still be able to jump when it hits the ground if the jump trigger was within this amount of time, in seconds </summary>
        public float jumpBufferTime = 0.15f;

        int hitCount;
        RaycastHit2D[] hits;

        bool impulseJump;
        bool wasJumping;
        float jumpPower;
        float lastJumped;
        float lastJumpedMidAir;

        public Rigidbody2D Body { get; private set; }
        public Vector2 MoveAxis { get; private set; }

        /// <summary> Whether its #ground check is colliding with a tile </summary>
        public bool IsGrounded { get; private set; }
        /// <summary> Whether it has a movement target direction </summary>
        public bool IsMoving { get => MoveAxis.magnitude > 0.1f; }
        /// <summary> Whether its movement target direction clashes with the velocity's direction </summary>
        public bool IsTurning { get => (MoveAxis.x < 0f && Body.velocity.x > 0f) || (MoveAxis.x > 0f && Body.velocity.x < 0f); }
        public bool IsJumping { get; private set; }
        public float JumpTime { get; private set; }

        void Start() {
            Body = GetComponent<Rigidbody2D>();
            hits = new RaycastHit2D[Mathf.CeilToInt(ground.width / GameController.Instance.tilemap.cellSize.x) + 1];

            if(TryGetComponent(out ControllableTrait control)) {
                control.Move += OnMove;
                control.Jump += OnJump;
            }
        }

        void OnDestroy() {
            if(TryGetComponent(out ControllableTrait control)) {
                control.Move -= OnMove;
                control.Jump -= OnJump;
            }
        }

        void FixedUpdate() {
            Vector2 pos = transform.position;

            var axis = new Vector2(MoveAxis.x, 0f);
            var velX = new Vector2(Body.velocity.x, 0f);
            if(velX.magnitude < maxSpeed || IsTurning) {
                Body.AddForceAtPosition(
                    moveForce * Time.fixedDeltaTime * axis,
                    pos + ground.Center,
                    ForceMode2D.Force
                );
            }

            if(hits != null) {
                hitCount = Physics2D.RaycastNonAlloc(
                    pos + new Vector2(ground.offsetX - ground.width / 2f, ground.offsetY - 0.01f),
                    Vector2.right,
                    hits,
                    ground.width,
                    LayerMask.GetMask("Tilemap")
                );

                for(int i = 0; i < hitCount; i++) {
                    Tilemaps.ApplyTile(hits[i].point, this);
                }

                IsGrounded = hitCount > 0;
            }

            if(impulseJump) {
                impulseJump = false;
                wasJumping = false;

                IsJumping = true;
                JumpTime = 0f;

                jumpPower = Mathf.Pow(Mathf.Min(Time.time - lastJumped, jumpCooldown) / jumpCooldown, 2f);
                Body.AddForceAtPosition(
                    Time.fixedDeltaTime * new Vector2(0f, jumpForce * jumpPower),
                    pos + ground.Center,
                    ForceMode2D.Impulse
                );

                IsGrounded = false;
                lastJumped = Time.time;
            } else if(IsJumping) {
                JumpTime += Time.fixedDeltaTime;

                if(IsGrounded || JumpTime >= jumpDuration * jumpPower) {
                    IsJumping = false;
                    JumpTime = 0f;
                } else {
                    float alpha = 1f - JumpTime / jumpDuration;
                    Body.AddForceAtPosition(
                        Time.fixedDeltaTime * new Vector2(0f, jumpContinuousForce * alpha),
                        pos + ground.Center,
                        ForceMode2D.Force
                    );
                }
            } else {
                JumpTime = 0f;
            }
        }

        public void OnMove(Vector2 axis) => MoveAxis = axis;

        public void OnJump(bool jump) {
            if(!IsGrounded) {
                if(IsJumping && JumpTime < jumpDuration) {
                    IsJumping = jump;
                }

                if(!wasJumping) lastJumpedMidAir = Time.time;
                wasJumping = jump;
            } else if(IsGrounded && !IsJumping) {
                if(!wasJumping || Time.time - lastJumpedMidAir <= jumpBufferTime) {
                    impulseJump = jump;
                } else {
                    wasJumping = jump;
                }
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
            public Vector2 Center { get => new Vector2(offsetX, offsetY); }
        }
    }
}
