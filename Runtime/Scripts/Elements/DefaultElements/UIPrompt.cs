

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// A UINode that attaches to a canvas. Can be activated and dismissed.
    /// </summary>
    public abstract class UIPrompt : InterfaceNode {

        public interface PromptInstantiator {
            UIPrompt Instantiate(PromptSequenceLayer layer);
        }

        private enum States { PAUSED, OPENING, OPEN, CLOSING, CLOSED }
        private States State = States.OPENING;

        private bool closing;
        private bool pausing;
        private bool reopening;

        public void UpdateFlow (bool isPaused) {
            switch (State) {

                case States.PAUSED:
                    if (reopening) {
                        State = States.OPENING;
                    }
                    break;

                case States.OPENING:
                    AnimateOpening(out bool isOpen);
                    if (isOpen) {
                        State = States.OPEN;
                    }
                    break;

                case States.OPEN:
                    if (pausing || closing) {
                        State = States.CLOSING;
                        StartClosing();
                    }
                    break;

                case States.CLOSING:
                    AnimateClosing(out bool hasClosed);
                    if (hasClosed) {
                        if (closing) {
                            // Destroy now
                            State = States.CLOSED;
                            HasCompleted = true;
                            OnDestroy();
                            Destroy(gameObject);
                        }
                        else {
                            // Pause only
                            State = States.PAUSED;
                            HasPaused = true;
                        }
                    }
                    break;

            }
        }

        public override bool InputIsDisabled => State != States.OPEN;
        public bool IsClosing => closing || pausing;
        public bool HasPaused { get; private set; }
        public bool HasCompleted { get; private set; }

        public PromptSequenceLayer PromptLayer { get; private set; }
        public abstract bool RestrictMouseInput { get;  }

        public void AttachToLayer (PromptSequenceLayer layer) {
            PromptLayer = layer;
            PromptLayer.Canvas.Attach(this);
        }

        /// <summary>
        /// Closes this prompt and instantiates the next prompt in its place.
        /// This prompt is hidden (not destroyed) so that navigating backwards will reopen this prompt.
        /// </summary>
        public void ProceedTo (PromptInstantiator nextPrompt) {
            if (!closing && !pausing) {
                pausing = true;
                reopening = false;
                PromptLayer.Prompt(nextPrompt);
            }
        }

        /// <summary>
        /// Reopen this prompt after it was hidden.
        /// </summary>
        public void Reopen () {
            if (pausing && !closing) {
                pausing = false;
                reopening = true;
                HasPaused = false;
            }
        }

        /// <summary>
        /// Closes this prompt. It will be destroyed once the animation is complete.
        /// </summary>
        public void Close () {
            closing = true;
        }

        /// <summary>
        /// Closes this prompt and immediately destroys it, skipping any closing animations.
        /// </summary>
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