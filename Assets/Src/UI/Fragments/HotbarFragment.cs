using UnityEngine;
using Innerclash.Core;

namespace Innerclash.UI.Fragments {
    public class HotbarFragment : MonoBehaviour, ISlotButtonHandler {
        public HandSlot[] slots;

        void Start() {
            foreach(var slot in slots) {
                slot.left.handler = this;
                slot.right.handler = this;
            }

            GameController.Instance.inventoryFragment.Updated += Updated;
        }

        void OnDestroy() {
            GameController.Instance.inventoryFragment.Updated -= Updated;
        }

        void Updated() {
            var content = GameController.Instance.inventoryFragment.trait.Inventory.contents;
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

        public void Handle(SlotButton button) {

        }
    }
}
