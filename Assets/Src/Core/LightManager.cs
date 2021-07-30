using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Innerclash.Utils;

namespace Innerclash.Core {
    [RequireComponent(typeof(SpriteRenderer))]
    public class LightManager : MonoBehaviour {
        public int extension;
        public int padding;

        public SpriteRenderer Renderer { get; private set; }
        public Camera Cam { get => GameController.Instance.mainCamera; }

        Texture2D texture;

        Color[] colors;
        readonly List<Vector2Int> emits = new List<Vector2Int>();

        static readonly int radius = 8;
        static readonly float normalDropoff = 0.7f;
        static readonly float diagonalDropoff = Mathf.Pow(normalDropoff, Mathf.Sqrt(2f));
        static readonly float threshold = 0.04f;
        static readonly int diameter = radius * 2 + 1;

        readonly Color[] singleEmission = new Color[diameter * diameter];
        readonly List<Vector2Int> fillQueue = new List<Vector2Int>();

        volatile bool
            shouldStop = false,
            shouldProcess = false,
            isProcessing = false;

        volatile Func<Action> resultAsync = null;
        volatile Action resultSync = null;

        void Start() {
            Renderer = GetComponent<SpriteRenderer>();

            RecalculatePosition();

            var t = new Thread(() => {
                while(!shouldStop) {
                    if(shouldProcess) {
                        isProcessing = true;
                        shouldProcess = false;

                        var result = resultAsync();
                        resultSync = result;

                        isProcessing = false;
                    }
                }
            });
            t.Start();
        }

        void OnDestroy() {
            shouldStop = true;
        }

        void Update() {
            RecalculatePosition();

            if(!isProcessing && resultAsync == null && resultSync == null) {
                resultAsync = RecalculateColors();
                shouldProcess = true;
            }

            if(resultSync != null) {
                var act = resultSync;
                resultSync = null;
                resultAsync = null;

                act();
            }
        }

        void RecalculatePosition() {
            var size = PreferredSize();
            int width = size.x,
                height = size.y;

            if(texture == null || (texture.width != width || texture.height != height)) {
                texture = new Texture2D(width, height);
                colors = new Color[width * height];

                Renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f, 1);
            }

            Vector2 previous = transform.position;
            transform.position = new Vector3(
                Mathf.RoundToInt(Cam.transform.position.x / extension) * extension,
                Mathf.RoundToInt(Cam.transform.position.y / extension) * extension,
                0f
            );

            if(previous != (Vector2)transform.position) {
                RecalculateColors()()();
            }
        }

        Vector2Int PreferredSize() {
            int height = (int)(Cam.orthographicSize * 2);
            int width = height * Screen.width / Screen.height;
            if(height % 2 != 0) height += 1;
            if(width % 2 != 0) width += 1;

            height += extension * 2;
            width += extension * 2;

            return new Vector2Int(width, height);
        }

        /// <summary> Call this synchronously, then call the returned delegate asynchronously, then call the other returned delegate synchronously. </summary>
        Func<Action> RecalculateColors() {
            if(texture == null) return () => () => {};

            Vector2 pos = transform.position;
            int width = texture.width,
                height = texture.height;

            lock(emits) {
                emits.Clear();
                for(int y = 0; y < height; y++) {
                    for(int x = 0; x < width; x++) {
                        int idx = x + y * width;

                        var tile = Tilemaps.GetTile(pos + new Vector2(x - width / 2, y - height / 2));
                        if(tile == null || tile.emitsLight) {
                            var color = tile == null ? Color.white : tile.emitLight;
                            colors[idx] = color;

                            emits.Add(new Vector2Int(x, y));
                        } else {
                            colors[idx] = Color.black;
                        }
                    }
                }
            }

            return () => {
                lock(emits) {
                    foreach(var emit in emits) {
                        Emit(emit.x, emit.y, width);
                    }

                    return () => {
                        texture.SetPixels(colors);
                        texture.Apply();
                    };
                }
            };
        }

        void Emit(int rootX, int rootY, int width) {
            for(int i = 0; i < singleEmission.Length; i++) {
                singleEmission[i] = new Color();
            }

            fillQueue.Clear();

            singleEmission[radius + radius * diameter] = colors[rootX + rootY * width];
            fillQueue.Add(new Vector2Int(rootX, rootY));

            while(fillQueue.Count > 0) {
                var currentTile = fillQueue[0];
                fillQueue.RemoveAt(0);

                int x = currentTile.x,
                    y = currentTile.y,
                    index = x + y * width,
                    currentLayer = Mathf.Max(Mathf.Abs(x - rootX), Mathf.Abs(y - rootY));

                bool pass = false;
                var currentColor = colors[index];
                var targetColor = singleEmission[(radius + x - rootX) + (radius + y - rootY) * diameter];

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

                for(int ny = y - 1; ny <= y + 1; ny++) {
                    for(int nx = x - 1; nx <= x + 1; nx++) {
                        int neighbor = Mathf.Max(Mathf.Abs(nx - rootX), Mathf.Abs(ny - rootY));
                        if(neighbor <= radius && neighbor == currentLayer + 1) {
                            float dropoff = (nx != x && ny != y) ? diagonalDropoff : normalDropoff;
                            int eidx = (radius + nx - rootX) + (radius + ny - rootY) * diameter;

                            var emit = singleEmission[eidx];
                            if(Structs.InBounds(colors, nx + ny * width) && emit.r + emit.g + emit.b == 0f) {
                                fillQueue.Add(new Vector2Int(nx, ny));
                            }

                            singleEmission[eidx].r = Mathf.Max(targetColor.r * dropoff, singleEmission[eidx].r);
                            singleEmission[eidx].g = Mathf.Max(targetColor.g * dropoff, singleEmission[eidx].g);
                            singleEmission[eidx].b = Mathf.Max(targetColor.b * dropoff, singleEmission[eidx].b);
                        }
                    }
                }
            }
        }
    }
}
