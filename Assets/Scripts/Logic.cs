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

        public WorldGenerator worldGenerator;

        public Vector3 Spawn { get; private set; }

        private void Awake() {
            Instance = this;
            Time.timeScale = 0f;
        }

        private void Start() {
            if(worldGenerator != null) {
                worldGenerator.Initialize();
                worldGenerator.GenerateMap();
                Spawn = worldGenerator.FindWorldCenter(tilemap) + Vector3.up + new Vector3(0.5f, 0.5f);
            }

            player.controllable = Instantiate(playerSpawnEntity);
            cameraSettings.followTarget = player.controllable.transform;

            ResetPosition();
        }

        private void FixedUpdate() {
            if(cameraSettings.followTarget != null) {
                Transform cam = cameraSettings.mainCamera.transform;
                Transform tar = cameraSettings.followTarget;
                Vector2 newPos = Vector2.Lerp(cam.position, tar.position, cameraSettings.followSpeed * Time.fixedDeltaTime);

                cam.position = new Vector3(newPos.x, newPos.y, cam.position.z);
            }
        }

        public void TogglePause() {
            Time.timeScale = Time.timeScale < 1f ? 1f : 0f;
        }

        public void ResetPosition() {
            if(player.controllable != null) {
                player.controllable.transform.position = Spawn;
                cameraSettings.mainCamera.transform.position = Spawn;
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
