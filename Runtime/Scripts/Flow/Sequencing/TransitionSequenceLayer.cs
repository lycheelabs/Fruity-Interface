using LycheeLabs.FruityInterface.Flow;

namespace LycheeLabs.FruityInterface {

    public class TransitionSequenceLayer : SequenceLayer {

        public bool IsBlockingLayersBelow => false;
        public bool IsBlockedByLayersAbove { get; set; }
        public bool IsTransitioning => (ActiveEvent != null && !ActiveEvent.IsExiting)
            || QueuedEvent != null;

        private readonly EventSequencer Sequencer;
        private TransitionEvent ActiveEvent;
        private TransitionEvent QueuedEvent;

        public TransitionSequenceLayer (EventSequencer sequencer) {
            Sequencer = sequencer;
        }

        public void Transition (TransitionEvent transition) {
            if (ActiveEvent == null) { 
                ActiveEvent = transition;
                ActiveEvent.Callbacks = this;
            }
            else if (ActiveEvent.IsExiting && QueuedEvent == null) {
                QueuedEvent = transition;
            }
        }

        public void Update() {
            if (ActiveEvent != null) {
                ActiveEvent.Update();
                if (ActiveEvent.IsComplete) {
                    ActiveEvent = QueuedEvent;
                    QueuedEvent = null;
                }
            }
        }

        public void OnTransitionApplied() {
            Sequencer.ClearLayersBelow(this);
        }

        public void Clear() {
            // Don't clear the transition layer
        }

    }

}