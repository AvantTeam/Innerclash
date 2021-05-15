using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Innerclash {
    public class WorldManager : MonoBehaviour {
        [SerializeField] private float chunkRefreshRate = 1f;
        [SerializeField] private int chunkLoadDistance = 2;

        public Dictionary<byte, WorldChunk> Chunks { get; private set; }
        private static WorldGenerator gen;
        private static Logic.WorldDimension worldDimension;

        private void Awake() {
            Chunks = new Dictionary<byte, WorldChunk>();
            gen = Logic.Instance.worldGenerator;
            worldDimension = Logic.Instance.worldDimension;
        }

        private void Start() {
            InvokeRepeating(nameof(RefreshChunk), 0f, chunkRefreshRate);
        }

        private void RefreshChunk() {
            if (Logic.Instance.player.controllable == null) return;
            Vector3 player = Logic.Instance.player.controllable.transform.position;
            int chunkPos = (int)(player.x / worldDimension.chunkSize);

            for (int offset = -chunkLoadDistance; offset <= chunkLoadDistance; offset++) {
                LoadChunk(Logic.Instance.tilemaps, chunkPos + offset);
            }
            UnloadChunk(Logic.Instance.tilemaps, chunkPos - chunkLoadDistance - 1);
            UnloadChunk(Logic.Instance.tilemaps, chunkPos + chunkLoadDistance + 1);
        }

        public void GenerateChunks(Vector2Int chunkDimension) {
            Tilemap[] tilemaps = Logic.Instance.tilemaps;
            byte id = 0;
            int startX = 0, largestTilemapSize = -1;
            foreach (Tilemap map in tilemaps) if (map.size.x > largestTilemapSize) largestTilemapSize = map.size.x;

            while (startX < largestTilemapSize) {
                WorldChunk chunk = new WorldChunk(id, chunkDimension);
                chunk.SaveChunkData(tilemaps, startX);
                Chunks.Add(id, chunk);
                id++;
                startX += chunkDimension.x;
            }
        }

        public void LoadChunk(Tilemap[] tilemaps, int pos) {
            int relativePos = pos;
            while(relativePos < 0) relativePos += Chunks.Count;
            byte id = (byte)(relativePos % Chunks.Count);
            int origin = id * worldDimension.chunkSize;
            int offset = pos / Chunks.Count * worldDimension.chunkSize;

            Dictionary<TilemapLayer, TileBase[]> data = Chunks[id].LoadChunkData(tilemaps);
            for (int layer = 0; layer < tilemaps.Length; layer++) {
                TileBase[] tiles = data[(TilemapLayer)layer];
                BoundsInt fillBound = new BoundsInt(origin + offset, 0, 0, worldDimension.chunkSize, worldDimension.worldHeight, 1);
                tilemaps[layer].SetTilesBlock(fillBound, tiles);
            }
        }

        public void UnloadChunk(Tilemap[] tilemaps, int pos) {
            byte id = (byte)(pos % Chunks.Count);
            int origin = id * worldDimension.chunkSize;
            int offset = pos / Chunks.Count * worldDimension.chunkSize;

            foreach (Tilemap map in tilemaps) {
                map.BoxFill(new Vector3Int(origin + offset, 0, 0), null, origin + offset, 0, origin + offset + worldDimension.chunkSize, worldDimension.worldHeight);
            }
        }

        public class WorldChunk {
            public byte ID { get; private set; }
            public Dictionary<TilemapLayer, TileBase[]> Tiles { get; private set; }

            public Vector2Int Dimension { get; private set; }

            public WorldChunk(byte id, Vector2Int dimension) {
                ID = id;
                Tiles = new Dictionary<TilemapLayer, TileBase[]>();
                Dimension = dimension;
                for (int layer = 0; layer <= 2; layer++) {
                    Tiles[(TilemapLayer)layer] = new TileBase[dimension.x * dimension.y];
                }
            }

            /*public TileBase LocalTileAt(Vector2Int localPosition, TilemapLayer layer) {
                if (localPosition.x < 0 || localPosition.y < 0 || localPosition.x >= worldDimension.chunkSize || localPosition.y >= worldDimension.worldHeight) return null;
                return Logic.Instance.tilesArray[Tile[layer][localPosition.x, localPosition.y]];
            }*/

            public void SaveChunkData(Tilemap[] tilemaps, int startX) {
                for (int layer = 0; layer < tilemaps.Length; layer++) {
                    Tiles[(TilemapLayer)layer] = tilemaps[layer].GetTilesBlock(new BoundsInt(startX, 0, 0, worldDimension.chunkSize, worldDimension.worldHeight, 1));
                }
            }

            public Dictionary<TilemapLayer, TileBase[]> LoadChunkData(Tilemap[] tilemaps) {
                /*Dictionary<TilemapLayer, TileBase[][]> res = new Dictionary<TilemapLayer, TileBase[][]>();
                for (int layer = 0; layer < tilemaps.Length; layer++) {
                    TileBase[][] temp = new TileBase[worldDimension.chunkSize][];
                    for (int x = 0; x < Dimension.x; x++) {
                        for (int i = 0; i < temp.Length; i++) temp[i] = new TileBase[worldDimension.worldHeight];
                        for (int y = 0; y < Dimension.y; y++) {
                            TileBase tile = Logic.Instance.TileOfID(Tile[(TilemapLayer)layer][x, y]);
                            temp[x][y] = tile;
                        }
                    }
                    res.Add((TilemapLayer)layer, temp);
                }
                return res;*/
                return Tiles;
            }
        }
    }
}
