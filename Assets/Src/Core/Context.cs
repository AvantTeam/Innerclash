using UnityEngine;
using UnityEngine.Tilemaps;

namespace Innerclash.Core {
    public class Context : MonoBehaviour {
        public Tilemap tilemap;
        public Camera mainCamera;
        public Player player;

        public static Context Instance { get; private set; }

        void Awake() {
            Instance = this;
        }
    }
}
