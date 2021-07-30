using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Core;
using Innerclash.World.Map;
using Innerclash.Utils;

using static Innerclash.World.Map.WorldMapGenerator;

namespace Innerclash.World {
    public class SectorGenerator : MonoBehaviour {
        [Range(0f, 1f)] public float samplingRadius = 0.33f;
        public int samplingInterval = 3;
        public Vector2Int mapSize = new Vector2Int(1200, 300);
        public Vector2Int heightmapBounds = new Vector2Int(40, 250);

        public ScriptedTile baseTile;

        Tilemap tilemap;
        Dictionary<Vector3Int, TileInfo> tiles;

        void Start() {
            tilemap = GameController.Instance.tilemap;
            tiles = new Dictionary<Vector3Int, TileInfo>();

            GenerateSector();
            ResetPlayerPosition();
        }

        public void GenerateSector() {
            WorldDataInfo worldData;
            try {
                var infoObj = GameObject.FindWithTag("WorldDataInfo");
                worldData = infoObj.GetComponent<WorldDataInfo>();
                infoObj = worldData.gameObject.gameObject;
            } catch(System.NullReferenceException) {
                Debug.LogError("No WorldDataInfo component found in scene; sector generation will be terminated.", this);
                return;
            }

            // HEIGHTMAP PASS
            BiomeData? previous = null;
            int prevColumn = -1;
            for(int column = 0, sample = 0; column < mapSize.x;) {
                Vector2 phase = MathHelper.PolarToRect(samplingRadius, (float)sample / mapSize.x * 2f * Mathf.PI);
                Vector2 samplingPoint = worldData.CurrentSectorWorldPosition + phase;
                BiomeData data = worldData.BiomeDataAt(samplingPoint);
                if(previous != null) {
                    BiomeData prev = (BiomeData)previous;
                    while(column < sample) {
                        float t = Mathf.InverseLerp(prevColumn, sample, column);
                        BiomeData used = BiomeData.Lerp(prev, data, t);

                        Vector2 terrainDeviationPoint = worldData.CurrentSectorWorldPosition + MathHelper.PolarToRect(samplingRadius, (float)column / mapSize.x * 2f * Mathf.PI);
                        int height = (int)Mathf.Min(MathHelper.Remap(used.Height, 0f, 1f, heightmapBounds.x, heightmapBounds.y) + (2f * Mathf.PerlinNoise(terrainDeviationPoint.x * used.TerrainRoughness, terrainDeviationPoint.y * used.TerrainRoughness) - 1f) * used.TerrainDeviation, mapSize.y);
                        for(int y = 0; y < height; y++) {
                            tiles.Add(new Vector3Int(column, y, 0), new TileInfo(baseTile, used));
                        }
                        for(int y = height; y < mapSize.y; y++) {
                            tiles.Add(new Vector3Int(column, y, 0), new TileInfo(null, used));
                        }
                        column++;
                        if(column >= mapSize.x) break;
                    }
                }
                previous = data;
                prevColumn = sample;
                sample += Mathf.Min(sample + samplingInterval, mapSize.x - 1);
            }

            // BIOME TILE PROVIDER PASS
            for(int x = 0; x < mapSize.x; x++) {
                for(int y = 0; y < mapSize.y; y++) {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if(!tiles.ContainsKey(pos)) {
                        Debug.Log($"Entry {pos} not found, skipping");
                        continue;
                    }
                    TileInfo current = tiles[pos];
                    foreach(BiomeTileProvider prov in current.Data.Biome.tileProviders) {
                        if(prov.Act(tiles, pos)) Debug.Log($"Act returns true on {pos}");
                    }
                }
            }

            // GENERATION
            Vector3Int[] positionArray = new Vector3Int[tiles.Count];
            ScriptedTile[] tilesArray = new ScriptedTile[tiles.Count];
            int i = 0;
            foreach(KeyValuePair<Vector3Int, TileInfo> pair in tiles) {
                positionArray[i] = pair.Key;
                tilesArray[i] = pair.Value.Tile;
                i++;
            }

            tilemap.SetTiles(positionArray, tilesArray);
            tilemap.RefreshAllTiles();
        }

        void ResetPlayerPosition() {
            bool found = false;
            Vector3Int current = new Vector3Int(mapSize.x / 2, mapSize.y, 0);
            while(!found && current.y >= 0) {
                if(tilemap.GetTile(current + Vector3Int.down) != null) found = true;
                current += Vector3Int.down;
            }
            if(found) GameController.Instance.controlled.transform.position = current + Vector3.up * 2f;
        }

        public struct TileInfo {
            public ScriptedTile Tile { get; set; }
            public BiomeData Data { get; set; }

            public TileInfo(ScriptedTile tile, BiomeData data) {
                Tile = tile;
                Data = data;
            }
        }
    }
}
