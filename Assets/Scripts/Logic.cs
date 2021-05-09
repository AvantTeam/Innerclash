using UnityEngine;
using Innerclash.Entities;

namespace Innerclash {
    public class Logic : MonoBehaviour {
        public static Logic Instance { get; private set; }

        public Camera mainCamera;
        public PlayerController player;
        public EntityControllable playerSpawnEntity;
        public Grid grid;

        private void Awake() {
            Instance = this;

            player.controllable = Instantiate(playerSpawnEntity, Vector3.zero, Quaternion.identity);
        }
    }
}
