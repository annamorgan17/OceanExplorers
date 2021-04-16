using UnityEngine;

namespace Phi.ButtonExstensions {
    public static class Button {
        public static bool isTriggerButton(this Collider col) {
            return col.tag == "ButtonActivator";
        }
    }
}
