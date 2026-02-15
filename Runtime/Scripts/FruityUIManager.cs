using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class FruityUIManager : MonoBehaviour {

        public static FruityUIManager Instance { get; private set; }

        public static void TriggerNewClick (MouseTarget target, MouseButton button) {
            if (Instance != null && target != null) {
                Instance.mouseState.QueueClick(target, button);
            }
        }

        internal static void Queue(InterfaceEvent newEvent) {
            if (Instance != null && newEvent != null) {
                Instance.events.Queue(newEvent);
            }
        }

        // ------------------------------------------------------------------------

        public AspectRatio MinAspectRatio = AspectRatio.STANDARD;
        public AspectRatio MaxAspectRatio = AspectRatio.ULTRAWIDE;
        public bool LogEvents;

        private MouseState mouseState;
        private InterfaceEventQueue events;
        private ScreenBounds bounds;
        
        private void Awake () { 
            Instance = this;
            mouseState = new MouseState();
            events = new InterfaceEventQueue();
            bounds = new ScreenBounds();
            bounds.Update(MinAspectRatio, MaxAspectRatio);

            // Default settings
            FruityUI.SetUICamera(Camera.main);
            FruityUI.SetWorldPlane(new Plane(Vector3.up, Vector3.zero));
        }

        void Update () {
            bounds.Update(MinAspectRatio, MaxAspectRatio);
            mouseState.Update();
            events.Update(LogEvents);
        }

    }

}