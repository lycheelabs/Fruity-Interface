using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public abstract class TimedBlockingEvent : BlockingEvent {

        private float time;
        private float duration;

        public TimedBlockingEvent (float duration) {
            this.duration = duration;
        }

        public abstract void Activate();

        public bool IsComplete => time >= duration;
        public void Update() { time += Time.deltaTime; }
        public void Finish() { }
    }

}