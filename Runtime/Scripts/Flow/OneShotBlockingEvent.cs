
namespace LycheeLabs.FruityInterface {
    public abstract class OneShotBlockingEvent : BlockingEvent {
        public abstract void Activate();

        public bool IsComplete => true;
        public void Update() { }
        public void Finish() { }
    }

}