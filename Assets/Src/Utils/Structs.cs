using System;
using System.Collections.Generic;

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

        public static int IndexOf<T>(IList<T> array, T value) {
            for(int i = 0; i < array.Count; i++) {
                if(ReferenceEquals(array[i], value)) return i;
            }

            return -1;
        }

        public static T WeightedRandom<T>(ObjectIntPair<T>[] weightedArray) {
            if(weightedArray.Length <= 0) return default;
            int total = 0;
            foreach(ObjectIntPair<T> pair in weightedArray) total += pair.value;
            int rand = UnityEngine.Random.Range(0, total);
            foreach(ObjectIntPair<T> pair in weightedArray) {
                if(rand < pair.value) return pair.item;
                rand -= pair.value;
            }
            UnityEngine.Debug.LogWarning($"Failed to select weighted random element from weighted array {weightedArray}");
            return default;
        }

        public static string ToString<T>(IList<T> array) => ToString(array, ", ", t => t.ToString());

        public static string ToString<T>(IList<T> array, string separator) => ToString(array, separator, t => t.ToString());

        public static string ToString<T>(IList<T> array, Func<T, string> stringifier) => ToString(array, ", ", stringifier);

        public static string ToString<T>(IList<T> array, string separator, Func<T, string> stringifier) {
            var res = "[";
            for(int i = 0; i < array.Count; i++) {
                if(i > 0) res += separator;
                res += stringifier(array[i]);
            }

            return res + "]";
        }
    }
}
