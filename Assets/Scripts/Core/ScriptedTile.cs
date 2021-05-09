using UnityEngine;
using UnityEngine.Tilemaps;

namespace Innerclash.Core {
    [CreateAssetMenu]
    public class ScriptedTile : Tile {
        public float drag = 1f;
        public float speedMult = 1f;
        public float jumpMult = 1f;

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject obj) {
            TileBehavior comp = obj.GetComponent<TileBehavior>();
            comp.drag = drag;
            comp.speedMult = speedMult;
            comp.jumpMult = jumpMult;

            return base.StartUp(position, tilemap, obj);
        }
    }
}
