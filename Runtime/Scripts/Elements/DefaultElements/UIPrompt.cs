
using System;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// A UINode that attaches to a canvas. Can be activated and dismissed.
    /// </summary>
    public abstract class UIPrompt : InterfaceNode {

        public interface Instantiator {
            UIPrompt Instantiate(UICanvas canvas);
        }

        public static T SpawnGeneric<T> (string name, UICanvas canvas) where T : UIPrompt {
            var rootObject = FruityUIPrefabs.NewUIRect();
            rootObject.name = name;

            var prompt = rootObject.AddComponent<T>();
            canvas?.Attach(prompt);

            return prompt;
        } 

        // -------------------------------------------------------

        private enum States { OPENING, OPEN, CLOSING, CLOSED }
        private States State = States.OPENING;

        private bool closeQueued;

        public void UpdateFlow () {
            switch (State) {

                case States.OPENING:
                    AnimateOpening(out bool isOpen);
                    if (isOpen) {
                        State = States.OPEN;
                    }
                    break;

                case States.OPEN:
                    if (closeQueued) {
                        State = States.CLOSING;
                        StartClosing();
                    }
                    break;

                case States.CLOSING:
                    AnimateClosing(out bool isClosed);
                    if (isClosed) {
                        State = States.CLOSED;
                        HasCompleted = true;
                        Deactivate();
                        Destroy(gameObject);
                    }
                    break;

            }
        }

        public override bool InputIsDisabled => State != States.OPEN;
        public bool HasCompleted { get; private set; }
        public void Close () {
            closeQueued = true;
        }

        // ------------------ Abstract lifecycle ------------------

        public abstract void Activate();
        protected abstract void AnimateOpening(out bool isComplete);
        protected abstract void StartClosing();
        protected abstract void AnimateClosing(out bool isComplete);
        protected abstract void Deactivate();

    }

}