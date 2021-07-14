using UnityEngine;
using Innerclash.Entities;

namespace Innerclash.World.Tiles.Behaviours {
    [CreateAssetMenu(menuName = "Content/World/Tile Behaviours/Movement Behaviour")]
    public class MovementBehaviour : TileBehaviour {
        // TODO make this be able to slow down, increase slippery, etc. to entities
        public override void Apply(ScriptedTile tile, Vector3Int position, Entity entity) {

        }
    }
}
