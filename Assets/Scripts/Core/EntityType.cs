using UnityEngine;
using Innerclash.Entities;

namespace Innerclash.Core {
    [CreateAssetMenu]
    public class EntityType : ScriptableObject {
        public float speed = 16f;
        public float accel = 2f;
        public float jumpHeight = 3f;
        [Range(0, 1)] public float midAirAccel = 0.2f;

        public EntityControllable entity;
        public float hitSizeX = 1f;
        public float hitSizeY = 1.9f;

        public EntityControllable create() {
            EntityControllable ent = Instantiate(entity, Vector3.zero, Quaternion.identity);
            ent.Type = this;
            return ent;
        }
    }
}
