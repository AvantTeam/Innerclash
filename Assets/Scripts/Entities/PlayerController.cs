using UnityEngine;

namespace Innerclash.Entities {
    public class PlayerController : EntityController {
        public float maxJumpBufferTime = 0.1f, maxCoyoteTime = 0.04f;

        private float jumpBufferTimer = -Mathf.Infinity, coyoteTimer = 0;
        private bool hasJumped;

        private void Update() {
            if(controllable != null && Time.timeScale > 0f) {
                float inputX = Input.GetAxisRaw("Horizontal");
                controllable.Move(inputX);

                bool jump = Input.GetButtonDown("Jump");

                if (controllable.Grounded) {
                    coyoteTimer = Time.time;
                    if(!hasJumped && Time.time < jumpBufferTimer + maxJumpBufferTime) {
                        controllable.Jump();
                        jumpBufferTimer = 0f;
                        hasJumped = true;
                    }
                }
                if (jump) {
                    jumpBufferTimer = Time.time;
                    if(!hasJumped && Time.time < coyoteTimer + maxCoyoteTime) {
                        controllable.Jump();
                        coyoteTimer = 0f;
                        hasJumped = true;
                    }
                }
                if (Input.GetButtonUp("Jump")) hasJumped = false;
            }
        }
    }
}
