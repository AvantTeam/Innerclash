using UnityEngine;

namespace Innerclash.Utils {
    public static class Masks {
        public static Vector2Int[] mask8 = new Vector2Int[] {
            Vector2Int.right,
            Vector2Int.right + Vector2Int.up,
            Vector2Int.up,
            Vector2Int.up + Vector2Int.left,
            Vector2Int.left,
            Vector2Int.left + Vector2Int.down,
            Vector2Int.down,
            Vector2Int.down + Vector2Int.right
        };

        public static int[] tileMap = new int[] {
            39, 36, 39, 36, 27, 16, 27, 24, 39, 36, 39, 36, 27, 16, 27, 24,
            38, 37, 38, 37, 17, 41, 17, 43, 38, 37, 38, 37, 26, 21, 26, 25,
            39, 36, 39, 36, 27, 16, 27, 24, 39, 36, 39, 36, 27, 16, 27, 24,
            38, 37, 38, 37, 17, 41, 17, 43, 38, 37, 38, 37, 26, 21, 26, 25,
             3,  4,  3,  4, 15, 40, 15, 20,  3,  4,  3,  4, 15, 40, 15, 20,
             5, 28,  5, 28, 29, 10, 29, 23,  5, 28,  5, 28, 31, 11, 31, 32,
             3,  4,  3,  4, 15, 40, 15, 20,  3,  4,  3,  4, 15, 40, 15, 20,
             2, 30,  2, 30,  9, 47,  9, 22,  2, 30,  2, 30, 14, 44, 14,  6,
            39, 36, 39, 36, 27, 16, 27, 24, 39, 36, 39, 36, 27, 16, 27, 24,
            38, 37, 38, 37, 17, 41, 17, 43, 38, 37, 38, 37, 26, 21, 26, 25,
            39, 36, 39, 36, 27, 16, 27, 24, 39, 36, 39, 36, 27, 16, 27, 24,
            38, 37, 38, 37, 17, 41, 17, 43, 38, 37, 38, 37, 26, 21, 26, 25,
             3,  0,  3,  0, 15, 42, 15, 12,  3,  0,  3,  0, 15, 42, 15, 12,
             5,  8,  5,  8, 29, 35, 29, 33,  5,  8,  5,  8, 31, 34, 31,  7,
             3,  0,  3,  0, 15, 42, 15, 12,  3,  0,  3,  0, 15, 42, 15, 12,
             2,  1,  2,  1,  9, 45,  9, 19,  2,  1,  2,  1, 14, 18, 14, 13
        };

        public static int GetMask(MaskPredicate pred) {
            int mask = 0;
            for(int i = 0; i < mask8.Length; i++) {
                var vec = mask8[i];
                if(pred(vec.x, vec.y)) mask |= 1 << i;
            }

            return mask;
        }

        public delegate bool MaskPredicate(int x, int y);
    }
}
