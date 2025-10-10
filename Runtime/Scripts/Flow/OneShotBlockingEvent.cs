
namespace LycheeLabs.FruityInterface {
    public abstract class OneShotBlockingEvent : BlockingEvent {
        public abstract void Activate();
        public void Update(bool isPaused, out bool isComplete) {
            isComplete = true;
        }
        public void Deactivate() { }

    }

}