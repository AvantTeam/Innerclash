using System;
using UnityEngine;

namespace Innerclash.Entities {
    public class ControllableTrait : MonoBehaviour {
        public event Action<Vector2> Move;
        public event Action<bool> Jump;

        public void OnMove(Vector2 axis) => Move?.Invoke(axis);

        public void OnJump(bool jump) => Jump?.Invoke(jump);
    }
}
