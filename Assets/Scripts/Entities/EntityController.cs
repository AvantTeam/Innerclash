using UnityEngine;

namespace Innerclash.Entities {
    [RequireComponent(typeof(EntityControllable))]
    public class EntityController : MonoBehaviour {
        public EntityControllable Controllable { get; protected set; }

        private void Start() {
            Controllable = gameObject.GetComponent<EntityControllable>();
        }
    }
}
