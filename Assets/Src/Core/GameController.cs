using UnityEngine;
using UnityEngine.Tilemaps;

using static UnityEngine.InputSystem.InputAction;

namespace Innerclash.Core {
    public class GameController : MonoBehaviour {
        public Tilemap tilemap;
        public Camera mainCamera;
        public Player player;

        public GameObject itemEntity;

        public GameObject overviewFragment;

        public static GameController Instance { get; private set; }

        public bool ViewingOverview { get => overviewFragment.activeInHierarchy; }

        void Awake() {
            Instance = this;
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
