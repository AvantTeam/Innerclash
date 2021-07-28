﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Innerclash.Core;
using Innerclash.Entities;
using Innerclash.Utils;

using static Innerclash.Misc.Item;

namespace Innerclash.UI.Fragments.Overview {
    public class InventoryFragment : MonoBehaviour, ISlotButtonHandler {
        public GameObject slotPrefab;

        [Header("Attributes")]
        public InventoryTrait trait;
        public ScrollRect scroll;
        public Slider massBar;
        public Image massImage;

        readonly List<GameObject> slots = new List<GameObject>();
        readonly List<int> strips = new List<int>();
        bool needsStripping = false;

        public RectTransform SlotParent { get; private set; }

        void Start() {
            SlotParent = scroll.content;
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
                        var n = Instantiate(slotPrefab, SlotParent);
                        n.GetComponent<SlotButton>().handler = this;

                        slots.Add(n);
                    }

                    var slot = slots[i].GetComponent<SlotButton>();
                    var image = slot.icon;
                    var text = slot.text;

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

                massBar.value = Mathf.Min(trait.Inventory.TotalMass / trait.maxMass, 1f);
                massImage.color = Color.Lerp(Color.green, Color.red, massBar.value);

                trait.NeedsUpdate = false;
            }
        }

        void Interact(int idx) {
            if(idx < 0 || idx >= slots.Count) return;

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

        void ISlotButtonHandler.Handle(SlotButton button) {
            Interact(Structs.IndexOf(slots, button.gameObject));
        }

        public void Trash() {
            GameController.Instance.CurrentStack = new ItemStack();

            trait.NeedsUpdate = true;
            needsStripping = true;
        }

        public void Drop() {
            if(Drop(GameController.Instance.CurrentStack)) {
                GameController.Instance.CurrentStack = new ItemStack();

                trait.NeedsUpdate = true;
                needsStripping = true;
            }
        }

        public bool Drop(ItemStack stack) {
            if(stack.Empty) return false;

            var item = stack.item.Create(trait.transform.position, stack.amount);
            return item.FromEntity = true;
        }

        public void OnVisibilityChange(bool visible) {
            var inst = GameController.Instance;
            if(!visible && inst.HoldingStack) {
                var source = inst.CurrentStack;

                int accepted = trait.Add(source);
                source.amount -= accepted;

                Drop(source);
                inst.CurrentStack = new ItemStack();

                trait.NeedsUpdate = true;
                needsStripping = true;
            }
        }
    }
}
