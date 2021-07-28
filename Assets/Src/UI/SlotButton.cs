using UnityEngine;
using UnityEngine.UI;

using static Innerclash.Misc.Item;

namespace Innerclash.UI {
    public class SlotButton : MonoBehaviour {
        public Image icon;
        public Text text;
        public ISlotButtonHandler handler;

        public void OnClick() {
            handler.Handle(this);
        }

        public void Set(ItemStack stack) {
            if(stack.Empty) {
                ResetIcon();
                return;
            }

            icon.enabled = true;
            icon.sprite = stack.item.sprite;

            text.enabled = true;
            text.text = stack.amount.ToString();
        }
        
        public void ResetIcon() {
            icon.enabled = false;
            text.enabled = false;
        }
    }
}
