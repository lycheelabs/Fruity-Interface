using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class FruityUIManager : MonoBehaviour {

        public static FruityUIManager Instance { get; private set; }

        public static void TriggerNewClick (ClickTarget target, MouseButton button) {
            if (Instance != null && target != null) {
                Instance.mouseState.ClickNow(target, button);
            }
        }

        internal static void Queue(InterfaceEvent newEvent) {
            if (Instance != null && newEvent != null) {
                Instance.events.Queue(newEvent);
            }
        }

        // ------------------------------------------------------------------------

        public ScreenAspect MinAspectRatio = ScreenAspect.STANDARD;
        public ScreenAspect MaxAspectRatio = ScreenAspect.ULTRAWIDE;
        public bool LogEvents;

        private MouseState mouseState;
        private EventQueue events;
        
        private void Start () {
            Instance = this;
            mouseState = new MouseState();
            events = new EventQueue();
        }

        void Update () {

            InterfaceConfig.Update(MinAspectRatio, MaxAspectRatio);
            mouseState.Update();
            events.Update(LogEvents);
            GrabTarget.UpdateCurrentGrab();
            
        }

    }

}