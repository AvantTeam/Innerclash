using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Entities;
using Innerclash.Utils;

using static Innerclash.Misc.Item;

namespace Innerclash.World {
    [CreateAssetMenu(menuName = "Content/World/Scripted Tile")]
    public class ScriptedTile : Tile {
        public Sprite[] sprites;

        [Header("Behaviour")]
        /// <summary> Rule mask. See docs of TileRule for more specifications </summary>
        public TileRule rules;
        /// <summary> Tile groups, mainly used for bit-masking </summary>
        public TileGroup groups;
        /// <summary> The item that drops when this tile is destroyed </summary>
        public ItemStack itemDrop;

        [Header("Material")]
        /// <summary> Friction used when the entity this tile is interacting with is not moving or is turning around </summary>
        public float staticFriction = 5000f;
        /// <summary> Friction used when the entity this tile is interacting with is moving </summary>
        public float dynamicFriction = 500f;

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
                var nscl = 1f / Mathf.PI;

                if(rules.HasFlag(TileRule.Rotate)) {
                    int dir = (int)(Mathf.Clamp01(Mathf.PerlinNoise(position.x * nscl, position.y * nscl)) * 3.999f);
                    rot = Quaternion.Euler(0f, 0f, dir * 90f);
                }

                if(rules.HasFlag(TileRule.FlipX) && Mathf.Clamp01(Mathf.PerlinNoise((position.x + 1) * nscl, (position.y + 2) * nscl)) > 0.5f) {
                    scl.x *= -1;
                }

                if(rules.HasFlag(TileRule.FlipY) && Mathf.Clamp01(Mathf.PerlinNoise((position.x + 3) * nscl, (position.y + 5) * nscl)) > 0.5f) {
                    scl.y *= -1;
                }

                trns.SetTRS(pos, rot, scl);
            }

            tileData.transform = trns;
            tileData.flags |= TileFlags.LockTransform;
        }

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            foreach(var off in Masks.mask8) {
                int x = position.x + off.x;
                int y = position.y + off.y;

                if(IsSameGroup(tilemap, x, y)) {
                    tilemap.RefreshTile(new Vector3Int(x, y, 0));
                }
            }
        }

        public bool IsSameGroup(ITilemap tilemap, int x, int y) {
            var tile = tilemap.GetTile<ScriptedTile>(new Vector3Int(x, y, 0));
            return tile != null && (groups & tile.groups) > 0;
        }

        public bool IsSameType(ITilemap tilemap, int x, int y) => tilemap.GetTile(new Vector3Int(x, y, 0)) == this;

        /// <summary> Applies this tile's material to the specified entity. Call in MonoBehaviour#FixedUpdate() </summary>
        public void Apply(PhysicsTrait entity, Vector3Int position) {
            var force = new Vector2(Mathf.Clamp(entity.Body.velocity.x, -1f, 1f) * -1f, 0f);
            force *= 0.5f + force.magnitude * 0.5f;

            entity.Body.AddForceAtPosition(
                AppliedFriction(entity) * Time.fixedDeltaTime * force,
                new Vector2(position.x + 0.5f, position.y + 0.5f)
            );
        }

        public float AppliedFriction(PhysicsTrait entity) => (!entity.IsMoving || entity.IsTurning) ? staticFriction : dynamicFriction;

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

        [System.Flags]
        public enum TileGroup {
            Stone = 1,
            Dirt = 2
        }
    }
}
