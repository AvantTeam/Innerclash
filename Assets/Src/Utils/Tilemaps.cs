using System;
using UnityEngine;
using Innerclash.Core;
using Innerclash.Entities;
using Innerclash.World;

namespace Innerclash.Utils {
    public static class Tilemaps {
        public static Vector3Int CellPos(Vector2 rawPos) => GameController.Instance.tilemap.WorldToCell(rawPos);

        public static ScriptedTile GetTile(Vector2 rawPos) => GameController.Instance.tilemap.GetTile<ScriptedTile>(CellPos(rawPos));

        /// <returns> Whether the entity's X velocity should be forcibly stopped </returns>
        public static bool ApplyTile(Vector2 rawPos, PhysicsTrait entity) {
            var pos = CellPos(rawPos);
            var tile = GameController.Instance.tilemap.GetTile<ScriptedTile>(pos);

            return tile != null && tile.Apply(entity, pos);
        }

        public static void WithTile(Vector2 rawPos, Action<ScriptedTile, Vector3Int> action) {
            var pos = CellPos(rawPos);
            var tile = GameController.Instance.tilemap.GetTile<ScriptedTile>(pos);

            if(tile != null) action(tile, pos);
        }

        public static void RemoveTile(Vector2 rawPos) {
            var pos = CellPos(rawPos);
            var map = GameController.Instance.tilemap;
            var tile = map.GetTile<ScriptedTile>(pos);
            
            if(tile != null) {
                map.SetTile(pos, null);
                if(tile.itemDrop.item != null) {
                    tile.itemDrop.item.Create(new Vector2(pos.x + 0.5f, pos.y + 0.5f), tile.itemDrop.amount);
                }

                foreach(var offset in Masks.mask8) {
                    map.RefreshTile(pos + (Vector3Int)offset);
                }
            }
        }
    }
}
