using UnityEngine;
using Innerclash.Entities;

namespace Innerclash.UI.Fragments {
    public class HotbarFragment : MonoBehaviour, ISlotButtonHandler {
        public InventoryTrait trait;
        public HandSlot[] slots;

        float lastUpdated;

        void Start() {
            foreach(var slot in slots) {
                slot.left.handler = this;
                slot.right.handler = this;
            }
        }

        void Update() {
            if(lastUpdated < trait.LastUpdated) {
                lastUpdated = trait.LastUpdated;

                var content = trait.Inventory.contents;
                for(int i = 0; i < slots.Length; i++) {
                    var slot = slots[i];

                    if(content.ContainsKey(i * 2)) {
                        slot.left.Set(content[i * 2]);
                    } else {
                        slot.left.ResetIcon();
                    }

                    if(content.ContainsKey(i * 2 + 1)) {
                        slot.right.Set(content[i * 2 + 1]);
                    } else {
                        slot.right.ResetIcon();
                    }
                }
            }
        }

        public void Handle(SlotButton button) {

        }
    }
}
