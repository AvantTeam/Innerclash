using UnityEngine;

namespace Innerclash.Misc {
    [CreateAssetMenu(menuName = "Content/Misc/Item")]
    public class Item : ScriptableObject {
        public Sprite sprite;
        public int maxStack = 100;

        [System.Serializable]
        public struct ItemStack {
            public Item item;
            public int amount;

            public ItemStack(Item item, int amount) {
                this.item = item;
                this.amount = amount;
            }
        }

        public class ItemInventory {
            public ItemStack[,] Items { get; private set; }
            public int Width { get; private set; }
            public int Height { get; private set; }

            public ItemInventory(int width, int height) {
                Items = new ItemStack[Width = width, Height = height];
            }
        }
    }
}
