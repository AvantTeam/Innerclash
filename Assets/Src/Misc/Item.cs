using UnityEngine;
using Innerclash.Core;
using Innerclash.Entities;

namespace Innerclash.Misc {
    [CreateAssetMenu(menuName = "Content/Misc/Item")]
    public class Item : ScriptableObject {
        public Sprite sprite;

        [Header("Entity")]
        /// <summary> How many items can one item entity contain </summary>
        public int maxStack = 100;
        /// <summary> Force impulsed when dropping this item </summary>
        public float randForce = 6f;

        public ItemTrait Create(Vector2 pos, int amount) {
            if(amount <= 0) return null;

            var obj = Instantiate(Context.Instance.itemEntity, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
            var trait = obj.GetComponent<ItemTrait>();
            trait.stack = new ItemStack(this, amount);

            obj.GetComponent<Rigidbody2D>().AddForce(
                new Vector2(
                    Random.Range(-1f, 1f) * randForce,
                    6f
                ),
                ForceMode2D.Impulse
            );

            return trait;
        }

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
