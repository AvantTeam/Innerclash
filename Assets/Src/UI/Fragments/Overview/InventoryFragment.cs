using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Innerclash.Entities;

namespace Innerclash.UI.Fragments.Overview {
    public class InventoryFragment : MonoBehaviour {
        [Header("Slot")]
        public GameObject slotPrefab;
        public int row = 8;
        public float pad = 15f / 8f;
        public float margin = 15f;

        public InventoryTrait trait;

        readonly List<GameObject> slots = new List<GameObject>();

        public RectTransform SlotParent { get; private set; }

        void Start() {
            SlotParent = gameObject.GetComponent<ScrollRect>().content;
        }

        void Update() {
            if(trait.NeedsUpdate) {
                int highest = -1;
                foreach(var key in trait.Inventory.contents.Keys) {
                    if(highest < key) highest = key;
                }

                for(int i = 0; i < highest + 2; i++) {
                    while(slots.Count <= i) {
                        slots.Add(NewSlot(i));
                    }

                    var slot = slots[i];
                    var image = slot.GetComponentInChildren<SlotImage>(true);
                    var text = slot.GetComponentInChildren<Text>(true);

                    if(trait.Inventory.contents.ContainsKey(i)) {
                        var stack = trait.Inventory.contents[i];

                        image.enabled = true;
                        image.sprite = stack.item.sprite;

                        text.enabled = true;
                        text.text = stack.amount.ToString();
                    } else {
                        image.enabled = false;
                        text.enabled = false;
                    }
                }

                slots[slots.Count - 1].GetComponent<Button>().enabled = trait.Inventory.TotalMass < trait.maxMass;

                trait.NeedsUpdate = false;
            }
        }

        GameObject NewSlot(int idx) {
            var slot = Instantiate(slotPrefab, SlotParent);
            var trns = (RectTransform)slot.transform;

            float x = idx % row;
            float y = idx / row;
            float width = ((SlotParent.rect.width - 2f * margin) / row) - pad;

            trns.localPosition = new Vector3(
                margin + x * width + x * row / (row - 1f) * pad,
                -(margin + y * width + y * row / (row - 1f) * pad)
            );
            trns.rect.Set(0f, -width, width, width);

            return slot;
        }
    }
}
