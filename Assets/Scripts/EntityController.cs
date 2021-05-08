using UnityEngine;

[RequireComponent(typeof(EntityControllable))]
public class EntityController : MonoBehaviour {
    public EntityControllable controllable { get; protected set; }

    private void Start() {
        controllable = gameObject.GetComponent<EntityControllable>();
    }
}
