using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class FruityUIManager : MonoBehaviour {

        public static FruityUIManager Instance { get; private set; }

        internal static void TriggerNewClick (MouseTarget target, MouseButton button) {
            if (Instance == null) {
                Debug.LogWarning("The scene contains no FruityUIManager!");
                return;
            }
            if (target != null) {
                Instance.mouseState.QueueClick(target, button);
            }
        }

        internal static void CancelDrag (DragTarget target) {
            if (Instance == null) {
                Debug.LogWarning("The scene contains no FruityUIManager!");
                return;
            }
            if (target != null) {
                Instance.mouseState.CancelDrag(target);
            }
        }

        internal static void QueueEvent (ControlEvent newEvent) {
            if (Instance == null) {
                Debug.LogWarning("The scene contains no FruityUIManager!");
                return;
            }
            if (newEvent != null) {
                Instance.events.Queue(newEvent);
            }
        }

        // ------------------------------------------------------------------------

        public AspectRatio MinAspectRatio = AspectRatio.STANDARD;
        public AspectRatio MaxAspectRatio = AspectRatio.ULTRAWIDE;
        public bool LogEvents;

        private MouseState mouseState;
        private ControlEventQueue events;
        private RawInputEventQueue inputEvents;

        internal void QueueInputEvent(RawInputEvent inputEvent) {
            inputEvents.Enqueue(inputEvent);
        }

        internal void DrainInputEvents(Action<RawInputEvent> handler) {
            inputEvents.Drain(handler);
        }

        internal void ClearInputEvents() {
            inputEvents.Clear();
        }
        
        private void Awake () { 
            Instance = this;
            mouseState = new MouseState();
            events = new ControlEventQueue();
            inputEvents = new RawInputEventQueue();
            
            FruityUI.SetAspect(MinAspectRatio, MaxAspectRatio);

            // Default settings
            FruityUI.SetUICamera(Camera.main);
            FruityUI.SetWorldPlane(new Plane(Vector3.up, Vector3.zero));
        }

        void Update () {
            FruityUI.Update();
            inputEvents.Drain(mouseState.QueueInputEvent);
            mouseState.Update();
            events.Update(LogEvents);
        }

    }

}
