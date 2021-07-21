using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Innerclash.Entities;

namespace Innerclash.UI.Fragments.Overview {
    public class InventoryFragment : MonoBehaviour {
        public GameObject slotPrefab;
        public InventoryTrait trait;

        List<GameObject> slots = new List<GameObject>();

        public GameObject SlotParent { get; private set; }

        void Start() {
            SlotParent = gameObject.GetComponent<ScrollRect>().content.gameObject;
        }

        void Update() {
            int highest = 1;
            foreach(var key in trait.Inventory.contents.Keys) {
                if(highest > key) highest = key;
            }

            for(int i = 0; i < highest; i++) {
                if(slots.Count <= i + 1) {
                    while(slots.Count <= i + 1) {
                        slots.Add(NewSlot(i));
                    }
                }
            }

            foreach(var key in trait.Inventory.contents.Keys) {
                var slot = slots[key];
                var stack = trait.Inventory.contents[key];

                var image = slot.GetComponentInChildren<Image>();
                image.enabled = true;

                var text = image.GetComponentInChildren<Text>();
                text.text = stack.amount.ToString();
            }
        }

        GameObject NewSlot(int idx) {
            var slot = Instantiate(slotPrefab, SlotParent.transform);
            slot.transform.localPosition = new Vector3(7f + (idx % 8) * 39.5f, 7f + (idx / 8) * 39.5f, 0f);

            return slot;
        }
    }
}
