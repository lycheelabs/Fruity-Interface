using System.Collections.Generic;

namespace LycheeLabs.FruityInterface  {
    
    internal class ControlEventQueue {

        private Queue<ControlEvent> events;
        private Queue<ControlEvent> bufferedEvents;

        public ControlEventQueue() {
            events = new Queue<ControlEvent>();
            bufferedEvents = new Queue<ControlEvent>();
        }

        public void Queue(ControlEvent e) {
            bufferedEvents.Enqueue(e);
        }
        
        public void Update(bool logging) {
            // Swap buffers
            (bufferedEvents, events) = (events, bufferedEvents);

            // Activate events
            while (events.Count > 0) {
                events.Dequeue().Activate(logging);
            }
        }

    }
    
}