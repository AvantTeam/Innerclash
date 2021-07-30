using UnityEngine;

namespace Innerclash.Utils {
    /// <summary> Utility class for providing octave perlin noise </summary>
    public static class Noises {
        public static float PerlinNoise(float x, float y, NoisePass pass) {
            return PerlinNoise(x, y, pass.scale, pass.octaves, pass.persistence, pass.lacunarity);
        }

        public static float PerlinNoise(float x, float y, float scale, int octaves, float persistence, float lacunarity) {
            float amplitude = 1f;
            float frequency = 1f;
            float nheight = 0f;

            for(int i = 0; i < octaves; i++) {
                float val = Mathf.PerlinNoise(x / scale * frequency, y / scale * frequency) * 2f - 1f;

                nheight += val * amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return nheight;
        }

        public static float[,] GenNoiseMap(int width, int height, NoisePass pass, AnimationCurve remap = null) {
            return GenNoiseMap(width, height, pass.scale, pass.octaves, pass.persistence, pass.lacunarity, remap);
        }

        public static float[,] GenNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity, AnimationCurve remap = null) {
            float min = float.MaxValue;
            float max = float.MinValue;

            float[,] res = new float[width, height];
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    float noise = PerlinNoise(x, y, scale, octaves, persistence, lacunarity);
                    if(noise > max) {
                        max = noise;
                    } else if(noise < min) {
                        min = noise;
                    }
                    res[x, y] = noise;
                }
            }

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    res[x, y] = Mathf.InverseLerp(min, max, res[x, y]);
                    if(remap != null) res[x, y] = remap.Evaluate(res[x, y]);
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
