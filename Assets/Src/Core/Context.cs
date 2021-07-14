using UnityEngine;
using UnityEngine.Tilemaps;

namespace Innerclash.Core {
    public class Context : MonoBehaviour {
        public Tilemap tilemap;
        public Camera mainCamera;

        public static Context Instance { get; private set; }

        private void Reset() {
            Instance = this;
        }
    }
}
