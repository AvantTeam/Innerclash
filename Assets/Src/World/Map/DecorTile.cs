using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Utils;

namespace Innerclash.World.Map {
    [CreateAssetMenu(menuName = "Content/World/Map/Decor Tile")]
    public class DecorTile : Tile {
        public Sprite[] sprites;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = Structs.Random(sprites);
        }
    }
}
