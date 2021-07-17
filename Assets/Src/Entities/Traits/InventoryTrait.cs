using UnityEngine;

using static Innerclash.Misc.Item;

namespace Innerclash.Entities.Traits {
    /// <summary> An entity trait that holds an inventory </summary>
    public class InventoryTrait : MonoBehaviour {
        public int inventoryWidth;
        public int inventoryHeight;

        public ItemInventory Inventory { get; private set; }

        void Start() {
            Inventory = new ItemInventory(inventoryWidth, inventoryHeight);
        }
    }
}
