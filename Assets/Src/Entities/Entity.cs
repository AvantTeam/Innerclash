using UnityEngine;
using Innerclash.Core;
using Innerclash.Utils;

namespace Innerclash.Entities {
    [RequireComponent(typeof(Rigidbody2D))]
    public class Entity : MonoBehaviour {
        public Rigidbody2D Body { get; private set; }
        public Vector2 MoveAxis { get; private set; }

        [Header("Material")]
        public GroundCheck ground;
        /// <summary> Force used to move </summary>
        public float moveForce = 800f;

        /// <summary> The amount of ground tiles this entity is touching </summary>
        int hitCount;
        /// <summary> Temporary array for raycasting. This array is initialized at #Start() </summary>
        RaycastHit2D[] hits;

        void Start() {
            Body = GetComponent<Rigidbody2D>();
            MoveAxis = new Vector2();

            hits = new RaycastHit2D[Mathf.FloorToInt(ground.width / Context.Instance.tilemap.cellSize.x) + 1];
        }

        void FixedUpdate() {
            if(hits != null) {
                hitCount = Physics2D.RaycastNonAlloc(
                    (Vector2)transform.position + new Vector2(ground.offsetX - ground.width / 2f, ground.offsetY - 0.1f),
                    Vector2.right,
                    hits,
                    ground.width
                );

                for(int i = 0; i < hitCount; i++) {
                    Tilemaps.ApplyTile(hits[i].point, this);
                }
            }

            Body.AddForce(new Vector2(MoveAxis.x, 0f) * moveForce * Time.fixedDeltaTime, ForceMode2D.Force);
        }

        public bool IsGrounded() => hitCount > 0;

        public bool IsMoving() => MoveAxis.magnitude > 0.1f;

        public void Move(Vector2 axis) => MoveAxis = axis;

        void OnDrawGizmos() {
            Gizmos.DrawLine(transform.position + (Vector3)ground.LeftLimit, transform.position + (Vector3)ground.RightLimit);

            for(int i = 0; i < hitCount; i++) {
                Tilemaps.WithTile(hits[i].point, (tile, pos) => Gizmos.DrawWireCube(
                    new Vector3(pos.x + 0.5f, pos.y + 0.5f, pos.y + 0.5f),
                    new Vector3(1f, 1f, 1f)
                ));
            }
        }

        [System.Serializable]
        public struct GroundCheck {
            public float offsetX;
            public float offsetY;
            public float width;

            public Vector2 LeftLimit { get => new Vector2(offsetX - width / 2f, offsetY); }
            public Vector2 RightLimit { get => new Vector2(offsetX + width / 2f, offsetY); }
        }
    }
}
