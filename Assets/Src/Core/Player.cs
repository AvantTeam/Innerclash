using UnityEngine;
using Innerclash.Entities;

using static UnityEngine.InputSystem.InputAction;

namespace Innerclash.Core {
    public class Player : MonoBehaviour {
        public Entity controlled;

        MainInput input;

        Vector2 moveAxis;
        bool jump;

        void Awake() {
            input = new MainInput();
        }

        void FixedUpdate() {
            controlled.Move(moveAxis);
            controlled.Jump(jump);
        }

        void OnEnable() {
            input.Enable();
        }

        void OnDisable() {
            input.Disable();
        }

        public void Move(CallbackContext context) {
            moveAxis = context.ReadValue<Vector2>();
        }

        public void Jump(CallbackContext context) {
            jump = context.performed;
        }
    }
}
