using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
        public GameObject overviewFragment;

        public bool HoldingStack { get => CurrentStack.amount > 0; }
        public ItemStack CurrentStack { get; set; }

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
            cursorItem.transform.position = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

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
                overviewFragment.SetActive(!ViewingOverview);
            }
        }
    }
}
