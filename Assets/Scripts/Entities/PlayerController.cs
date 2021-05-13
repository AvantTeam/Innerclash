using UnityEngine;

namespace Innerclash.Entities {
    public class PlayerController : EntityController {
        public float minJumpDelay = 0.2f, maxJumpBufferTime = 0.1f, maxCoyoteTime = 0.04f;

        private float jumpTimer = 0f, jumpBufferTimer = -Mathf.Infinity, coyoteTimer = 0f;
        private bool hasJumped;

        private void Update() {
            if(controllable != null && Time.timeScale > 0f) {
                float inputX = Input.GetAxisRaw("Horizontal");
                controllable.Move(inputX);

                bool jump = Input.GetButtonDown("Jump");

                if (controllable.Grounded) {
                    coyoteTimer = Time.time;
                    if(!hasJumped && Time.time < jumpBufferTimer + maxJumpBufferTime) {
                        DoJump();
                    }
                }
                if (jump) {
                    jumpBufferTimer = Time.time;
                    if(!hasJumped && Time.time < coyoteTimer + maxCoyoteTime) {
                        DoJump();
                    }
                }
                if (Time.time > jumpTimer + minJumpDelay) hasJumped = false;
            }
        }

        private void DoJump() {
            controllable.Jump();
            coyoteTimer = 0f;
            hasJumped = true;
            jumpTimer = Time.time;
        }
    }
}
