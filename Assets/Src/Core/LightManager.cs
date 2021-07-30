using System.Collections.Generic;
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
        bool[] emits;

        static int radius = 8;
        static float normalDropoff = 0.7f;
        static float diagonalDropoff = Mathf.Pow(normalDropoff, Mathf.Sqrt(2f));
        static float threshold = 0.02f;
        static int diameter = radius * 2 + 1;

        Color[] singleEmission = new Color[diameter * diameter];
        List<Vector2Int> fillQueue = new List<Vector2Int>();

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
                emits = new bool[width * height];

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

                    var tile = Tilemaps.GetTile(pos + new Vector2(x - width / 2, y - height / 2));
                    if(tile == null || tile.emitsLight) {
                        colors[idx] = tile == null ? new Color(1f, 1f, 1f, 1f) : tile.emitLight;
                        emits[idx] = true;
                    } else {
                        colors[idx] = new Color(0f, 0f, 0f, 1f);
                        emits[idx] = false;
                    }
                }
            }

            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    int idx = x + y * width;

                    if(emits[idx]) Emit(idx);
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
        }

        void Emit(int idx) {
            int rootX = idx % texture.width,
                rootY = idx / texture.width;

            for(int i = 0; i < singleEmission.Length; i++) {
                singleEmission[i] = new Color();
            }

            fillQueue.Clear();

            singleEmission[radius + radius * diameter] = colors[idx];
            fillQueue.Add(new Vector2Int(rootX, rootY));

            while(fillQueue.Count > 0) {
                var currentTile = fillQueue[0];
                fillQueue.RemoveAt(0);

                int x = currentTile.x,
                    y = currentTile.y,
                    index = x + y * texture.width,
                    currentLayer = Mathf.Max(Mathf.Abs(x - rootX), Mathf.Abs(y - rootY));

                bool pass = false;
                var currentColor = colors[index];
                var targetColor = singleEmission[radius + x - rootX + (radius * y - rootY) * diameter];

                if(
                    (targetColor.r > threshold || targetColor.g > threshold || targetColor.b > threshold) &&
                    (targetColor.r > currentColor.r || targetColor.g > currentColor.g || targetColor.b > currentColor.b)
                ) {
                    colors[index] = new Color(
                        Mathf.Max(targetColor.r, currentColor.r),
                        Mathf.Max(targetColor.g, currentColor.g),
                        Mathf.Max(targetColor.b, currentColor.b)
                    );
                    pass = true;
                }

                if(!(x == rootX && y == rootY) && !pass) continue;

                for(int nx = x - 1; nx <= x + 1; nx++) {
                    for(int ny = y - 1; ny <= y + 1; ny++) {
                        int neighbor = Mathf.Max(Mathf.Abs(nx - rootX), Mathf.Abs(ny - rootY));
                        if(neighbor <= radius && neighbor == currentLayer + 1) {
                            float dropoff = (nx != x && ny != y) ? diagonalDropoff : normalDropoff;
                            int emitX = radius + nx - rootX,
                                emitY = radius + ny - rootY;

                            var emit = singleEmission[emitX + emitY * diameter];
                            if(emit.r + emit.g + emit.b == 0f) {
                                fillQueue.Add(new Vector2Int(nx, ny));
                            }

                            singleEmission[emitX + emitY * diameter].r = Mathf.Max(targetColor.r * dropoff, singleEmission[emitX + emitY * diameter].r);
                            singleEmission[emitX + emitY * diameter].g = Mathf.Max(targetColor.g * dropoff, singleEmission[emitX + emitY * diameter].g);
                            singleEmission[emitX + emitY * diameter].b = Mathf.Max(targetColor.b * dropoff, singleEmission[emitX + emitY * diameter].b);
                        }
                    }
                }
            }
        }
    }
}
