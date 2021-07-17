using UnityEngine;
using Innerclash.Entities;

namespace Innerclash.World.Tiles.Behaviours {
    [CreateAssetMenu(menuName = "Content/World/Tile Behaviours/Movement Behaviour")]
    public class MovementBehaviour : TileBehaviour {
        public float friction;

        public override void Apply(ScriptedTile tile, Vector3Int position, Entity entity) {
            Vector2 force = new Vector2(entity.Body.velocity.x * -1f, 0f).normalized * friction;
            entity.Body.AddForce(force * Time.fixedDeltaTime);
        }
    }
}
