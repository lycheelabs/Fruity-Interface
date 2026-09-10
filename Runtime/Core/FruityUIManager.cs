using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class FruityUIManager : MonoBehaviour {

        public static FruityUIManager Instance { get; private set; }

        public static void SubmitRawInput(RawInputEvent inputEvent) {
            if (Instance != null) {
                Instance.QueueInputEvent(inputEvent);
            }
        }

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
        public bool LogRawInput;
        public bool LogRaycasts;

        private MouseState mouseState;
        private ControlEventQueue events;
        private RawInputEventQueue inputEvents;
        private bool externalInputModuleActive;

        internal void QueueInputEvent(RawInputEvent inputEvent) {
            if (LogRawInput) {
                Debug.Log($"[FruityUIManager] Raw input: {inputEvent.Type}, button={inputEvent.Button}, " +
                    $"position={inputEvent.ScreenPosition}, time={inputEvent.Time}");
            }
            inputEvents.Enqueue(inputEvent);
        }

        internal void DrainInputEvents(Action<RawInputEvent> handler) {
            inputEvents.Drain(handler);
        }

        internal void ClearInputEvents() {
            inputEvents.Clear();
        }

        public void SetExternalInputModuleActive(bool active) {
            externalInputModuleActive = active;
        }

        public void ProcessExternalInputModule() {
            ProcessInput();
        }
        
        private void Awake () { 
            Instance = this;
            mouseState = new MouseState();
            events = new ControlEventQueue();
            inputEvents = new RawInputEventQueue();
            mouseState.LogRaycasts = LogRaycasts;
            
            FruityUI.SetAspect(MinAspectRatio, MaxAspectRatio);

            // Default settings
            FruityUI.SetUICamera(Camera.main);
            FruityUI.SetWorldPlane(new Plane(Vector3.up, Vector3.zero));
        }

        void Update () {
            if (externalInputModuleActive) return;
            ProcessInput();
        }

        private void ProcessInput() {
            FruityUI.Update();
            inputEvents.Drain(mouseState.QueueInputEvent);
            mouseState.Update();
            events.Update(LogEvents);
        }

    }

}
