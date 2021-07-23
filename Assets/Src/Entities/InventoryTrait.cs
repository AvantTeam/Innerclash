using UnityEngine;

using static Innerclash.Misc.Item;

namespace Innerclash.Entities {
    /// <summary> An entity trait that holds an inventory </summary>
    public class InventoryTrait : MonoBehaviour {
        public float maxMass = 1000f;

        public ItemInventory Inventory { get; private set; }
        public bool NeedsUpdate { get; set; }

        void Start() {
            Inventory = new ItemInventory();
            NeedsUpdate = true;
        }

        public int Accept(ItemStack stack) {
            return Mathf.Min(Mathf.FloorToInt((maxMass - Inventory.TotalMass) / stack.item.mass), stack.amount);
        }

        public int Add(ItemStack stack) {
            return Add(stack, null);
        }

        public int Add(ItemStack stack, int? idx) {
            NeedsUpdate = true;

            int res = Accept(stack);
            Inventory.Add(new ItemStack(stack.item, res), idx);

            return res;
        }
    }
}
