using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public abstract class TimedBlockingEvent : BlockingEvent {

        private float time;
        private float duration;

        public TimedBlockingEvent (float duration) {
            this.duration = duration;
        }

        public abstract void Activate();

        public void Update(bool isPaused, out bool isComplete) {
            isComplete = false;
            if (!isPaused) {
                time += Time.deltaTime;
                isComplete = (time >= duration);
            }
        }

        public void Deactivate() { }

    }

}