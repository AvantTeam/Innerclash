using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Core;
using Innerclash.World;

namespace Innerclash.Entities {
    public class TilesTrait : MonoBehaviour {
        public float placeRange = 5f;
        public float destroyRange = 5f;

        public void DestroyTile(Tilemap tilemap, int x, int y) {
            ScriptedTile tile = tilemap.GetTile<ScriptedTile>(new Vector3Int(x, y, 0));
            if(tile != null && tile.itemDrop.item != null && tile.itemDrop.amount > 0) {
                var ent = Instantiate(Context.Instance.itemEntity, new Vector3(x, y, 0f), Quaternion.identity);

                var trait = ent.GetComponent<ItemTrait>();
                trait.stack = tile.itemDrop;
            }
        }
    }
}
