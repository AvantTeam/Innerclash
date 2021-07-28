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
        /// <summary> How long can an item entity would persist in the world context </summary>
        public float lifeTime = 60f;

        public ItemTrait Create(Vector2 pos, int amount) {
            if(amount <= 0) return null;

            var obj = Instantiate(GameController.Instance.itemEntity, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
            var trait = obj.GetComponent<ItemTrait>();
            trait.stack = new ItemStack(this, amount);
            trait.lifeTime = lifeTime;

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
            public bool Empty { get => amount <= 0; }

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
            public readonly Dictionary<int, ItemStack> contents = new Dictionary<int, ItemStack>();
            public int offset;

            private static readonly Dictionary<int, ItemStack> temp = new Dictionary<int, ItemStack>();

            public int Highest {
                get {
                    int highest = offset - 1;
                    foreach(var key in contents.Keys) {
                        if(highest < key) highest = key;
                    }
                    return highest;
                }
            }

            public float TotalMass {
                get {
                    float result = 0f;
                    foreach(var stack in contents.Values) {
                        if(stack.Empty) continue;
                        result += stack.item.mass * stack.amount;
                    }

                    return result;
                }
            }

            public void Add(ItemStack other, int? slot) {
                if(other.amount <= 0) return;

                if(slot == null) {
                    temp.Clear();

                    int highest = Highest;
                    for(int i = offset; i <= highest; i++) {
                        if(other.amount <= 0) break;
                        if(!contents.ContainsKey(i)) continue;

                        var stack = contents[i];
                        ItemStack.Transfer(ref other, ref stack, other.amount);

                        temp.Add(i, stack);
                    }

                    foreach(var key in temp.Keys) {
                        contents[key] = temp[key];
                    }

                    if(other.amount > 0) {
                        int i = offset - 1;
                        while(contents.ContainsKey(++i)) ;

                        contents.Add(i, other);
                    }
                } else {
                    int key = (int)slot;
                    if(contents.ContainsKey(key)) {
                        var stack = contents[key];
                        ItemStack.Transfer(ref other, ref stack, other.amount);

                        contents[key] = stack;
                    } else {
                        contents.Add(key, other);
                    }
                }
            }
        }
    }
}
