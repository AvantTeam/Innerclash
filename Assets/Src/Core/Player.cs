using UnityEngine;
using UnityEngine.InputSystem;
using Innerclash.Entities;
using Innerclash.Utils;

using static UnityEngine.InputSystem.InputAction;

namespace Innerclash.Core {
    public class Player : MonoBehaviour {
        public GameObject controlled;

        MainInput input;

        Vector2 moveAxis;
        bool jump;

        void Awake() {
            input = new MainInput();
        }

        void FixedUpdate() {
            var phys = controlled.GetComponent<PhysicsTrait>();
            phys.Move(moveAxis);
            phys.Jump(jump);
        }

        void OnEnable() {
            input.Enable();
        }

        void OnDisable() {
            input.Disable();
        }

        /// <summary>
        /// Keyboard: WASD
        /// </summary>
        public void Move(CallbackContext context) {
            moveAxis = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Keyboard: Spacebar
        /// </summary>
        public void Jump(CallbackContext context) {
            jump = context.performed;
        }

        /// <summary>
        /// Mouse: Left click
        /// </summary>
        public void Break(CallbackContext context) {
            if(context.performed) {
                Tilemaps.RemoveTile(Context.Instance.mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            }
        }

        /// <summary>
        /// Mouse: Right click
        /// </summary>
        public void Interact(CallbackContext context) {
            if(context.performed) {

            }
        }
    }
}
