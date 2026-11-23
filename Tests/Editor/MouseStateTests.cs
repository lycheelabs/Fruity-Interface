using LycheeLabs.InputTree;
using InputTree.Tests;
using NUnit.Framework;
using UnityEngine;

namespace Input {

    public class MouseStateTests {

        private MouseHarness harness;
        private RecordingTarget a;
        private RecordingTarget b;

        [SetUp]
        public void SetUp() {
            harness = new MouseHarness();
            a = harness.CreateTarget("A");
            b = harness.CreateTarget("B");
        }

        [TearDown]
        public void TearDown() {
            harness.Dispose();
        }

        [Test]
        public void Click_PressAndRelease_FiresOnce() {
            harness.Raycaster.Target = a;

            harness.Press(MouseButton.Left);
            harness.Release(MouseButton.Left);

            Assert.AreEqual(1, a.CountOf("click"));
        }

        [Test]
        public void Click_HeldDuration_MatchesTimeBetweenPressAndRelease() {
            harness.Raycaster.Target = a;

            harness.Press(MouseButton.Left);
            harness.AdvanceTime(0.25f);
            harness.Release(MouseButton.Left);

            Assert.AreEqual(0.25f, a.LastClickParams.HeldDuration, 0.001f);
        }

        [Test]
        public void Click_RapidRepress_IsDebounced() {
            harness.Raycaster.Target = a;

            harness.Press(MouseButton.Left);
            harness.Release(MouseButton.Left);
            harness.AdvanceTime(0.02f);
            harness.Press(MouseButton.Left);
            harness.Release(MouseButton.Left);

            Assert.AreEqual(1, a.CountOf("click"));

            harness.AdvanceTime(0.1f);
            harness.Press(MouseButton.Left);
            harness.Release(MouseButton.Left);

            Assert.AreEqual(2, a.CountOf("click"));
        }

        [Test]
        public void Click_ClickOnMouseDown_FiresOnPress() {
            a.ClickOnMouseDown = true;
            harness.Raycaster.Target = a;

            harness.Press(MouseButton.Left);
            Assert.AreEqual(1, a.CountOf("click"));

            harness.Release(MouseButton.Left);
            Assert.AreEqual(1, a.CountOf("click"));
        }

        [Test]
        public void Drag_DragOnly_StartsAndDropsOnRelease() {
            a.DragMode = MouseDragMode.DragOnly;
            harness.Raycaster.Target = a;

            harness.Press(MouseButton.Left);
            harness.Move(new Vector2(50, 0));
            harness.Release(MouseButton.Left);

            Assert.AreEqual(1, a.CountOf("dragStart"));
            Assert.GreaterOrEqual(a.CountOf("drag"), 1);
            Assert.AreEqual(1, a.CountOf("dragDrop"));
            Assert.AreSame(a, a.LastDropParams.DraggingOver);
            Assert.IsNull(FruityUI.DraggedTarget);
        }

        [Test]
        public void Drag_DropOnOtherTarget_ReportsDraggingOver() {
            a.DragMode = MouseDragMode.DragOnly;
            harness.Raycaster.Target = a;
            harness.Press(MouseButton.Left);

            harness.Raycaster.Target = b;
            harness.Move(new Vector2(50, 0));
            harness.Release(MouseButton.Left);

            Assert.AreEqual(1, b.CountOf("over.first"));
            Assert.AreSame(b, a.LastDropParams.DraggingOver);
        }

        [Test]
        public void Drag_CancelledByOppositeButton() {
            a.DragMode = MouseDragMode.DragOnly;
            harness.Raycaster.Target = a;
            harness.Press(MouseButton.Left);

            harness.Press(MouseButton.Right);

            Assert.AreEqual(1, a.CountOf("dragCancel"));
            Assert.AreEqual(0, a.CountOf("dragDrop"));
            Assert.IsNull(FruityUI.DraggedTarget);
        }

        [Test]
        public void Drag_CancelledWhenDragModeBecomesDisabled() {
            a.DragMode = MouseDragMode.DragOnly;
            harness.Raycaster.Target = a;
            harness.Press(MouseButton.Left);

            a.DragMode = MouseDragMode.Disabled;
            harness.Move(new Vector2(50, 0));

            Assert.AreEqual(1, a.CountOf("dragCancel"));
            Assert.AreEqual(0, a.CountOf("dragDrop"));
        }

        [Test]
        public void Drag_RawCancelEvent_CancelsDrag() {
            a.DragMode = MouseDragMode.DragOnly;
            harness.Raycaster.Target = a;
            harness.Press(MouseButton.Left);

            harness.Cancel();

            Assert.AreEqual(1, a.CountOf("dragCancel"));
            Assert.IsNull(FruityUI.DraggedTarget);
        }

        [Test]
        public void Drag_PublicCancelDrag_CancelsDrag() {
            a.DragMode = MouseDragMode.DragOnly;
            harness.Raycaster.Target = a;
            harness.Press(MouseButton.Left);

            FruityUI.CancelDrag(a);
            harness.Step();

            Assert.AreEqual(1, a.CountOf("dragCancel"));
            Assert.AreEqual(0, a.CountOf("dragDrop"));
        }

        [Test]
        public void Pickup_PickUpOnly_ClickToPlace() {
            a.DragMode = MouseDragMode.PickUpOnly;
            harness.Raycaster.Target = a;

            harness.Press(MouseButton.Left);
            harness.Release(MouseButton.Left);
            Assert.AreEqual(1, a.CountOf("dragStart"));
            Assert.AreEqual(0, a.CountOf("dragDrop"));

            harness.AdvanceTime(0.1f);
            harness.Press(MouseButton.Left);

            Assert.AreEqual(1, a.CountOf("dragDrop"));
            Assert.AreEqual(0, a.CountOf("click"));
        }

        [Test]
        public void Pickup_DragOrPickUp_ShortClickConvertsToPickUp() {
            a.DragMode = MouseDragMode.DragOrPickUp;
            harness.Raycaster.Target = a;

            harness.Press(MouseButton.Left);
            harness.Release(MouseButton.Left);

            Assert.AreEqual(0, a.CountOf("dragDrop"));
            Assert.AreSame(a, FruityUI.DraggedTarget);

            harness.AdvanceTime(0.1f);
            harness.Press(MouseButton.Left);

            Assert.AreEqual(1, a.CountOf("dragDrop"));
        }

        [Test]
        public void Hover_MovingBetweenTargets_UpdatesHighlight() {
            harness.Raycaster.Target = a;
            harness.Move(Vector2.zero);
            Assert.AreEqual(1, a.CountOf("hover.first"));

            harness.Raycaster.Target = b;
            harness.Move(Vector2.one);

            Assert.AreEqual(1, a.CountOf("hoverEnd"));
            Assert.AreEqual(1, b.CountOf("hover.first"));
            Assert.AreSame(b, FruityUI.HighlightedTarget);
        }

        [Test]
        public void TriggerNewClick_ClicksWithoutRawInput() {
            harness.Raycaster.Target = a;

            FruityUI.TriggerNewClick(a, MouseButton.Left);
            harness.Step();

            Assert.AreEqual(1, a.CountOf("click"));
        }
    }

}
