using System;

namespace Innerclash.Utils {
    // Could've done `Pair<K, V>` instead.
    [Serializable]
    public struct ObjectIntPair<T> {
        public T item;
        public int value;

        public override string ToString() {
            return $"({item}: {value})";
        }
    }
}
