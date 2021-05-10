using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Entities;

namespace Innerclash {
    public class Logic : MonoBehaviour {
        public static Logic Instance { get; private set; }

        public Camera mainCamera;
        public PlayerController player;
        public EntityControllable playerSpawnEntity;
        public Tilemap tilemap;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            player.controllable = Instantiate(playerSpawnEntity, Vector3.zero, Quaternion.identity);
        }

        private void Update() {
            if(player.controllable != null) {
                Vector3 pos = player.controllable.transform.position;
                mainCamera.transform.position = new Vector3(pos.x, pos.y, mainCamera.transform.position.z);
            }
        }
    }
}
