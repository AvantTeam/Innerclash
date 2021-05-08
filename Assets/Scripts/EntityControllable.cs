using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityControllable : MonoBehaviour {
    [SerializeField] private float speed = 16f;

    private Rigidbody2D body;

    private void Start() {
        body = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Move(float x) {
        body.velocity += new Vector2(x * speed * Time.deltaTime, 0f);
    }
}
