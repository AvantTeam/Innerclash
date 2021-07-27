using UnityEngine;

namespace Innerclash.World.Map {
    [CreateAssetMenu(menuName = "Content/World/Map/Biome")]
    public class Biome : ScriptableObject {
        public Color mapColor = Color.black;
        public BiomeAttribute[] attributes;

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
    }
}
