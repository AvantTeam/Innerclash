using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Entities;

namespace Innerclash.World.Tiles {
    /// <summary> Base class of all tile behaviours </summary>
    public class TileBehaviour : ScriptableObject {
        /// <summary> Modifies a TileData. Called in ScriptedTile#GetTileData() </summary>
        public virtual void Data(ScriptedTile tile, Vector3Int position, ITilemap tilemap, ref TileData tileData) { }

        /// <summary> Interacts with an entity. Called on a *fixed rate* in Entity#FixedUpdate() </summary>
        public virtual void Apply(ScriptedTile tile, Vector3Int position, PhysicsTrait entity) { }
    }
}
