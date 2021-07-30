using UnityEngine;
using Innerclash.Utils;

namespace Innerclash.World.Map {
    [CreateAssetMenu(menuName = "Content/World/Map/Biome")]
    public class Biome : ScriptableObject {
        [Header("Sector")]
        public Color mapColor = Color.black;
        public BiomeAttribute[] attributes;
        public BiomeTileProvider[] tileProviders;
        public int terrainDeviation = 5;
        public float terrainRoughness = 20f;

        [Header("World Map")]
        public ObjectIntPair<DecorTile>[] decors;
        public float decorDensity;

        [System.Serializable]
        public struct BiomeAttribute {
            public BiomeAttributeType type;

            [Range(0f, 1f)]
            public float min, max;
        }
        
        public enum BiomeAttributeType {
            Height,
            Temperature,
            ArchaicDensity
        }

        public DecorTile GetDecorTile() {
            if(decors == null || decors.Length < 1) return null;
            return Random.value < decorDensity ? Structs.WeightedRandom(decors) : null;
        }
    }
}
