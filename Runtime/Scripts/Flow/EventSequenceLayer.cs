using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public class EventSequenceLayer : SequenceLayer {

        public bool IsBlockingLayersBelow => IsAnimating;
        public bool IsAnimating => ActiveEvent != null || QueuedEvents.Count > 0;
        public bool IsBlockedByLayersAbove { get; set; }

        private readonly EventSequencer Sequencer;
        private List<BlockingEvent> QueuedEvents;
        private BlockingEvent ActiveEvent;

        public EventSequenceLayer(EventSequencer sequencer) {
            Sequencer = sequencer;
            QueuedEvents = new List<BlockingEvent>();
        }

        public void Queue(BlockingEvent newEvent) {
            if (newEvent == null) {
                Debug.LogWarning("Event is null");
                return;
            }

            QueuedEvents.Add(newEvent);
            Sequencer.RefreshLayers();

            if (!IsAnimating) {
                ActivateNextEvent(QueuedEvents);
            }
        }

        public void Update() {

            // Update active event
            if (ActiveEvent != null) {
                ActiveEvent.Update(IsBlockedByLayersAbove, out var isComplete);
                if (isComplete) {
                    ActiveEvent.Deactivate();
                    ActiveEvent = null;
                }
            }

            // Activate next queued event
            if (!IsBlockedByLayersAbove) {
                if (ActiveEvent == null && QueuedEvents.Count > 0) {
                    ActivateNextEvent(QueuedEvents);
                    return;
                }
            }

        }

        private void ActivateNextEvent(List<BlockingEvent> queue) {
            ActiveEvent = queue[0];
            queue.RemoveAt(0);
            ActiveEvent.Activate();
        }

    }

}