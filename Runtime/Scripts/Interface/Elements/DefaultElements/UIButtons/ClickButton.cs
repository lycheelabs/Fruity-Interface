
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a UIButton that passes behaviour to a ClickButtonEffect component.
    /// This class makes it easier to create simple button types because they can share prefabs 
    /// (For example, using the ImageButton and TextButton subclasses.)
    /// More complex buttons should implement UIButton directly.
    /// </summary>
    public class ClickButton : ButtonNode, ClickTarget, EnteringElement {

        private ClickButtonEffect _effect;
        public ClickButtonEffect TryGetEffect {
            get {
                _effect = _effect ?? GetComponent<ClickButtonEffect>();
                return _effect;
            }
        }

        protected override void OnHighlight (bool firstFrame, HoverParams highlightParams) {
            if (firstFrame && TryGetEffect != null) {
                TryGetEffect.MouseOver();
            }
        }

        protected override void OnDehighlight () {
            //
        }

        public sealed override void MouseClick (ClickParams clickParams) {
            ButtonAnimator.Squash(3);
            if (TryGetEffect != null && TryGetEffect.MouseButtonIsPermitted (clickParams.ClickButton)) {
                TryGetEffect.Activate(clickParams.ClickButton);
            }
        }

        bool ClickTarget.TryMouseUnclick(ClickParams clickParams) {
            if (TryGetEffect != null) {
                return TryGetEffect.TryUnclick(clickParams.ClickButton);
            }
            return true;
        }

        public void SetEnterTween (float tween) {
            if (TryGetEffect != null) {
                Offset = (1 - tween) * TryGetEffect.EntryOffset;
            }
        }

    }

}