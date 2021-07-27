using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Core;
using Innerclash.World.Map;

using static Innerclash.World.Map.WorldMapGenerator;

namespace Innerclash.World {
    public class SectorGenerator : MonoBehaviour {
        [Range(0f, 1f)] public float samplingRadius = 0.33f;
        public int terrainDeviation = 10;
        public float terrainRoughness = 20f;
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
            var infoObj = GameObject.FindWithTag("WorldDataInfo");
            WorldDataInfo worldData = infoObj.GetComponent<WorldDataInfo>();

            for(int column = 0; column < mapSize.x; column++) {
                Vector2 phase = new Vector2(Mathf.Cos((float)column / mapSize.x * 2f * Mathf.PI), Mathf.Sin((float)column / mapSize.x * 2f * Mathf.PI)) * samplingRadius;
                Vector2 samplingPoint = worldData.CurrentSectorWorldPosition + phase;
                BiomeData sample = worldData.BiomeDataAt(samplingPoint);
                int height = (int)Mathf.Min(sample.Height * mapSize.y + (-1f + 2f * Mathf.PerlinNoise(samplingPoint.x * terrainRoughness, samplingPoint.y * terrainRoughness)) * terrainDeviation, mapSize.y);
                if(column == mapSize.x / 2) spawnHeight = height + 1;
                for(int y = 0; y < height; y++) {
                    tiles.Add(new Vector3Int(column, y, 0), baseTile);
                }
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
        }

        void ResetPlayerPosition() {
            GameController.Instance.controlled.transform.position = new Vector3(mapSize.x / 2, spawnHeight);
        }
    }
}
