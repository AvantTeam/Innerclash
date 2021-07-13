using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Utils;

namespace Innerclash.Content {
    [CreateAssetMenu]
    public class ScriptedTile : Tile {
        public Sprite[] sprites;

        public TileRule rules;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);

            if(((int)rules & 7) > 0) {
                Matrix4x4 tr = tileData.transform;
                Vector3 pos = Vector3.zero;
                Quaternion rot = Quaternion.identity;
                Vector3 scl = Vector3.one;

                if(rules.HasFlag(TileRule.Rotate)) {
                    int dir = Random.Range(0, 4);
                    rot = Quaternion.Euler(0f, 0f, dir * 90f);
                }

                if(rules.HasFlag(TileRule.FlipX)) {
                    if(Random.Range(0f, 1f) > 0.5f) {
                        scl.x *= -1;
                    }
                }

                if(rules.HasFlag(TileRule.FlipY)) {
                    if(Random.Range(0f, 1f) > 0.5f) {
                        scl.y *= -1;
                    }
                }

                tr.SetTRS(pos, rot, scl);
                tileData.transform = tr;
                tileData.flags |= TileFlags.LockTransform;
            }

            if(rules.HasFlag(TileRule.Bitmask)) {
                int mask = Masks.tileMap[Masks.GetMask((x, y) => IsSameType(tilemap, x, y))];
                tileData.sprite = sprites[mask];
            }
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            foreach(var off in Masks.mask8) {
                int x = position.x + off.x;
                int y = position.y + off.y;

                if(IsSameType(tilemap, x, y)) {
                    tilemap.RefreshTile(new Vector3Int(x, y, 0));
                }
            }
        }

        public bool IsSameType(ITilemap tilemap, int x, int y) => tilemap.GetTile(new Vector3Int(x, y, 0)) == this;

        [System.Flags]
        public enum TileRule {
            None = 0,
            Rotate = 1,
            FlipX = 2,
            FlipY = 4,
            Bitmask = 8
        }
    }
}
