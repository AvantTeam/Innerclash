using UnityEngine;

using static Innerclash.Misc.Item;

namespace Innerclash.Entities {
    /// <summary> Every item drop that exists in the world context </summary>
    [RequireComponent(typeof(PhysicsTrait), typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class ItemTrait : MonoBehaviour {
        public ItemStack stack;

        private static readonly Collider2D[] collides = new Collider2D[8];

        public PhysicsTrait Physics { get; private set; }
        public CircleCollider2D Collider { get; private set; }
        public SpriteRenderer Render { get; private set; }

        void Start() {
            Physics = GetComponent<PhysicsTrait>();
            Collider = GetComponent<CircleCollider2D>();
            Render = GetComponent<SpriteRenderer>();
        }

        void Update() {
            int found = Physics2D.OverlapCircleNonAlloc((Vector2)transform.position + Collider.offset, Collider.radius, collides, LayerMask.GetMask("Entity"));
            for(int i = 0; i < found; i++) {
                if(stack.amount <= 0) break;

                if(collides[i].TryGetComponent(out InventoryTrait inv)) {
                    int accept = inv.Add(stack);
                    stack.amount -= accept;
                }
            }

            if(stack.item != null) {
                Render.sprite = stack.item.sprite;
            }

            if(stack.amount <= 0) {
                Destroy(gameObject);
            }
        }

        void OnCollisionStay2D(Collision2D collision) {
            if(stack.item == null || stack.amount <= 0) return;

            if(collision.gameObject.TryGetComponent(out ItemTrait otherItem) && CanJoin(otherItem)) {
                ItemStack.Transfer(ref stack, ref otherItem.stack, stack.amount);
            }
        }

        public bool CanJoin(ItemTrait other) =>
            other.stack.item == stack.item &&
            other.stack.amount >= stack.amount &&
            other.stack.amount < other.stack.item.maxStack;
    }
}
