
namespace LycheeLabs.FruityInterface.Flow {

    public interface BlockingEvent {
        void Activate();
        void Update(bool isPaused, out bool isComplete);
        void Deactivate();
    }

}
