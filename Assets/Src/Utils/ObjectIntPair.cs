using System;

namespace Innerclash.Utils {
    [Serializable]
    public struct ObjectIntPair<T> {
        public T item;
        public int value;

        public override string ToString() {
            return $"({item}: {value})";
        }
    }
}
