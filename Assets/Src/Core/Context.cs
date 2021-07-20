using UnityEngine;
using UnityEngine.Tilemaps;

using static UnityEngine.InputSystem.InputAction;

namespace Innerclash.Core {
    public class Context : MonoBehaviour {
        public Tilemap tilemap;
        public Camera mainCamera;
        public Player player;

        public GameObject itemEntity;

        public GameObject overviewFragment;

        public static Context Instance { get; private set; }

        void Awake() {
            Instance = this;
        }

        void FixedUpdate() {
            var pos = player.controlled.transform.position;
            mainCamera.transform.position = new Vector3(pos.x, pos.y, -10f);
        }

        public void OnOpenOverview(CallbackContext context) {
            if(context.performed) {
                overviewFragment.SetActive(!overviewFragment.activeInHierarchy);
            }
        }
    }
}
