using LycheeLabs.FruityInterface;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FruityInterface.Tests {

    /// <summary>
    /// Drives a real FruityInputRuntime with a fake raycaster and clock.
    /// Each Press/Release/Move/Cancel queues raw input and runs one Process() step.
    /// </summary>
    public sealed class MouseHarness : IDisposable {

        public readonly FakeRaycaster Raycaster = new FakeRaycaster();
        public readonly FakeClock Clock = new FakeClock();
        public readonly List<string> Log = new List<string>();

        private readonly FruityInputRuntime runtime;

        public MouseHarness() {
            ResetAllState();
            Clock.UnscaledTime = 1f;
            runtime = new FruityInputRuntime(new MouseState(Raycaster, Clock), false, false);
        }

        public RecordingTarget CreateTarget(string name) {
            return new RecordingTarget(name, Log);
        }

        public void Move(Vector2 screenPosition) {
            runtime.QueueInputEvent(RawInputEvent.Move(0, screenPosition, Clock.UnscaledTime));
            Step();
        }

        public void Press(MouseButton button, Vector2 screenPosition = default) {
            runtime.QueueInputEvent(RawInputEvent.ButtonDown(0, button, screenPosition, Clock.UnscaledTime));
            Step();
        }

        public void Release(MouseButton button, Vector2 screenPosition = default) {
            runtime.QueueInputEvent(RawInputEvent.ButtonUp(0, button, screenPosition, Clock.UnscaledTime));
            Step();
        }

        public void Cancel() {
            runtime.QueueInputEvent(RawInputEvent.Cancel(0, Vector2.zero, Clock.UnscaledTime));
            Step();
        }

        public void Step() {
            runtime.Process();
        }

        public void AdvanceTime(float seconds) {
            Clock.UnscaledTime += seconds;
            Clock.FrameCount++;
        }

        public void Dispose() {
            runtime.Dispose();
            ResetAllState();
        }

        public static void ResetAllState() {
            FruityUI.ResetStateForTests();
            MouseState.ResetStateForTests();
            HoverHierarchyEvent.ResetStateForTests();
            DragOverHierarchyEvent.ResetStateForTests();
        }
    }

}
