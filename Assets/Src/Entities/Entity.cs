using UnityEngine;
using Innerclash.Core;
using Innerclash.World;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Entity : MonoBehaviour {
        public Rigidbody2D Body { get; private set; }
        // The collider *must* be a CapsuleCollider2D
        public CapsuleCollider2D Collider { get; private set; }

        public float speed;

        // The amount of ground tiles this entity is touching
        int hitCount;
        // Temporary array for raycasting. This array is initialized at #Start()
        RaycastHit2D[] hits;

        void Start() {
            Body = GetComponent<Rigidbody2D>();
            Collider = GetComponent<CapsuleCollider2D>();

            hits = new RaycastHit2D[Mathf.CeilToInt(Collider.size.x / Context.Instance.tilemap.cellSize.x)];
        }

        void Update() {
            if(hits != null) {
                float size = Collider.size.x;
                Vector2 origin = (Vector2)transform.position - new Vector2(size / 2f, 0.001f);

                hitCount = Physics2D.RaycastNonAlloc(origin, Vector2.right, hits, size);

                var tilemap = Context.Instance.tilemap;
                for(int i = 0; i < hitCount; i++) {
                    var pos = tilemap.WorldToCell(hits[i].point);
                    var tile = tilemap.GetTile<ScriptedTile>(pos);

                    if(tile != null) tile.Apply(this, pos);
                }
            }
        }

        public bool IsGrounded() => hitCount > 0;

        public void Move(Vector2 axis) {
            Body.AddForce(new Vector2(axis.x, 0f) * speed * Time.fixedDeltaTime, ForceMode2D.Force);
        }
    }
}
