using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Entities;
using Innerclash.Utils;
using Innerclash.World.Tiles;

namespace Innerclash.World {
    [CreateAssetMenu(menuName = "Content/World/Scripted Tile")]
    public class ScriptedTile : Tile {
        public Sprite[] sprites;

        // Rule mask. See docs of TileRule for more specifications
        public TileRule rules;

        // Tile behaviours. These can affect tile datas on startup and entities on interaction
        public TileBehaviour[] behaviours;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);

            // Get the tile mask if has the bitmask rule
            int mask = 0;
            if(rules.HasFlag(TileRule.Bitmask)) {
                mask = Masks.GetMask((x, y) => IsSameType(tilemap, x, y));
                tileData.sprite = sprites[Masks.tileMap[mask]];
            }

            // Check if there is any transform-locking rules
            // 7 = Rotate | FlipX | FlipY
            // Will be ignored if has the bitmask rule and the tile isn't surrounded
            if((!rules.HasFlag(TileRule.Bitmask) || mask == Masks.surround8) && ((int)rules & 7) > 0) {
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

            if(behaviours != null) {
                foreach(var behaviour in behaviours) {
                    behaviour.Data(this, position, tilemap, ref tileData);
                }
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

        // Makes the tile's behaviours interact with the entity (e.g. lowering it's speed, health, and such)
        public void Apply(Entity entity, Vector3Int position) {
            if(behaviours != null) {
                foreach(var behaviour in behaviours) {
                    behaviour.Apply(this, position, entity);
                }
            }
        }

        // Mask for tile rule specifications
        [System.Flags]
        public enum TileRule {
            None = 0, // Specifies none. This is the default rule
            Rotate = 1, // Randoms the tile matrix's rotation. Won't affect the rotation of non-center tiles if Bitmask mask rule is present
            FlipX = 2, // Gives a 50% chance to scale the tile matrix's X scale by -1. Won't affect the rotation of non-center tiles if Bitmask mask rule is present
            FlipY = 4, // Gives a 50% chance to scale the tile matrix's Y scale by -1. Won't affect the rotation of non-center tiles if Bitmask mask rule is present
            Bitmask = 8 // Enables tile bitmasking
        }
    }
}
