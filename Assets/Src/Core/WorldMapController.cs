using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Innerclash.World.Map;

using static UnityEngine.InputSystem.InputAction;

namespace Innerclash.Core {
    public class WorldMapController : MonoBehaviour {
        [Header("Constant Components")]
        public Camera mainCamera;
        public Tilemap worldSectors;
        public float cameraSpeed = 10f;

        public SectorPreset[] presets;
        public SectorPreset startSector;

        Vector2 axis;
        readonly Dictionary<Vector3Int, object> sectors = new Dictionary<Vector3Int, object>();

        public Vector3Int Hovering { get; private set; }

        public static WorldMapController Instance { get; private set; }
        public WorldMapGenerator MapGenerator { get; private set; }

        WorldDataInfo worldData;

        void Awake() {
            Instance = this;
            MapGenerator = GetComponent<WorldMapGenerator>();
            var infoObj = GameObject.FindWithTag("WorldDataInfo");
            if(infoObj != null) {
                worldData = infoObj.GetComponent<WorldDataInfo>();
                if(worldData == null) {
                    worldData = infoObj.AddComponent<WorldDataInfo>();
                }
            } else {
                infoObj = new GameObject("WorldDataInfoObject");
                worldData = infoObj.AddComponent<WorldDataInfo>();
            }
            DontDestroyOnLoad(infoObj);

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

        void LoadSector(Vector3Int position) {
            worldData.SetData(this);
            SceneManager.LoadScene("SectorGen");
        }

        public void OnMove(CallbackContext context) => axis = context.ReadValue<Vector2>();

        public void OnClick(CallbackContext context) {
            if(context.performed) {
                LoadSector(Hovering);
            }
        }

        [System.Serializable]
        public struct SectorPreset {
            public Vector3Int position;
        }
    }
}
