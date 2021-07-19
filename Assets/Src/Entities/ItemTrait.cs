using UnityEngine;

using static Innerclash.Misc.Item;

namespace Innerclash.Entities {
    /// <summary> Every item drop that exists in the world </summary>
    [RequireComponent(typeof(PhysicsTrait))]
    public class ItemTrait : MonoBehaviour {
        public ItemStack stack;
    }
}
