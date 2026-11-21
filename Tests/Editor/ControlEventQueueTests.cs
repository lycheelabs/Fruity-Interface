using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LycheeLabs.FruityInterface.Tests {

    public class ControlEventQueueTests {

        private sealed class RecordingEvent : ControlEvent {

            private readonly string name;
            private readonly List<string> log;
            private readonly Action onActivate;

            public RecordingEvent(string name, List<string> log, Action onActivate = null) {
                this.name = name;
                this.log = log;
                this.onActivate = onActivate;
            }

            public void Activate(bool logging) {
                log.Add(name);
                onActivate?.Invoke();
            }
        }

        private ControlEventQueue queue;
        private List<string> log;

        [SetUp]
        public void SetUp() {
            queue = new ControlEventQueue();
            log = new List<string>();
        }

        [Test]
        public void Queue_NotActivatedUntilUpdate() {
            queue.Queue(new RecordingEvent("a", log));

            Assert.AreEqual(0, log.Count);
        }

        [Test]
        public void Update_ActivatesQueuedEventsInOrder() {
            queue.Queue(new RecordingEvent("a", log));
            queue.Queue(new RecordingEvent("b", log));
            queue.Queue(new RecordingEvent("c", log));

            queue.Update(false);

            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, log);
        }

        [Test]
        public void Update_ClearsEventsAfterActivating() {
            queue.Queue(new RecordingEvent("a", log));

            queue.Update(false);
            queue.Update(false);

            CollectionAssert.AreEqual(new[] { "a" }, log);
        }

        [Test]
        public void Update_EmptyQueue_DoesNothing() {
            queue.Update(false);

            Assert.AreEqual(0, log.Count);
        }

        [Test]
        public void Update_EventQueuedDuringActivation_WaitsForNextUpdate() {
            var second = new RecordingEvent("second", log);
            var first = new RecordingEvent("first", log, () => queue.Queue(second));
            queue.Queue(first);

            queue.Update(false);
            CollectionAssert.AreEqual(new[] { "first" }, log);

            queue.Update(false);
            CollectionAssert.AreEqual(new[] { "first", "second" }, log);
        }
    }

}
