using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Entities;

namespace Innerclash {
    [RequireComponent(typeof(WorldManager))]
    public class Logic : MonoBehaviour {
        public static Logic Instance { get; private set; }

        public AsyncExecutor Async { get; private set; }

        [Header("Player & Camera")]
        public CameraSettings cameraSettings;
        public PlayerController player;
        public EntityControllable playerSpawnType;

        [Header("World")]
        public bool generateWorld = true;
        public WorldDimension worldDimension;
        public WorldGenerator worldGenerator;
        public WorldManager worldManager;
        public Tilemap[] tilemaps;
        public TileBase[] tilesArray;

        public Dictionary<TileBase, short> tilesID;

        public Vector3 Spawn { get; private set; }

        private void Awake() {
            Instance = this;
            Async = new AsyncExecutor();
            Time.timeScale = 0f;

            worldManager = GetComponent<WorldManager>();

            tilesID = new Dictionary<TileBase, short>();
            for(int id = 0; id < tilesArray.Length; id++) {
                if (!tilesID.ContainsKey(tilesArray[id])){
                    tilesID.Add(tilesArray[id], (short)id);
                } else {
                    Debug.LogError($"Duplicate tile {tilesArray[id]} found for ID {tilesID[tilesArray[id]]} and {id}");
                }
            }
        }

        private void Start() {
            if(generateWorld && worldGenerator != null) {
                worldGenerator.Initialize();
                worldGenerator.GenerateMap();
            }/* else {
                Vector3Int pos = new Vector3Int(tilemap.size.x / 2, tilemap.size.y, 0);
                bool found = false;
                while(!found && pos.y > 0) {
                    if(tilemap.GetTile(pos + Vector3Int.down) != null) {
                        found = true;
                    } else {
                        pos += Vector3Int.down;
                    }
                }

                Spawn = new Vector3(pos.x, pos.y) + Vector3.up + new Vector3(0.5f, 0.5f);
            }*/
            Spawn = WorldGenerator.FindWorldCenter(tilemaps[(int)TilemapLayer.foreground]) + Vector3.up + new Vector3(0.5f, 0.5f);
            worldManager.GenerateChunks(new Vector2Int(worldDimension.chunkSize, worldDimension.worldHeight));
            foreach (Tilemap map in tilemaps) map.ClearAllTiles();

            player.controllable = Instantiate(playerSpawnType, Spawn, Quaternion.identity);
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
                player.controllable.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                cameraSettings.mainCamera.transform.position = Spawn + Vector3.back * 10;
            }
        }

        public TileBase ForegroundTileAt(Vector3 worldPosition) {
            Tilemap foreground = tilemaps[(int)TilemapLayer.foreground];
            return foreground.GetTile(foreground.WorldToCell(worldPosition));
        }

        public short GetTileID(TileBase tile) {
            if (tile != null && tilesID.ContainsKey(tile)) return tilesID[tile];
            return -1;
        }

        public TileBase TileOfID(short id) {
            if (id < 0) return null;
            return tilesArray[id];
        }

        [System.Serializable]
        public class CameraSettings {
            public Camera mainCamera;
            public Transform followTarget;
            public float followSpeed = 2f;
        }

        [System.Serializable]
        public class WorldDimension {
            public int chunkSize = 50, worldChunkCount = 48, worldHeight = 500;
            public int WorldWidth { get => chunkSize * worldChunkCount; }
        }
    }
}
