using System.Collections.Generic;
using UnityEngine;

namespace Innerclash {
    public class Logic : MonoBehaviour {
        public static Logic Instance { get; private set; }

        private void Awake() {
            Instance = this;
        }
    }
}
