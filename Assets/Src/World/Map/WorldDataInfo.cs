using UnityEngine;
using Innerclash.Core;
using Innerclash.Utils;

using static Innerclash.World.Map.WorldMapGenerator;

namespace Innerclash.World.Map {
    public class WorldDataInfo : MonoBehaviour {
        public Vector3Int CurrentSectorPosition { get; private set; }
        public Vector2 CurrentSectorWorldPosition { get; private set; }
        public BiomeData[,] WorldBiomeData { get; private set; }

        public BiomeData BiomeDataAt(Vector2 worldPos) {
            Vector2Int pixelPos = PixelCoordAt(worldPos);
            int x = pixelPos.x, y = pixelPos.y;
            return WorldBiomeData[x, y];
        }

        public void SetData(WorldMapController controller, Vector3Int current) {
            CurrentSectorPosition = current;
            CurrentSectorWorldPosition = controller.worldSectors.CellToWorld(CurrentSectorPosition);

            WorldBiomeData = controller.MapGenerator.worldBiomeData;
        }

        Vector2Int PixelCoordAt(Vector2 worldPos) {
            int x = (int)MathHelper.Remap(worldPos.x, -32f, 32f, 0f, WorldBiomeData.GetUpperBound(0)),
                y = (int)MathHelper.Remap(worldPos.y, -32f, 32f, 0f, WorldBiomeData.GetUpperBound(1));
            return new Vector2Int(x, y);
        }
    }
}
