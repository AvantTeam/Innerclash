using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Entities;
using Innerclash.Utils;
using Innerclash.World.Tiles;

namespace Innerclash.World {
    [CreateAssetMenu(menuName = "Content/World/Scripted Tile")]
    public class ScriptedTile : Tile {
        public Sprite[] sprites;

        /// <summary> Rule mask. See docs of TileRule for more specifications </summary>
        public TileRule rules;

        /// <summary> Tile behaviours. These can affect tile datas on startup and entities on interaction </summary>
        public TileBehaviour[] behaviours;

        [Header("Material")]
        public float friction = 100f;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);

            // Get the tile mask if has the bitmask rule
            int mask = 0;
            if(rules.HasFlag(TileRule.Bitmask)) {
                mask = Masks.GetMask8((x, y) => IsSameType(tilemap, position.x + x, position.y + y));
                tileData.sprite = sprites[Masks.tileMap[mask]];
            }

            // Check if there is any transform-locking rules
            // Will be ignored if has the bitmask rule and the tile isn't surrounded
            var trns = tileData.transform;
            var pos = Vector3.zero;
            var rot = Quaternion.identity;
            var scl = Vector3.one;

            if((!rules.HasFlag(TileRule.Bitmask) || mask == Masks.surround8) && ((int)rules & 7) > 0) { // 7 = Rotate | FlipX | FlipY
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

                trns.SetTRS(pos, rot, scl);
            }

            tileData.transform = trns;
            tileData.flags |= TileFlags.LockTransform;

            foreach(var behaviour in behaviours) {
                behaviour.Data(this, position, tilemap, ref tileData);
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

        /// <summary>
        /// Makes the tile's behaviours interact with the entity (e.g. lowering it's speed, health, and such).
        /// Call this in MonoBehaviour#FixedUpdate()
        /// </summary>
        public void Apply(Entity entity, Vector3Int position) {
            var force = new Vector2(Mathf.Clamp(entity.Body.velocity.x, -1f, 1f) * -1f, 0f);
            entity.Body.AddForce(force * friction * Time.fixedDeltaTime);

            foreach(var behaviour in behaviours) {
                behaviour.Apply(this, position, entity);
            }
        }

        /// <summary> Mask for tile rule specifications </summary>
        [System.Flags]
        public enum TileRule {
            /// <summary> Specifies none. This is the default rule </summary>
            None = 0,
            /// <summary> Randoms the tile matrix's rotation. Won't affect non-center tiles if Bitmask mask rule is present </summary>
            Rotate = 1,
            /// <summary> Gives a 50% chance to scale the tile matrix's X scale by -1. Won't affect non-center tiles if Bitmask mask rule is present </summary>
            FlipX = 2,
            /// <summary> Gives a 50% chance to scale the tile matrix's Y scale by -1. Won't affect non-center tiles if Bitmask mask rule is present </summary>
            FlipY = 4,
            /// <summary> Enables tile bitmasking </summary>
            Bitmask = 8
        }
    }
}
