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
        }

        private void Start() {
            player.controllable = Instantiate(playerSpawnEntity, Vector3.zero, Quaternion.identity);
            cameraSettings.followTarget = player.controllable.transform;
        }

        /*private void Update() {
            
        }*/

        private void LateUpdate() {
            if(cameraSettings.followTarget != null) {
                Transform cam = cameraSettings.mainCamera.transform;
                Transform tar = cameraSettings.followTarget;
                Vector2 newPos = Vector2.Lerp(cam.position, tar.position, cameraSettings.followSpeed * Time.deltaTime);
                cam.position = new Vector3(newPos.x, newPos.y, cam.position.z);
                
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
