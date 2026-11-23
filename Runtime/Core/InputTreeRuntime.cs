using System;
using UnityEngine;

namespace LycheeLabs.InputTree {

    internal sealed class InputTreeRuntime {

        private static InputTreeRuntime active = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad() {
            active = null;
        }

        public static void SubmitRawInput(RawInputEvent inputEvent) {
            if (active != null) {
                active.QueueInputEvent(inputEvent);
            }
        }

        internal static void TriggerNewClick (MouseTarget target, MouseButton button) {
            if (active == null) {
                Debug.LogWarning("The scene contains no InputTreeInputModule!");
                return;
            }
            if (target != null) {
                active.mouseState.QueueClick(target, button);
            }
        }

        internal static void CancelDrag (DragTarget target) {
            if (active == null) {
                Debug.LogWarning("The scene contains no InputTreeInputModule!");
                return;
            }
            if (target != null) {
                active.mouseState.CancelDrag(target);
            }
        }

        internal static void QueueEvent (ControlEvent newEvent) {
            if (active == null) {
                Debug.LogWarning("The scene contains no InputTreeInputModule!");
                return;
            }
            if (newEvent != null) {
                active.events.Queue(newEvent);
            }
        }

        // ------------------------------------------------------------------------

        private MouseState mouseState;
        private ControlEventQueue events;
        private RawInputEventQueue inputEvents;

        public bool LogEvents { get; }
        public bool LogRawInput { get; }

        internal InputTreeRuntime(bool logEvents, bool logRawInput, bool logRaycasts)
            : this(new MouseState { LogRaycasts = logRaycasts }, logEvents, logRawInput) {
        }

        internal InputTreeRuntime(MouseState mouseState, bool logEvents, bool logRawInput) {
            this.mouseState = mouseState;
            events = new ControlEventQueue();
            inputEvents = new RawInputEventQueue();
            LogEvents = logEvents;
            LogRawInput = logRawInput;
            active = this;
        }

        internal void QueueInputEvent(RawInputEvent inputEvent) {
            if (LogRawInput) {
                Debug.Log($"[InputTreeRuntime] Raw input: {inputEvent.Type}, button={inputEvent.Button}, " +
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

        internal void Process() {
            Interface.Update();
            ProcessInput();
        }

        internal void Dispose() {
            if (active == this) active = null;
        }

        private void ProcessInput() {
            inputEvents.Drain(mouseState.QueueInputEvent);
            mouseState.Update();
            events.Update(LogEvents);
        }

    }

}
