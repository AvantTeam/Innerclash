using UnityEngine;
using UnityEngine.Tilemaps;
using Innerclash.Core;
using Innerclash.World;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Entity : MonoBehaviour {
        public Rigidbody2D Body { get; private set; }
        // The collider *must* be a CapsuleCollider2D
        public CapsuleCollider2D Collider { get; private set; }

        // The amount of ground tiles this entity is touching
        private int hitCount;
        // Temporary array for raycasting. This array is initialized at #Start()
        private RaycastHit2D[] hits;

        private void Start() {
            Body = GetComponent<Rigidbody2D>();
            Collider = GetComponent<CapsuleCollider2D>();

            Vector3 tilesize = Context.Instance.tilemap.cellSize;
            hits = new RaycastHit2D[Mathf.CeilToInt(Collider.size.x / tilesize.x)];
        }

        private void Update() {
            float size = Collider.size.x;
            Vector2 origin = (Vector2)transform.position - new Vector2(size / 2f, 0.001f);

            hitCount = Physics2D.RaycastNonAlloc(origin, Vector2.right, hits, size);

            Tilemap tilemap = Context.Instance.tilemap;
            for(int i = 0; i < hitCount; i++) {
                var pos = tilemap.WorldToCell(hits[i].point);
                var tile = tilemap.GetTile<ScriptedTile>(pos);

                tile.Apply(this, pos);
            }
        }

        public bool IsGrounded() => hitCount > 0;
    }
}
