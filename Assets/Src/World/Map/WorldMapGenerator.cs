using UnityEngine;
using Innerclash.Utils;

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

        public BiomeData[,] worldBiomeData { get; private set; }
        public SpriteRenderer Renderer { get; private set; }

        void Start() {
            Generate();
        }

        public void Generate() {
            Renderer = GetComponent<SpriteRenderer>();

            int width = (int)mask.rect.width;
            int height = (int)mask.rect.height;
            var texture = new Texture2D(width, height) {
                filterMode = FilterMode.Point
            };
            worldBiomeData = new BiomeData[width, height];

            float[,]
                heightNoise = GenNoiseMap(width, height, heightPass),
                temperatureNoise = GenNoiseMap(width, height, temperaturePass),
                archaicNoise = GenNoiseMap(width, height, archaicPass);

            Color[] map = new Color[width * height];
            for(int y = 0; y < height; y++) {
                for(int x = 0; x < width; x++) {
                    float cheight = heightNoise[x, y];
                    float pole = (float)y / height;
                    float ctemp = temperatureNoise[x, y] * poleTemperatureCurve.Evaluate(pole);
                    float carch = archaicNoise[x, y] * poleArchaicDensityCurve.Evaluate(pole);

                    Biome target = null;
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
                        out target
                    )) {
                        map[x + y * width] = target.mapColor * mask.texture.GetPixel(x, y) * heightGradient.Evaluate(cheight) * heightMultiplier;
                    }else{
                        map[x + y * width] = Color.black * mask.texture.GetPixel(x, y);
                    }
                    worldBiomeData[x, y] = new BiomeData(target, cheight, ctemp, carch);
                }
            }

            texture.SetPixels(map);
            texture.Apply();

            Renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f, 16f);
        }

        public struct BiomeData {
            public Biome Biome { get; private set; }
            public float Height { get; private set; }
            public float Temperature { get; private set; }
            public float ArchaicDensity { get; private set; }

            public Color CondensedData { get => new Color(Height, Temperature, ArchaicDensity); }

            public BiomeData(Biome biome, float height, float temperature, float archaicDensity) {
                Biome = biome;
                Height = height;
                Temperature = temperature;
                ArchaicDensity = archaicDensity;
            }
        }
    }
}
