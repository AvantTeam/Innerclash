using UnityEngine;
using Innerclash.Utils;

using static UnityEngine.Texture;
using static Innerclash.Utils.Noises;
using static Innerclash.World.Map.Biome;

namespace Innerclash.World.Map {
    [RequireComponent(typeof(SpriteRenderer))]
    public class WorldMapGenerator : MonoBehaviour {
        public bool autoUpdate;

        public Sprite mask;

        public NoisePass
            heightPass,
            temperaturePass,
            archaicPass;

        public Gradient heightGradient;
        public float heightMultiplier;

        public AnimationCurve
            poleTemperatureCurve,
            poleArchaicDensityCurve;

        public Biome[] biomes;

        public float[,] HeightNoise { get; private set; }
        public float[,] TemperatureNoise { get; private set; }
        public float[,] ArchaicNoise { get; private set; }

        public SpriteRenderer Renderer { get; private set; }

        void Start() {
            Generate();
        }

        public void Generate() {
            Renderer = GetComponent<SpriteRenderer>();

            int width = (int)mask.rect.width;
            int height = (int)mask.rect.height;
            var texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;

            HeightNoise = Noises.GenNoiseMap(width, height, heightPass);
            TemperatureNoise = Noises.GenNoiseMap(width, height, temperaturePass);
            ArchaicNoise = Noises.GenNoiseMap(width, height, archaicPass);

            Color[] map = new Color[width * height];
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    float cheight = HeightNoise[x, y];
                    float pole = (float)y / height;
                    float ctemp = TemperatureNoise[x, y] * poleTemperatureCurve.Evaluate(pole);
                    float carch = ArchaicNoise[x, y] * poleArchaicDensityCurve.Evaluate(pole);

                    if(Structs.Find(biomes, b =>
                        Structs.Optional(b.attributes,
                            a => a.type == BiomeAttributeType.Height,
                            a => cheight >= a.min && cheight <= a.max
                        ) &&

                        Structs.Optional(b.attributes,
                            a => a.type == BiomeAttributeType.Temperature,
                            a => ctemp >= a.min && ctemp <= a.max
                        ) &&

                        Structs.Optional(b.attributes,
                            a => a.type == BiomeAttributeType.ArchaicDensity,
                            a => carch >= a.min && carch <= a.max
                        ),
                        out Biome target
                    )) {
                        map[x + y * width] = target.mapColor * mask.texture.GetPixel(x, y) * heightGradient.Evaluate(cheight) * heightMultiplier;
                    }else{
                        map[x + y * width] = Color.black * mask.texture.GetPixel(x, y);
                    }
                }
            }

            texture.SetPixels(map);
            texture.Apply();

            Renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f, 16f);
        }
    }
}
