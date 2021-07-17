using System;
using UnityEngine;
using Innerclash.Core;
using Innerclash.Entities;
using Innerclash.World;

namespace Innerclash.Utils {
    public static class Tilemaps {
        public static Vector3Int CellPos(Vector2 rawPos) => Context.Instance.tilemap.WorldToCell(rawPos);

        public static ScriptedTile GetTile(Vector2 rawPos) => Context.Instance.tilemap.GetTile<ScriptedTile>(CellPos(rawPos));

        /// <returns> Whether the entity's X velocity should be forcibly stopped </returns>
        public static bool ApplyTile(Vector2 rawPos, Entity entity) {
            var pos = CellPos(rawPos);
            var tile = Context.Instance.tilemap.GetTile<ScriptedTile>(pos);

            return tile != null && tile.Apply(entity, pos);
        }

        public static void WithTile(Vector2 rawPos, Action<ScriptedTile, Vector3Int> action) {
            var pos = CellPos(rawPos);
            var tile = Context.Instance.tilemap.GetTile<ScriptedTile>(pos);

            if(tile != null) action(tile, pos);
        }
    }
}
