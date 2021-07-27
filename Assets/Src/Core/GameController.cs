using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Innerclash.Entities;
using Innerclash.UI.Fragments.Overview;
using Innerclash.Utils;

using static UnityEngine.InputSystem.InputAction;
using static Innerclash.Misc.Item;

namespace Innerclash.Core {
    public class GameController : MonoBehaviour {
        [Header("Constant components")]
        public Tilemap tilemap;
        public Camera mainCamera;

        [Header("Default Prefabs")]
        public GameObject itemEntity;

        [Header("Constant UI fragments")]
        public GameObject cursorItem;
        public GameObject hoverPanel;
        public GameObject overviewFragment;
        public InventoryFragment inventoryFragment;
        public ControllableTrait controlled;

        Vector2 moveAxis;
        bool jump;
        float breakPress, actPress;

        public ItemStack CurrentStack { get; set; }
        public bool HoldingStack { get => CurrentStack.item != null && CurrentStack.amount > 0; }

        public static GameController Instance { get; private set; }

        public bool ViewingOverview { get => overviewFragment.activeInHierarchy; }

        void Awake() {
            Instance = this;
        }

        void Start() {
            CurrentStack = new ItemStack();
        }

        void Update() {
            // Update selected item on cursor
            cursorItem.transform.position = Mouse.current.position.ReadValue();

            var image = cursorItem.GetComponentInChildren<Image>();
            var text = cursorItem.GetComponentInChildren<Text>();
            if(HoldingStack) {
                image.enabled = true;
                image.sprite = CurrentStack.item.sprite;

                text.enabled = true;
                text.text = CurrentStack.amount.ToString();
            } else {
                image.enabled = false;
                text.enabled = false;
            }

            breakPress = ViewingOverview ? 0f : breakPress;
            actPress = ViewingOverview ? 0f : actPress;

            if(breakPress > 0f) {
                Tilemaps.RemoveTile(mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            }
            if(actPress > 0f) {

            }
        }

        void FixedUpdate() {
            var pos = controlled.transform.position;
            mainCamera.transform.position = new Vector3(pos.x, pos.y, -10f);

            controlled.OnMove(moveAxis);
            controlled.OnJump(jump);
            controlled.OnBreak(breakPress);
            controlled.OnAct(actPress);
        }

        public void OnOpenOverview(CallbackContext context) {
            if(context.performed) {
                bool v = !ViewingOverview;

                overviewFragment.SetActive(v);
                inventoryFragment.OnVisibilityChange(v);
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
            float val = context.ReadValue<float>();
            breakPress = val >= InputSystem.settings.defaultButtonPressPoint ? val : 0f;
        }

        /// <summary>
        /// Mouse: Right click
        /// </summary>
        public void OnAct(CallbackContext context) {
            float val = context.ReadValue<float>();
            actPress = val >= InputSystem.settings.defaultButtonPressPoint ? val : 0f;
        }
    }
}
