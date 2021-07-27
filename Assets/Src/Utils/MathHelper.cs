using UnityEngine;

namespace Innerclash.Utils {
    public static class MathHelper {
        public static float Remap(float valueToRemap, float fromA, float fromB, float toA, float toB) {
            return Mathf.Lerp(toA, toB, Mathf.InverseLerp(fromA, fromB, valueToRemap));
        }
    }
}
