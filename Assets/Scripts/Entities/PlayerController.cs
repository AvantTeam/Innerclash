using UnityEngine;

namespace Innerclash.Entities {
    public class PlayerController : EntityController {
        private void Update() {
            float inputX = Input.GetAxisRaw("Horizontal");
            Controllable.Move(inputX);

            bool jump = Input.GetButtonDown("Jump");
            if(jump) Controllable.Jump();
        }
    }
}
