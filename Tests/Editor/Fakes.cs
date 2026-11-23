using LycheeLabs.FruityInterface;
using System.Collections.Generic;
using UnityEngine;

namespace FruityInterface.Tests {

    public sealed class FakeRaycaster : IMouseRaycaster {

        public MouseTarget Target;
        public InterfaceNode Node;

        public void CollideAndResolve(MouseButton button, out MouseTarget target, out InterfaceNode targetNode, out Vector3 targetPoint) {
            target = Target;
            targetNode = Node;
            targetPoint = Vector3.zero;
        }
    }

    public sealed class FakeClock : IMouseClock {

        public float UnscaledTime { get; set; }
        public int FrameCount { get; set; }
    }

    public sealed class RecordingTarget : ClickTarget, DragTarget, DraggedOverTarget {

        public readonly string Name;
        public readonly List<string> Log;

        public MouseDragMode DragMode = MouseDragMode.Disabled;
        public bool ClickOnMouseDown { get; set; }
        public bool ReceivesDragOver { get; set; } = true;

        public ClickParams LastClickParams;
        public DragParams LastDropParams;

        public RecordingTarget(string name, List<string> log = null) {
            Name = name;
            Log = log ?? new List<string>();
        }

        public int CountOf(string call) {
            return Log.FindAll(entry => entry == Name + ":" + call).Count;
        }

        private void Record(string call) {
            Log.Add(Name + ":" + call);
        }

        public void UpdateMouseHover(bool isFirstFrame, HoverParams highlightParams) {
            Record(isFirstFrame ? "hover.first" : "hover");
        }

        public void EndMouseHover() {
            Record("hoverEnd");
        }

        public void ApplyMouseClick(ClickParams clickParams) {
            LastClickParams = clickParams;
            Record("click");
        }

        public MouseDragMode GetDragMode(MouseButton dragButton) {
            return DragMode;
        }

        public void UpdateMouseDragging(bool isFirstFrame, DragParams dragParams) {
            Record(isFirstFrame ? "dragStart" : "drag");
        }

        public void ApplyMouseDrag(DragParams dragParams) {
            LastDropParams = dragParams;
            Record("dragDrop");
        }

        public void CancelMouseDrag() {
            Record("dragCancel");
        }

        public void UpdateMouseDraggedOver(bool isFirstFrame, DragParams dragParams) {
            Record(isFirstFrame ? "over.first" : "over");
        }

        public void EndMouseDraggedOver() {
            Record("overEnd");
        }
    }

    public sealed class HoverOnlyTarget : MouseTarget {

        public void UpdateMouseHover(bool isFirstFrame, HoverParams highlightParams) { }

        public void EndMouseHover() { }
    }

}
