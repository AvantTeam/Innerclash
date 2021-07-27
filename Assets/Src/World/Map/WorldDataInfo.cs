using UnityEngine;
using Innerclash.Core;

namespace Innerclash.World.Map {
    public class WorldDataInfo : MonoBehaviour {
        public Vector3Int CurrentSectorPosition { get; private set; }
        public Vector2 CurrentSectorWorldPosition { get; private set; }

        float[,] height, temperature, archaic;

        public Color NoiseDataAt(int x, int y) {
            return new Color(height[x, y], temperature[x, y], archaic[x, y]);
        }

        public void SetData(WorldMapController controller) {
            CurrentSectorPosition = controller.Hovering;
            CurrentSectorWorldPosition = controller.worldSectors.CellToWorld(CurrentSectorPosition);

            height = controller.MapGenerator.HeightNoise;
            temperature = controller.MapGenerator.TemperatureNoise;
            archaic = controller.MapGenerator.ArchaicNoise;
        }
    }
}
