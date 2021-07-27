using UnityEditor;
using UnityEngine;
using Innerclash.World.Map;

namespace Innerclash.Editors.World.Map {
    [CustomEditor(typeof(WorldMapGenerator))]
    public class WorldMapGeneratorEditor : Editor {
        public override void OnInspectorGUI() {
            if(target is WorldMapGenerator gen) {
                if((DrawDefaultInspector() && gen.autoUpdate) || GUILayout.Button("Generate")) {
                    gen.Generate();
                }
            }
        }
    }
}
