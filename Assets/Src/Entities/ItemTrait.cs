using UnityEngine;

using static Innerclash.Misc.Item;

namespace Innerclash.Entities {
    /// <summary> Every item drop that exists in the world context </summary>
    [RequireComponent(typeof(PhysicsTrait), typeof(SpriteRenderer))]
    public class ItemTrait : MonoBehaviour {
        public ItemStack stack;

        public PhysicsTrait Physics { get; private set; }
        public SpriteRenderer Render { get; private set; }

        void Start() {
            Physics = GetComponent<PhysicsTrait>();
            Render = GetComponent<SpriteRenderer>();
        }

        void Update() {
            if(stack.item != null) {
                Render.sprite = stack.item.sprite;
            }

            if(stack.amount <= 0) {
                Destroy(gameObject);
            }
        }

        void OnCollisionStay2D(Collision2D collision) {
            ItemTrait trait;
            if(collision.gameObject.TryGetComponent(out trait) && CanJoin(trait)) {
                int accept = Mathf.Min(trait.stack.item.maxStack - trait.stack.amount, stack.amount);
                if(accept > 0) {
                    trait.stack.amount += accept;
                    stack.amount -= accept;
                }
            }
        }

        public bool CanJoin(ItemTrait other) =>
            other.stack.item == stack.item &&
            other.stack.amount >= stack.amount &&
            other.stack.amount < other.stack.item.maxStack;
    }
}
