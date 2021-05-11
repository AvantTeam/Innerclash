using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Util;

namespace Innerclash.Core {
    [CreateAssetMenu]
    public class MaskedTile : ScriptedTile {
        public Sprite[] sprites;
        public SpriteVariator[] variators;

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            for(int x = -1; x <= 1; x++) {
                for(int y = -1; y <= 1; y++) {
                    Vector3Int next = position + new Vector3Int(x, y, 0);
                    if(SameTile(next, tilemap)) tilemap.RefreshTile(next);
                }
            }
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);

            int mask = MaskUtils.GetMask((x, y) => SameTile(position + new Vector3Int(x, y, 0), tilemap));
            tileData.sprite = sprites[MaskUtils.tileMap[mask]];
            foreach(SpriteVariator variator in variators) {
                if(mask == variator.maskTarget) {
                    variator.Evaluate(position, ref tileData);
                }
            }
        }

        private bool SameTile(Vector3Int position, ITilemap tilemap) => tilemap.GetTile(position) == this;

        [System.Serializable]
        public class SpriteVariator {
            public int maskTarget;
            public VariatorMode mode;
            [Range(0, 1)] public float variationThreshold;
            public float variationDensity;
            public WeightedSpriteEntry[] variationSprites;

            public void Evaluate(Vector3Int position, ref TileData tileData) {
                switch (mode) {
                    case VariatorMode.RotateQuarter: {
                            if(GetValueAt(position, variationDensity) > variationThreshold) {
                                Matrix4x4 m = tileData.transform;
                                m.SetTRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f * Mathf.Floor(1f + 3f * GetValueAt(position, 1.618f))), Vector3.one);
                                tileData.transform = m;
                                tileData.flags = TileFlags.LockTransform;
                            }
                            break;
                        }
                    case VariatorMode.RotateHalf: {
                            if (GetValueAt(position, variationDensity) > variationThreshold) {
                                Matrix4x4 m = tileData.transform;
                                m.SetTRS(Vector3.zero, Quaternion.Euler(0f, 0f, 180f * Mathf.Floor(1f + GetValueAt(position, 1.618f))), Vector3.one);
                                tileData.transform = m;
                                tileData.flags = TileFlags.LockTransform;
                            }
                            break;
                        }
                    case VariatorMode.FlipHorizontal: {
                            if (GetValueAt(position, variationDensity) > variationThreshold) {
                                Matrix4x4 m = tileData.transform;
                                m.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(-1f, 1f, 1f));
                                tileData.transform = m;
                                tileData.flags = TileFlags.LockTransform;
                            }
                            break;
                        }
                    case VariatorMode.FlipVertical: {
                            if (GetValueAt(position, variationDensity) > variationThreshold) {
                                Matrix4x4 m = tileData.transform;
                                m.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1f, -1f, 1f));
                                tileData.transform = m;
                                tileData.flags = TileFlags.LockTransform;
                            }
                            break;
                        }
                    case VariatorMode.FromVariationSprites: {
                            if (GetValueAt(position, variationDensity) > variationThreshold) {
                                int totalWeight = 0;
                                foreach (WeightedSpriteEntry entry in variationSprites) totalWeight += entry.weight;
                                int val = (int)(totalWeight * GetValueAt(position, 1.618f));
                                foreach(WeightedSpriteEntry entry in variationSprites) {
                                    if(val < entry.weight) {
                                        tileData.sprite = entry.sprite;
                                        break;
                                    } else {
                                        val -= entry.weight;
                                    }
                                }
                            }
                            break;
                        }
                }
            }

            private float GetValueAt(Vector3Int position, float scale) {
                return Mathf.Clamp(Mathf.PerlinNoise((float)position.x / scale, (float)position.y / scale), 0.001f, 0.999f);
            }

            public enum VariatorMode {
                RotateQuarter,
                RotateHalf,
                FlipHorizontal,
                FlipVertical,
                FromVariationSprites
            }

            [System.Serializable]
            public class WeightedSpriteEntry {
                public Sprite sprite;
                public int weight;
            }
        }
    }
}
