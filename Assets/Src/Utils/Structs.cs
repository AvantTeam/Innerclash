using System;

namespace Innerclash.Utils {
    public static class Structs {
        public static bool Find<T>(T[] array, Predicate<T> pred, out T result) {
            foreach(var t in array) {
                if(pred(t)) {
                    result = t;
                    return true;
                }
            }

            result = default;
            return false;
        }

        public static int Select<T>(T[] array, Predicate<T> pred, T[] result) {
            int idx = 0;
            foreach(var t in array) {
                if(idx >= result.Length) break;
                if(pred(t)) result[idx++] = t;
            }

            return idx;
        }

        public static void Select<T>(T[] array, Predicate<T> pred, T[] result, Action<T> act) {
            int found = Select(array, pred, result);
            for(int i = 0; i < found; i++) {
                act(result[i]);
            }
        }

        public static bool Optional<T>(T[] array, Predicate<T> pred, Predicate<T> sec) {
            return !Find(array, pred, out T res) || sec(res);
        }

        public static T Random<T>(T[] array) {
            if(array.Length <= 0) return default;
            return array[UnityEngine.Random.Range(0, array.Length - 1)];
        }
    }
}
