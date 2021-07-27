using System;
using UnityEngine;

namespace Innerclash.Entities {
    public class ControllableTrait : MonoBehaviour {
        public event Action<Vector2> Move;
        public event Action<bool> Jump;
        public event Action<float> Break;
        public event Action<float> Act;

        public void OnMove(Vector2 axis) => Move?.Invoke(axis);

        public void OnJump(bool jump) => Jump?.Invoke(jump);

        public void OnBreak(float press) => Break?.Invoke(press);

        public void OnAct(float press) => Act?.Invoke(press);
    }
}
