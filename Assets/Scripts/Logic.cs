using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Entities;

namespace Innerclash {
    public class Logic : MonoBehaviour {
        public static Logic Instance { get; private set; }

        public CameraSettings cameraSettings;
        public PlayerController player;
        public EntityControllable playerSpawnEntity;
        public Tilemap tilemap;

        private void Awake() {
            Instance = this;
            Time.timeScale = 0f;
        }

        private void Start() {
            player.controllable = Instantiate(playerSpawnEntity);
            cameraSettings.followTarget = player.controllable.transform;

            ResetPosition();
        }

        private void LateUpdate() {
            if(cameraSettings.followTarget != null) {
                Transform cam = cameraSettings.mainCamera.transform;
                Transform tar = cameraSettings.followTarget;
                Vector2 newPos = Vector2.Lerp(cam.position, tar.position, cameraSettings.followSpeed * Time.deltaTime);

                cam.position = new Vector3(newPos.x, newPos.y, cam.position.z);
            }
        }

        public void TogglePause() {
            Time.timeScale = Time.timeScale < 1f ? 1f : 0f;
        }

        public void ResetPosition() {
            if(player.controllable != null) {
                player.controllable.transform.position = Vector3.zero;
            }
        }

        [System.Serializable]
        public class CameraSettings {
            public Camera mainCamera;
            public Transform followTarget;
            public float followSpeed = 2f;
        }
    }
}
