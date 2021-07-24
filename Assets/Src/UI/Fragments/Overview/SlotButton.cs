using UnityEngine.UI;
using Innerclash.Core;

namespace Innerclash.UI.Fragments.Overview {
    public class SlotButton : Button {
        public int Index { get; set; }

        public void OnClick() {
            GameController.Instance.inventoryFragment.Interact(Index);
        }
    }
}
