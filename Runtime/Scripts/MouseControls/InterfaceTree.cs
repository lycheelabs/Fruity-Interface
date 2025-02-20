﻿using System.Collections.Generic;

namespace LycheeLabs.FruityInterface  {
    
    public class InterfaceTree {

        private Queue<InterfaceEvent> events;
        private Queue<InterfaceEvent> bufferedEvents;

        public InterfaceTree() {
            events = new Queue<InterfaceEvent>();
            bufferedEvents = new Queue<InterfaceEvent>();
        }

        public void QueueEvent(InterfaceEvent e) {
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