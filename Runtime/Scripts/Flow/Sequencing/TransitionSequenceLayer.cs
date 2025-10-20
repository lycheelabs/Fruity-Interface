using LycheeLabs.FruityInterface.Flow;
using System;

namespace LycheeLabs.FruityInterface {
    public class TransitionSequenceLayer : SequenceLayer {

        public bool IsBlockingLayersBelow => false;
        public bool IsBlockedByLayersAbove { get; set; }

        private readonly EventSequencer Sequencer;
        private TransitionEvent ActiveEvent;

        public TransitionSequenceLayer(EventSequencer sequencer) {
            Sequencer = sequencer;
        }

        public void Transition (TransitionEvent transition) {
            if (ActiveEvent == null) { 
                ActiveEvent = transition;
                ActiveEvent.Callbacks = this;
            }
        }

        public void Update() {
            if (ActiveEvent != null) {
                ActiveEvent.Update();
                if (ActiveEvent.IsComplete) {
                    ActiveEvent = null;
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