using UnityEngine;

namespace Innerclash.Utils {
    public static class Noises {
        public static float[,] GenNoiseMap(int width, int height, NoisePass pass) {
            return GenNoiseMap(width, height, pass.scale, pass.octaves, pass.persistence, pass.lacunarity);
        }

        public static float[,] GenNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity) {
            float[,] res = new float[width, height];
            float min = float.MaxValue;
            float max = float.MinValue;

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    float amplitude = 1f;
                    float frequency = 1f;
                    float nheight = 0f;

                    for(int i = 0; i < octaves; i++) {
                        float val = Mathf.PerlinNoise(x / scale * frequency, y / scale * frequency) * 2f - 1f;

                        nheight += val * amplitude;
                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    if(nheight > max) {
                        max = nheight;
                    } else if(nheight < min) {
                        min = nheight;
                    }

                    res[x, y] = nheight;
                }
            }

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    res[x, y] = Mathf.InverseLerp(min, max, res[x, y]);
                }
            }

            return res;
        }

        [System.Serializable]
        public struct NoisePass {
            public float scale;
            public int octaves;
            public float persistence;
            public float lacunarity;
        }
    }
}
