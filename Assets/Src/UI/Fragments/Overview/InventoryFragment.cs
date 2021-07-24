using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Innerclash.Core;
using Innerclash.Entities;

using static UnityEngine.RectTransform;
using static Innerclash.Misc.Item;

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

                trait.NeedsUpdate = false;
            }
        }

        void Interact(int idx) {
            if(idx >= slots.Count) return;
            Debug.Log("Interacting with " + idx);

            trait.NeedsUpdate = true;

            var inst = GameController.Instance;
            var content = trait.Inventory.contents;

            if(inst.HoldingStack) { // If is already holding a stack, either put or swap
                var source = inst.CurrentStack;
                if(content.ContainsKey(idx)) {
                    var stack = trait.Inventory.contents[idx];
                    if(stack.item == source.item) { // If the item is the same, transfer it
                        ItemStack.Transfer(ref source, ref stack, stack.amount);

                        content[idx] = stack;
                        inst.CurrentStack = source;
                    } else { // Otherwise, simply swap
                        content[idx] = source;
                        inst.CurrentStack = stack;
                    }
                } else {
                    int accepted = trait.Add(source, idx);
                    source.amount -= accepted;
                }
            } else if(content.ContainsKey(idx)) { // Otherwise try to take an existing stack from inventory
                inst.CurrentStack = content[idx];
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
            trns.SetSizeWithCurrentAnchors(Axis.Horizontal, width);
            trns.SetSizeWithCurrentAnchors(Axis.Vertical, width);

            var b = slot.GetComponent<Button>();
            b.onClick.AddListener(() => Interact(idx));

            return slot;
        }
    }
}
