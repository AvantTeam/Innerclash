using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Entities;

namespace Innerclash.World.Tiles {
    // Base class of all tile behaviours
    public class TileBehaviour : ScriptableObject {
        public virtual void Data(ScriptedTile tile, Vector3Int position, ITilemap tilemap, ref TileData tileData) { }

        public virtual void Apply(ScriptedTile tile, Vector3Int position, Entity entity) { }
    }
}
