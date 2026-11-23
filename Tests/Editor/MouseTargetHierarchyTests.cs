using LycheeLabs.InputTree;
using InputTree.Tests;
using NUnit.Framework;
using UnityEngine;

namespace Hierarchy {

    public class MouseTargetHierarchyTests {

        private HoverHierarchy hover;
        private DragOverHierarchy dragOver;

        [SetUp]
        public void SetUp() {
            MouseHarness.ResetAllState();
            hover = new HoverHierarchy();
            dragOver = new DragOverHierarchy();
        }

        [TearDown]
        public void TearDown() {
            hover.Clear();
            dragOver.Clear();
            MouseHarness.ResetAllState();
        }

        private void Hover(MouseTarget target) {
            var hoverParams = new HoverParams(null, Vector3.zero, MouseButton.None);
            hover.Build(target, hoverParams);
            hover.ApplyDiff(false, hoverParams);
        }

        private void DragOver(MouseTarget target) {
            dragOver.Build(target, null, Vector3.zero, MouseButton.Left);
            dragOver.ApplyDiff(false, new DragParams(null, null, Vector2.zero, Vector2.zero, MouseButton.Left));
        }

        [Test]
        public void Hover_FirstFrame_SetsHighlightedTarget() {
            var a = new RecordingTarget("A");

            Hover(a);

            Assert.AreEqual(1, a.CountOf("hover.first"));
            Assert.AreSame(a, Interface.HighlightedTarget);
        }

        [Test]
        public void Hover_SecondFrame_IsNotFirstFrame() {
            var a = new RecordingTarget("A");

            Hover(a);
            Hover(a);

            Assert.AreEqual(1, a.CountOf("hover.first"));
            Assert.AreEqual(1, a.CountOf("hover"));
            Assert.AreEqual(0, a.CountOf("hoverEnd"));
        }

        [Test]
        public void Hover_MovingToAnotherTarget_EndsPrevious() {
            var a = new RecordingTarget("A");
            var b = new RecordingTarget("B");

            Hover(a);
            Hover(b);

            Assert.AreEqual(1, a.CountOf("hoverEnd"));
            Assert.AreEqual(1, b.CountOf("hover.first"));
            Assert.AreSame(b, Interface.HighlightedTarget);
        }

        [Test]
        public void Hover_LeavingAllTargets_EndsAndClearsHighlight() {
            var a = new RecordingTarget("A");

            Hover(a);
            Hover(null);

            Assert.AreEqual(1, a.CountOf("hoverEnd"));
            Assert.IsNull(Interface.HighlightedTarget);
        }

        [Test]
        public void DragOver_EnteringTarget_SetsDraggedOverTarget() {
            var a = new RecordingTarget("A");

            DragOver(a);

            Assert.AreEqual(1, a.CountOf("over.first"));
            Assert.AreSame(a, Interface.DraggedOverTarget);
        }

        [Test]
        public void DragOver_LeavingTarget_EndsAndClears() {
            var a = new RecordingTarget("A");

            DragOver(a);
            DragOver(null);

            Assert.AreEqual(1, a.CountOf("overEnd"));
            Assert.IsNull(Interface.DraggedOverTarget);
        }

        [Test]
        public void DragOver_IgnoresTargetsThatDoNotReceiveDragOver() {
            var a = new RecordingTarget("A") { ReceivesDragOver = false };

            DragOver(a);

            Assert.AreEqual(0, a.CountOf("over.first"));
            Assert.IsNull(Interface.DraggedOverTarget);
        }

        [Test]
        public void DragOver_IgnoresHoverOnlyTargets() {
            var hoverOnly = new HoverOnlyTarget();

            DragOver(hoverOnly);

            Assert.IsNull(Interface.DraggedOverTarget);
        }
    }

}
