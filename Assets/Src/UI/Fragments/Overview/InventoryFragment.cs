using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Innerclash.Entities;

namespace Innerclash.UI.Fragments.Overview {
    public class InventoryFragment : MonoBehaviour {
        [Header("Slot")]
        public GameObject slotPrefab;

        public InventoryTrait trait;

        List<GameObject> slots = new List<GameObject>();

        public GameObject SlotParent { get; private set; }

        void Start() {
            SlotParent = gameObject.GetComponent<ScrollRect>().content.gameObject;
        }

        void Update() {
            if(trait.NeedsUpdate) {
                int highest = 0;
                foreach(var key in trait.Inventory.contents.Keys) {
                    if(highest < key) highest = key;
                }

                for(int i = 0; i < highest + 1; i++) {
                    if(slots.Count <= i + 1) {
                        while(slots.Count <= i + 1) {
                            slots.Add(NewSlot(i));
                        }
                    }

                    var slot = slots[i];
                    var image = slot.GetComponentInChildren<Image>();

                    if(trait.Inventory.contents.ContainsKey(i)) {
                        image.enabled = true;
                        image.sprite = trait.Inventory.contents[i].item.sprite;
                    } else {
                        image.enabled = false;
                    }
                }

                trait.NeedsUpdate = false;
            }
        }

        GameObject NewSlot(int idx) {
            var slot = Instantiate(slotPrefab, SlotParent.transform);
            var trns = (RectTransform)slot.transform;
            trns.localPosition = new Vector3(
                14f + idx % 8 * 39.5f,
                -(14f + idx / 8 * 39.5f),
                0f
            );

            trns.ForceUpdateRectTransforms();
            return slot;
        }
    }
}
