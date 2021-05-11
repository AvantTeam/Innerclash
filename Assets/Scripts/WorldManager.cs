using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Innerclash {
    public class WorldManager : MonoBehaviour {
        public Dictionary<int, Tilemap> Tilemaps { get; private set; }

        private static WorldGenerator gen;

        private void Awake() {
            gen = Logic.Instance.worldGenerator;
        }

        public class WorldChunk {
            public byte Pos { get; private set; }
            public short[,] Tile { get; private set; }

            public WorldChunk(byte pos, Vector2Int dimension) {
                Pos = pos;
                Tile = new short[dimension.x, dimension.y];
            }

            public TileBase LocalTileAt(Vector2Int localPosition) {
                if (localPosition.x < 0 || localPosition.y < 0 || localPosition.x > Tile.GetUpperBound(0) || localPosition.y > Tile.GetUpperBound(1)) return null;
                return Logic.Instance.tilesArray[Tile[localPosition.x, localPosition.y]];
            }

            public TileBase GlobalTileAt(Vector2Int position) {
                return LocalTileAt(position - Vector2Int.right * gen.mapDimension.chunkSize * (position.x % gen.mapDimension.chunkSize));
            }
        }
    }
}
