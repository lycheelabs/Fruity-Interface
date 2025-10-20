
namespace LycheeLabs.FruityInterface {

    public interface BlockingEvent {
        void Activate();
        void Update(bool isPaused, out bool isComplete);
        void Deactivate();
    }

}