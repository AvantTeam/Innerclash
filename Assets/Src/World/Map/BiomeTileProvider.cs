using System;
using System.Collections.Generic;
using UnityEngine;

using static Innerclash.World.SectorGenerator;

namespace Innerclash.World.Map {
    [Serializable]
    public class BiomeTileProvider {
        public ProviderType type;
        public ScriptedTile tile;

        // SURFACE TYPE FIELDS
        [Range(1, 30)] public int depth = 8;

        public bool Act(Dictionary<Vector3Int, TileInfo> tiles, Vector3Int position) {
            switch(type) {
                case ProviderType.surface:
                    Vector3Int airCheck = position + Vector3Int.up;
                    if(tiles[position].Tile != null && (!tiles.ContainsKey(airCheck) || tiles[airCheck].Tile == null)) {
                        for(int i = 0; i < depth; i++) {
                            if(position.y + depth < 0) break;
                            TileInfo info = tiles[position + Vector3Int.down * i];
                            info.Tile = tile;
                            tiles[position + Vector3Int.down * i] = info;
                        }
                        return true;
                    }
                    return false;
                default: return false;
            }
        }

        public enum ProviderType {
            surface = 0
        }
    }
}
