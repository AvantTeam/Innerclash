using UnityEngine;
using UnityEngine.Tilemaps;

namespace Innerclash {
    public class WorldGenerator : MonoBehaviour {
        public bool randomSeed;
        public int seed;
        public WorldDimension mapDimension;
        public Vector2Int sectorCenter = Vector2Int.zero;
        public int seaLevel = 200;

        public Tilemap[] worldTilemaps;
        public TileNoisePass[] noisePasses;

        private System.Random rand;

        public void Initialize() {
            if (randomSeed) seed = System.DateTime.Now.GetHashCode();
            ResetGenerator();
        }

        public void ResetGenerator() {
            rand = new System.Random(seed);
        }

        public void GenerateMap() {
            ResetGenerator();
            foreach(TileNoisePass pass in noisePasses) {
                pass.offsets = new Vector2[pass.octave];
                for (int i = 0; i < pass.octave; i++) pass.offsets[i] = new Vector2((float)rand.NextDouble(), (float)rand.NextDouble()).normalized * (float)rand.NextDouble() * 100f;

                for(int x = 0; x < mapDimension.WorldWidth; x++) {
                    float sourcePhase = (float)x / mapDimension.WorldWidth;
                    float sourceAngle = sourcePhase * 2 * Mathf.PI;
                    Vector2 sourcePoint = new Vector2(Mathf.Cos(sourceAngle), Mathf.Sin(sourceAngle)) + sectorCenter;
                    float freq = 1f, amp = 1f, height = 0f;

                    for(int i = 0; i < pass.octave; i++) {
                        Vector2 offset = pass.offsets[i];
                        Vector2 octavePoint = sourcePoint * pass.scale * freq + offset;
                        float samplePoint = Mathf.PerlinNoise(octavePoint.x, octavePoint.y) * amp;
                        height += samplePoint;
                        freq *= pass.lacunarity;
                        amp *= pass.persistence;
                    }

                    int finalHeight = (int)Mathf.Min(Mathf.LerpUnclamped(pass.heightRange.x, pass.heightRange.y, pass.remap.Evaluate(height / 1.33f)), mapDimension.worldHeight - 1);
                    worldTilemaps[(int)pass.targetLayer].BoxFill(new Vector3Int(x, finalHeight, 0), pass.tile, x, 0, x, finalHeight);
                    if(pass.generateBackgroundTile)worldTilemaps[(int)TilemapLayer.background].BoxFill(new Vector3Int(x, finalHeight, 0), pass.tile, x, 0, x, finalHeight);
                }
            }
        }

        public static Vector3Int FindWorldCenter(Tilemap world) {
            Vector3Int pos = new Vector3Int(world.size.x / 2, world.size.y, 0);
            bool found = false;
            while(!found && pos.y > 0) {
                if(world.GetTile(pos + Vector3Int.down) != null) {
                    found = true;
                } else {
                    pos += Vector3Int.down;
                }
            }
            return pos;
        }

        [System.Serializable]
        public class WorldDimension {
            public int chunkSize = 50, worldChunkCount = 48, worldHeight = 500;
            public int WorldWidth { get => chunkSize * worldChunkCount; }
        }

        [System.Serializable]
        public class TileNoisePass {
            public TilemapLayer targetLayer;
            public TileBase tile;
            public bool generateBackgroundTile = true;
            public Vector2Int heightRange;
            public float scale = 1f;
            [Range(1, 16)] public int octave = 4;
            public float lacunarity = 1.75f;
            [Range(0, 1)] public float persistence = 0.5f;
            public AnimationCurve remap;

            internal Vector2[] offsets;
        }

        public enum TilemapLayer {
            front = 0, foreground = 1, background = 2
        }
    }
}
