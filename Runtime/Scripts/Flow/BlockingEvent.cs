
namespace LycheeLabs.FruityInterface {

    public interface BlockingEvent {
        void Activate();
        void Update();
        bool IsComplete { get; }
        void Finish();
    }

}