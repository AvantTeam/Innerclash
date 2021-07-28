using UnityEngine;

namespace Innerclash.UI.Fragments {
    public class HotbarFragment : MonoBehaviour, ISlotButtonHandler {
        public HandSlot[] slots;

        void Start() {
            foreach(var slot in slots) {
                slot.left.handler = this;
                slot.right.handler = this;
            }
        }

        void ISlotButtonHandler.Handle(SlotButton button) {
            
        }
    }
}
