using LycheeLabs.FruityInterface.Flow;

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
                Sequencer.RefreshLayers(); // Not actually needed here
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

    }

}