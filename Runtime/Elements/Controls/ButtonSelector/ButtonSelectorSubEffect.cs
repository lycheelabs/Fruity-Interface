using UnityEngine;

namespace LycheeLabs.InputTree.Elements {

    public class ButtonSelectorSubEffect : ButtonEffect {

        public ButtonSelectorNode Target { get; set; }

        public override void MouseOver () {
            // The child button owns its hover animation.
        }

        public override void Activate (MouseButton clickButton) {
            Target?.OnChildClicked(this);
        }

    }

}
