using UnityEngine;

using static Innerclash.Misc.Item;

namespace Innerclash.Entities {
    /// <summary> Every item drop that exists in the world context </summary>
    [RequireComponent(typeof(PhysicsTrait), typeof(SpriteRenderer))]
    public class ItemTrait : MonoBehaviour {
        public ItemStack stack;

        public SpriteRenderer Render { get; private set; }

        void Start() {
            Render = GetComponent<SpriteRenderer>();
            Destroy(gameObject, 5f);
        }

        void Update() {
            if(stack.item != null) {
                Render.sprite = stack.item.sprite;
            }
        }
    }
}
