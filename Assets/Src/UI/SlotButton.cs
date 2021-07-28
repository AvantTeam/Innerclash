using UnityEngine;
using UnityEngine.UI;
using Innerclash.Core;

namespace Innerclash.UI {
    public class SlotButton : MonoBehaviour {
        public Image icon;
        public Text text;
        public ISlotButtonHandler handler;

        public void OnClick() {
            handler.Handle(this);
        }
    }
}
