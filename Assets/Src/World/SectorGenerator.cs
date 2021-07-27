using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Core;

namespace Innerclash.World {
    public class SectorGenerator : MonoBehaviour {
        Tilemap tilemap;

        void Start() {
            tilemap = GameController.Instance.tilemap;
        }

        public void GenerateSector() {

        }
    }
}
