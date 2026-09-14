using System;

namespace LycheeLabs.FruityInterface.Elements {

    public sealed class CallbackButtonEffect : ButtonEffect {

        private Action callback;

        public void Bind (Action callback) {
            this.callback = callback;
        }

        public void Unbind () {
            callback = null;
        }

        public override void MouseOver () {
            // The ButtonNode owns hover animation and input feedback.
        }

        public override void Activate (MouseButton clickButton) {
            callback?.Invoke();
        }

        private void OnDestroy () {
            callback = null;
        }

    }

}
