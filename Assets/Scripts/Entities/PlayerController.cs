using UnityEngine;

namespace Innerclash.Entities {
    public class PlayerController : EntityController {
        private void Update() {
            float inputX = Input.GetAxis("Horizontal");
            Controllable.Move(inputX);

            bool jump = Input.GetButton("Jump");
            if(jump) Controllable.Jump();
        }
    }
}
