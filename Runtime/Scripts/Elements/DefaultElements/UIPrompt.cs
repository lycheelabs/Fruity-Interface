
using System;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// A UINode that attaches to a canvas. Can be activated and dismissed.
    /// </summary>
    public abstract class UIPrompt : InterfaceNode {

        public interface PromptInstantiator {
            UIPrompt Instantiate(PromptSequenceLayer layer);
        }

        private enum States { OPENING, OPEN, CLOSING, CLOSED }
        private States State = States.OPENING;

        private bool closeQueued;

        public void UpdateFlow (bool isPaused) {
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
                        OnDestroy();
                        Destroy(gameObject);
                    }
                    break;

            }
        }

        public override bool InputIsDisabled => State != States.OPEN;
        public bool HasCompleted { get; private set; }

        public PromptSequenceLayer PromptLayer { get; private set; }
        public abstract bool RestrictMouseInput { get;  }

        public void AttachToLayer (PromptSequenceLayer layer) {
            PromptLayer = layer;
            PromptLayer.Canvas.Attach(this);
        }

        public void Close () {
            closeQueued = true;
        }

        public void CloseImmediately () {
            StartClosing();
            OnDestroy();
            Destroy(gameObject);
        }

        // ------------------ Abstract lifecycle ------------------

        public abstract void StartOpening();
        protected abstract void AnimateOpening(out bool isComplete);
        protected abstract void StartClosing();
        protected abstract void AnimateClosing(out bool isComplete);
        protected abstract void OnDestroy();

    }

}