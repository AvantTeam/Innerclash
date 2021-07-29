using UnityEngine;
using Innerclash.Utils;

namespace Innerclash.Core {
    [RequireComponent(typeof(SpriteRenderer))]
    public class LightManager : MonoBehaviour {
        public Material shaderMaterial;

        public int pixelsPerUnit;
        public int extension;
        public int padding;

        public SpriteRenderer Renderer { get; private set; }
        public Camera Cam { get => GameController.Instance.mainCamera; }

        Texture2D texture;
        Color[] colors;

        void Start() {
            Renderer = GetComponent<SpriteRenderer>();
            RecalculatePosition();
        }

        void FixedUpdate() {
            if((Cam.transform.position - Renderer.transform.position).magnitude >= extension - padding) {
                RecalculatePosition();
            }

            RecalculateColors();
        }

        void RecalculatePosition() {
            Renderer.transform.position = new Vector2(
                Mathf.RoundToInt(Cam.transform.position.x / extension) * extension + 0.5f,
                Mathf.RoundToInt(Cam.transform.position.y / extension) * extension
            );

            int height = (int)(Cam.orthographicSize * 2 * pixelsPerUnit);
            int width = height * Screen.width / Screen.height;
            height += extension * pixelsPerUnit * 2;
            width += extension * pixelsPerUnit * 2;

            if(texture == null || (texture.width != width || texture.height != height)) {
                texture = new Texture2D(width, height);
                colors = new Color[width * height];

                Renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f, pixelsPerUnit);
            }
        }

        void RecalculateColors() {
            Vector2 pos = Renderer.transform.position;
            int width = texture.width,
                height = texture.height;

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    int idx = x + y * width;

                    if(Tilemaps.GetTile(pos + new Vector2(x - width / 2, y - height / 2)) != null) {
                        colors[idx] = new Color(0f, 0f, 0f, 1f);
                    } else {
                        colors[idx] = new Color(1f, 1f, 1f, 1f);
                    }
                }
            }
            
            texture.SetPixels(colors);
            texture.Apply();
        }
    }
}
