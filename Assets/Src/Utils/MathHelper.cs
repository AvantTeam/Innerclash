using UnityEngine;

namespace Innerclash.Utils {
    public static class MathHelper {
        public static float Remap(float valueToRemap, float fromA, float fromB, float toA, float toB) {
            return Mathf.Lerp(toA, toB, Mathf.InverseLerp(fromA, fromB, valueToRemap));
        }

        public static Vector2 PolarToRect(float magnitude, float angle) {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * magnitude;
        }
    }
}
