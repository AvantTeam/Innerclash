using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Util;

namespace Innerclash.Core {
    [CreateAssetMenu]
    public class MaskedTile : ScriptedTile {
        public Sprite[] sprites;

        public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
            for(int x = -1; x <= 1; x++) {
                for(int y = -1; y <= 1; y++) {
                    Vector3Int next = position + new Vector3Int(x, y, 0);
                    if(SameTile(next, tilemap)) tilemap.RefreshTile(next);
                }
            }
        }

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = sprites[MaskUtils.tileMap[
                MaskUtils.GetMask((x, y) => SameTile(position + new Vector3Int(x, y, 0), tilemap))
            ]];
        }

        private bool SameTile(Vector3Int position, ITilemap tilemap) => tilemap.GetTile(position) == this;
    }
}
