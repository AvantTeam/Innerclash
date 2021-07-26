using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

using static UnityEngine.InputSystem.InputAction;

namespace Innerclash.Core {
    public class WorldMapController : MonoBehaviour {
        [Header("Constant Components")]
        public Camera mainCamera;
        public Tilemap worldSectors;
        public float cameraSpeed = 5f;

        public SectorPreset[] presets;
        public SectorPreset startSector;

        Vector2 axis;
        readonly Dictionary<Vector3Int, object> sectors = new Dictionary<Vector3Int, object>();

        public Vector3Int Hovering { get; private set; }

        public static WorldMapController Instance { get; private set; }

        void Awake() {
            Instance = this;

            foreach(var preset in presets) {
                sectors.Add(preset.position, null);
            }
        }

        void Update() {
            Hovering = worldSectors.WorldToCell(Mouse.current.position.ReadValue());
        }

        void FixedUpdate() {
            mainCamera.transform.position += cameraSpeed * Time.fixedDeltaTime * new Vector3(axis.x, axis.y);
        }

        public void OnMove(CallbackContext context) => axis = context.ReadValue<Vector2>();

        public void OnClick(CallbackContext context) {
            if(context.performed) {
                
            }
        }

        [System.Serializable]
        public struct SectorPreset {
            public Vector3Int position;
        }
    }
}
