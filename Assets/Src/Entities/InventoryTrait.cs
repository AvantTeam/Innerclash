using UnityEngine;

using static Innerclash.Misc.Item;

namespace Innerclash.Entities {
    /// <summary> An entity trait that holds an inventory </summary>
    public class InventoryTrait : MonoBehaviour {
        public float maxMass = 1000f;

        public ItemInventory Inventory { get; private set; }

        void Start() {
            Inventory = new ItemInventory();
        }

        public int Accept(ItemStack stack) {
            return Mathf.FloorToInt((maxMass - Inventory.TotalMass) / stack.item.mass);
        }

        public int Add(ItemStack stack) {
            int res = Accept(stack);
            Inventory.Add(new ItemStack(stack.item, res), null);

            return res;
        }
    }
}
