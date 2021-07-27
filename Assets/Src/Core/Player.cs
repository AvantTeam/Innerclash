using UnityEngine;
using UnityEngine.InputSystem;
using Innerclash.Entities;
using Innerclash.Utils;

using static UnityEngine.InputSystem.InputAction;

namespace Innerclash.Core {
    public class Player : MonoBehaviour {
        public ControllableTrait controlled;

        Vector2 moveAxis;
        bool jump;
        float breakPress, actPress;

        void FixedUpdate() {
            controlled.OnMove(moveAxis);
            controlled.OnJump(jump);
        }

        void Update() {
            breakPress = GameController.Instance.ViewingOverview ? 0f : breakPress;
            actPress = GameController.Instance.ViewingOverview ? 0f : actPress;

            if(breakPress > 0f) {
                Tilemaps.RemoveTile(GameController.Instance.mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            }
            if(actPress > 0f) {

            }
        }

        /// <summary>
        /// Keyboard: W-A-S-D
        /// </summary>
        public void OnMove(CallbackContext context) => moveAxis = context.ReadValue<Vector2>();

        /// <summary>
        /// Keyboard: Spacebar
        /// </summary>
        public void OnJump(CallbackContext context) => jump = context.performed;

        /// <summary>
        /// Mouse: Left click
        /// </summary>
        public void OnBreak(CallbackContext context) {
            /*if(!GameController.Instance.ViewingOverview && context.performed) {
                Tilemaps.RemoveTile(GameController.Instance.mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            }*/
            float val = context.ReadValue<float>();
            breakPress = val >= InputSystem.settings.defaultButtonPressPoint ? val : 0f;
        }

        /// <summary>
        /// Mouse: Right click
        /// </summary>
        public void OnAct(CallbackContext context) {
            /*if(!GameController.Instance.ViewingOverview && context.performed) {

            }*/
            float val = context.ReadValue<float>();
            actPress = val >= InputSystem.settings.defaultButtonPressPoint ? val : 0f;
        }
    }
}
