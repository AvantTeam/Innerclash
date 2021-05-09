using UnityEngine;
using UnityEngine.Tilemaps;

namespace Innerclash.Core {
    [CreateAssetMenu]
    public class ScriptedTile : TileBase {
        public PhysicsMaterial2D material;

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject obj) {
            obj.GetComponent<Collider2D>().sharedMaterial = material;
            obj.layer = 3;

            return base.StartUp(position, tilemap, obj);
        }
    }
}
