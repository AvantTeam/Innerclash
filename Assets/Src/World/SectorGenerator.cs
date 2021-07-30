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
        public int samplingInterval = 2;
        public Vector2Int mapSize = new Vector2Int(800, 300);

        public ScriptedTile baseTile;

        Tilemap tilemap;
        Dictionary<Vector3Int, ScriptedTile> tiles;

        int spawnHeight;

        void Start() {
            tilemap = GameController.Instance.tilemap;
            tiles = new Dictionary<Vector3Int, ScriptedTile>();

            GenerateSector();
            ResetPlayerPosition();
        }

        public void GenerateSector() {
            WorldDataInfo worldData;
            try {
                var infoObj = GameObject.FindWithTag("WorldDataInfo");
                worldData = infoObj.GetComponent<WorldDataInfo>();
                infoObj = worldData.gameObject;
            } catch(System.NullReferenceException) {
                Debug.LogError("No WorldDataInfo component found in scene; sector generation will be terminated.", this);
                return;
            }

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
                        int height = (int)Mathf.Min(used.Height * mapSize.y + (2f * Mathf.PerlinNoise(terrainDeviationPoint.x * used.TerrainRoughness, terrainDeviationPoint.y * used.TerrainRoughness) - 1f) * used.TerrainDeviation, mapSize.y);
                        if(column == mapSize.x / 2) spawnHeight = height + 1;
                        for(int y = 0; y < height; y++) {
                            tiles.Add(new Vector3Int(column, y, 0), baseTile);
                        }
                        column++;
                        if(column >= mapSize.x) break;
                    }
                }
                previous = data;
                prevColumn = sample;
                sample += Mathf.Min(sample + samplingInterval, mapSize.x - 1);
            }

            Vector3Int[] positionArray = new Vector3Int[tiles.Count];
            ScriptedTile[] tilesArray = new ScriptedTile[tiles.Count];
            int i = 0;
            foreach(KeyValuePair<Vector3Int, ScriptedTile> pair in tiles) {
                positionArray[i] = pair.Key;
                tilesArray[i] = pair.Value;
                i++;
            }

            tilemap.SetTiles(positionArray, tilesArray);
            tilemap.RefreshAllTiles();
        }

        void ResetPlayerPosition() {
            GameController.Instance.controlled.transform.position = new Vector3(mapSize.x / 2, spawnHeight);
        }
    }
}
