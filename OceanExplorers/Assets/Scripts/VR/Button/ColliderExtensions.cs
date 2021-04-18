using UnityEngine;

// a exstension class for the buttons to check if it the right tag for a hand
namespace Phi.ButtonExstensions {
    public static class Button {
        public static bool isTriggerButton(this Collider col) {
            return col.tag == "PlayerHand";
        }
    }
}
