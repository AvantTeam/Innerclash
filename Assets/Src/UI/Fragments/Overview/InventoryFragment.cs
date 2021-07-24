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

        [Header("Attributes")]
        public InventoryTrait trait;
        public Image massBar;
        public Image massProgression;

        readonly List<GameObject> slots = new List<GameObject>();
        readonly List<int> strips = new List<int>();
        bool needsStripping = false;

        public RectTransform SlotParent { get; private set; }

        void Start() {
            SlotParent = GetComponent<ScrollRect>().content;
        }

        void Update() {
            if(trait.NeedsUpdate) {
                int highest = -1;
                foreach(var key in trait.Inventory.contents.Keys) {
                    if(highest < key) highest = key;
                }

                highest = Mathf.Max(highest, slots.Count - 2);
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

                if(needsStripping) {
                    strips.Clear();

                    var content = trait.Inventory.contents;
                    for(int i = slots.Count - 2; i > 0; i--) {
                        if(!content.ContainsKey(i) && !content.ContainsKey(i + 1)) {
                            strips.Add(i + 1);
                        } else if(content.ContainsKey(i)) {
                            break;
                        }
                    }

                    foreach(int strip in strips) {
                        var slot = slots[strip];
                        slots.RemoveAt(strip);

                        Destroy(slot);
                    }

                    needsStripping = false;
                }

                float width = ((RectTransform)massBar.transform).sizeDelta.x;
                float prog = Mathf.Min(trait.Inventory.TotalMass / trait.maxMass, 1f);
                float min = massProgression.sprite.rect.width * 2f / 3f;

                ((RectTransform)massProgression.transform).SetSizeWithCurrentAnchors(
                    Axis.Horizontal,
                    min + prog * (width - min)
                );
                massProgression.color = Color.Lerp(Color.green, Color.red, prog);

                trait.NeedsUpdate = false;
            }
        }

        public void Interact(int idx) {
            if(idx >= slots.Count) return;

            var inst = GameController.Instance;
            var content = trait.Inventory.contents;

            if(inst.HoldingStack) { // If is already holding a stack, either put or swap
                var source = inst.CurrentStack;
                if(content.ContainsKey(idx)) {
                    var stack = content[idx];
                    if(stack.item == source.item && !stack.Full) { // If the item is the same, transfer it
                        ItemStack.Transfer(ref source, ref stack, source.amount);

                        content[idx] = stack;
                        inst.CurrentStack = source;

                        if(source.Empty) needsStripping = true;
                    } else { // Otherwise, simply swap
                        content[idx] = source;
                        inst.CurrentStack = stack;
                    }
                } else {
                    int accepted = trait.Add(source, idx);
                    source.amount -= accepted;

                    inst.CurrentStack = source;

                    needsStripping = true;
                }

                trait.NeedsUpdate = true;
            } else if(content.ContainsKey(idx)) { // Otherwise try to take an existing stack from inventory
                inst.CurrentStack = content[idx];
                content.Remove(idx);

                trait.NeedsUpdate = true;
            }
        }

        public void OnVisibilityChange(bool visible) {
            var inst = GameController.Instance;
            if(!visible && inst.HoldingStack) {
                var source = inst.CurrentStack;

                int accepted = trait.Add(source);
                source.amount -= accepted;

                source.item.Create(trait.transform.position, source.amount);
                inst.CurrentStack = new ItemStack();

                trait.NeedsUpdate = true;
                needsStripping = true;
            }
        }

        GameObject NewSlot(int idx) {
            var slot = Instantiate(slotPrefab, SlotParent);
            var trns = (RectTransform)slot.transform;

            float x = idx % row;
            float y = idx / row;
            float width = (((RectTransform)SlotParent.parent.parent).sizeDelta.x - 2f * margin) / row - pad;

            trns.localPosition = new Vector3(
                margin + x * width + x * row / (row - 1f) * pad,
                -(margin + y * width + y * row / (row - 1f) * pad)
            );
            trns.SetSizeWithCurrentAnchors(Axis.Horizontal, width);
            trns.SetSizeWithCurrentAnchors(Axis.Vertical, width);

            var b = slot.GetComponent<SlotButton>();
            b.Index = idx;

            return slot;
        }
    }
}
