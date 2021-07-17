using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Entities;
using Innerclash.Utils;
using Innerclash.World.Tiles;

namespace Innerclash.World {
    [CreateAssetMenu(menuName = "Content/World/Scripted Tile")]
    public class ScriptedTile : Tile {
        public Sprite[] sprites;

        [Header("Behaviour")]
        /// <summary> Rule mask. See docs of TileRule for more specifications </summary>
        public TileRule rules;
        /// <summary> Tile behaviours. These can affect tile datas on startup and entities on interaction </summary>
        public TileBehaviour[] behaviours;

        [Header("Material")]
        /// <summary> Friction used when the entity this tile is interacting with is not moving or is turning around </summary>
        public float staticFriction = 500f;
        /// <summary> Friction used when the entity this tile is interacting with is moving </summary>
        public float dynamicFriction = 5000f;
        /// <summary> When interacting with entity, the entity's X velocity will be clamped to 0 if the magnitude is lower than this </summary>
        [Range(0f, 1f)] public float frictionThreshold = 0.3f;
        [Range(0f, 1f)] public float frictionOffset = 0.6f;

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

        /// <summary> Call in MonoBehaviour#FixedUpdate() </summary>
        /// <returns> Whether the entity's X velocity should be forcibly stopped </returns>
        public bool Apply(Entity entity, Vector3Int position) {
            bool res = entity.WasMoving;

            // TODO there's got to be a better way than this
            var force = new Vector2(Mathf.Clamp(entity.Body.velocity.x, -1f, 1f) * -1f, 0f);
            if(force.magnitude > frictionThreshold) {
                float curve = frictionOffset + force.magnitude * (1f - frictionOffset);
                entity.Body.AddForce(curve * force * ((!entity.IsMoving || entity.IsTurning) ? staticFriction : dynamicFriction) * Time.fixedDeltaTime);
            } else if(!res && !entity.IsMoving) {
                res = true;
            }

            foreach(var behaviour in behaviours) {
                behaviour.Apply(this, position, entity);
            }

            return res;
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
