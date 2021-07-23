using System.Collections.Generic;
using UnityEngine;
using Innerclash.Core;
using Innerclash.Entities;

namespace Innerclash.Misc {
    [CreateAssetMenu(menuName = "Content/Misc/Item")]
    public class Item : ScriptableObject {
        public Sprite sprite;

        [Header("Behaviour")]
        /// <summary> How many items can one item entity or inventory contain </summary>
        public int maxStack = 100;
        /// <summary> Force impulsed when dropping this item </summary>
        public float randForce = 6f;
        /// <summary> Used for inventory calculations </summary>
        public float mass = 1f;

        public ItemTrait Create(Vector2 pos, int amount) {
            if(amount <= 0) return null;

            var obj = Instantiate(GameController.Instance.itemEntity, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
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

            public bool Full { get => amount >= item.maxStack; }

            public ItemStack(Item item, int amount) {
                this.item = item;
                this.amount = amount;
            }

            public static int Transfer(ref ItemStack from, ref ItemStack to, int amount) {
                if(from.item != to.item) return 0;

                int accept = Mathf.Min(to.item.maxStack - to.amount, amount, from.amount);
                to.amount += accept;
                from.amount -= accept;

                return accept;
            }
        }

        public class ItemInventory {
            public Dictionary<int, ItemStack> contents = new Dictionary<int, ItemStack>();

            private static Dictionary<int, ItemStack> temp = new Dictionary<int, ItemStack>();

            public float TotalMass {
                get {
                    float result = 0f;
                    foreach(var stack in contents.Values) {
                        if(stack.amount <= 0) continue;
                        result += stack.item.mass * stack.amount;
                    }

                    return result;
                }
            }

            public void Add(ItemStack other, int? slot) {
                if(other.amount <= 0) return;

                if(slot == null) {
                    temp.Clear();

                    foreach(var key in contents.Keys) {
                        if(other.amount <= 0) break;

                        var stack = contents[key];
                        ItemStack.Transfer(ref other, ref stack, other.amount);

                        temp.Add(key, stack);
                    }

                    foreach(var key in temp.Keys) {
                        contents[key] = temp[key];
                    }

                    if(other.amount > 0) {
                        int i = -1;
                        while(contents.ContainsKey(++i)) ;

                        contents.Add(i, other);
                    }
                } else {
                    if(contents.ContainsKey((int)slot)) {
                        var existing = contents[(int)slot];
                        ItemStack.Transfer(ref other, ref existing, other.amount);
                    } else {
                        contents.Add((int)slot, other);
                    }
                }
            }
        }
    }
}
