using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Innerclash.UI.Fragments.Overview;

using static UnityEngine.InputSystem.InputAction;
using static Innerclash.Misc.Item;

namespace Innerclash.Core {
    public class GameController : MonoBehaviour {
        [Header("Constant components")]
        public Tilemap tilemap;
        public Camera mainCamera;
        public Player player;

        [Header("Default Prefabs")]
        public GameObject itemEntity;

        [Header("Constant UI fragments")]
        public GameObject cursorItem;
        public GameObject hoverPanel;
        public GameObject overviewFragment;
        public InventoryFragment inventoryFragment;

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
        }

        void FixedUpdate() {
            var pos = player.controlled.transform.position;
            mainCamera.transform.position = new Vector3(pos.x, pos.y, -10f);
        }

        public void OnOpenOverview(CallbackContext context) {
            if(context.performed) {
                bool v = !ViewingOverview;

                overviewFragment.SetActive(v);
                inventoryFragment.OnVisibilityChange(v);
            }
        }
    }
}
