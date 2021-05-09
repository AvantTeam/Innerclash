using UnityEngine;

namespace Innerclash {
    public class Logic : MonoBehaviour {
        public static Logic Instance { get; private set; }

        public Camera mainCamera;
        public GameObject player;
        public Grid grid;

        private void Awake() {
            Instance = this;
        }
    }
}
