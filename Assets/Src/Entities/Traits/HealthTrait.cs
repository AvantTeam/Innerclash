using UnityEngine;

namespace Innerclash.Entities.Traits {
    /// <summary> An entity trait that has a health and can be damaged </summary>
    public class HealthTrait : MonoBehaviour {
        public float maxHealth = 100f;

        public float Health { get; private set; }

        void Start() {
            Health = maxHealth;
        }

        void Update() {
            if(Health <= 0f) Destroy(this);
        }
    }
}
