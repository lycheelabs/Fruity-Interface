using System;
using System.Collections.Generic;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// FIFO queue for input transitions captured outside the EventSystem tick.
    /// Events enqueued while draining are processed by the next drain.
    /// </summary>
    internal sealed class RawInputEventQueue {

        private Queue<RawInputEvent> bufferedEvents = new Queue<RawInputEvent>();
        private Queue<RawInputEvent> activeEvents = new Queue<RawInputEvent>();

        public int Count => bufferedEvents.Count + activeEvents.Count;

        public void Enqueue(RawInputEvent inputEvent) {
            bufferedEvents.Enqueue(inputEvent);
        }

        public void Drain(Action<RawInputEvent> handler) {
            if (handler == null) throw new ArgumentNullException(nameof(handler));

            (bufferedEvents, activeEvents) = (activeEvents, bufferedEvents);
            while (activeEvents.Count > 0) {
                handler(activeEvents.Dequeue());
            }
        }

        public void Clear() {
            bufferedEvents.Clear();
            activeEvents.Clear();
        }

    }

}
