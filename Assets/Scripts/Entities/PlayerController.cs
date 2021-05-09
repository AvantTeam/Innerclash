using UnityEngine;

namespace Innerclash.Entities {
    public class PlayerController : EntityController {
        private void Update() {
            if(controllable != null) {
                float inputX = Input.GetAxisRaw("Horizontal");
                controllable.Move(inputX);

                bool jump = Input.GetButtonDown("Jump");
                if(jump) controllable.Jump();
            }
        }
    }
}
