using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// A MouseTarget (both ClickTarget and Dragtarget) that converts basic mouse events 
    /// into grab events, which are  passed to a GrabBehaviour instance.
    /// 'Grabbing' means the object can be dragged normally, 
    /// and also dragged indirectly by clicking once to pick up and clicking again to place.
    /// </summary>
    public class GrabTarget : ClickTarget, DragTarget {

        // Clicks lasting longer than this will be counted as drags
        public const float MAX_CLICK_DURATION = 0.4f;

        private static GrabTarget draggedInstance;
        private static GrabTarget clickedInstance;

        public static GrabTarget CurrentGrabbedInstance => clickedInstance ?? draggedInstance;
        public static GrabBehaviour CurrentGrabbedBehaviour => CurrentGrabbedInstance?.Behaviour;

        public static void UpdateCurrentGrab () {
            CurrentGrabbedInstance?.UpdateDrag();
        }

        // ---------------------------------------------------

        private GrabBehaviour Behaviour;
        private MouseButton CurrentGrabButton;

        private float grabStartTime;
        private Vector2 grabStartPosition;
        private int grabEndFrame;

        public GrabTarget(GrabBehaviour callbacks) {
            if (callbacks == null) {
                throw new NullReferenceException();
            }
            Behaviour = callbacks;
            CurrentGrabButton = MouseButton.None;
        }

        public bool IsHovering { get; private set; }
        public bool IsGrabbed => draggedInstance == this || clickedInstance == this;
        public bool DraggingIsEnabled => true;

        private bool TargetIsDisabled(MouseButton button) =>
            Behaviour.GetMode(button) == GrabBehaviour.Mode.DISABLED;

        private bool TargetIsDraggable(MouseButton button) =>
            Behaviour.GetMode(button) == GrabBehaviour.Mode.GRAB ||
            Behaviour.GetMode(button) == GrabBehaviour.Mode.ONLY_DRAG;

        private bool TargetIsDraggableOnly (MouseButton button) =>
            Behaviour.GetMode(button) == GrabBehaviour.Mode.ONLY_DRAG;

        private bool TargetIsClickableOnly(MouseButton button) =>
            Behaviour.GetMode(button) == GrabBehaviour.Mode.ONLY_CLICK;

        private void UpdateDrag () {
            if (IsGrabbed) {
                if (!DraggingIsEnabled || TargetIsClickableOnly(CurrentGrabButton)) {
                    CancelGrab();
                } else {
                    Behaviour.OnGrabbing(false, FruityUI.DraggedOverTarget);
                }
            }
        }

        private void StartGrab(MouseButton button, Vector2 uiPosition) {
            CurrentGrabButton = button;
            Behaviour.OnGrabbing(true, null);
            grabStartTime = Time.unscaledTime;
            grabStartPosition = uiPosition;
        }

        public void CancelGrab () {
            Behaviour.OnGrabCancelled();

            draggedInstance = null;
            clickedInstance = null;
            CurrentGrabButton = MouseButton.None;
            grabEndFrame = Time.frameCount;
        }

        private void CompleteGrab () {
            Behaviour.OnGrabCompleted(FruityUI.DraggedOverTarget);

            draggedInstance = null;
            clickedInstance = null;
            CurrentGrabButton = MouseButton.None;
            grabEndFrame = Time.frameCount;
        }

        private void Hover () {
            Behaviour.OnHovering(!IsHovering);
            IsHovering = true;
        }

        // -------------------------------------------------

        public void MouseHovering (bool firstFrame, HighlightParams highlightParams) {
            var otherIsClicked = clickedInstance != this && clickedInstance != null;
            if (!DraggingIsEnabled || otherIsClicked) {
                MouseHoverEnd();
                return;
            }
            Hover();
        }

        public void MouseHoverEnd () {
            Behaviour.OnHoverEnd();
            IsHovering = false;
        }

        public void MouseClick (ClickParams clickParams) {
            if (grabEndFrame == Time.frameCount) return;

            // Button mode
            if (TargetIsClickableOnly(clickParams.ClickButton)) {
                if (DraggingIsEnabled && !TargetIsDisabled(clickParams.ClickButton)) {
                    Behaviour.OnButtonClick(clickParams.ClickButton);
                }
                return;
            }

            // Cancel drag
            if (TargetIsDisabled(clickParams.ClickButton)) {
                CancelGrab();
                return;
            }

            // Pass grab state from 'dragging' over to 'clicked'
            if (IsGrabbed) {
                clickedInstance = this;
            }

            // Grabs are completed from CompleteMouseDrag(), not here.
            // Once grabbed, all drag events are overridden with this target.
            // Therefore my drag events will trigger upon a second (placement) click.

        }

        public bool TryMouseUnclick (ClickParams clickParams) {
            if (CurrentGrabbedInstance == this) {
                if (DraggingIsEnabled && clickParams.ClickButton == CurrentGrabButton) {
                    if (Behaviour.CanMultiPlace (FruityUI.HighlightedTarget)) {

                        // Complete but dont end
                        Behaviour.OnGrabCompleted(FruityUI.HighlightedTarget);
                        return false;

                    } else {

                        // Complete and end
                        CompleteGrab();
                        var newDragger = FruityUI.HighlightedTarget as GrabTarget;
                        return newDragger != null && !Behaviour.CanPassGrabTo(newDragger);
                    }
                } else {
                    CancelGrab();
                }
            }
            clickedInstance = null;
            return true;
        }

        public void MouseDragging (bool isFirstFrame, DragParams dragParams) {

            // Check cancellation
            if (!TargetIsDraggable(dragParams.DragButton)) {
                if (IsGrabbed) {
                    CancelGrab();
                }
                return;
            }

            // Start a grab
            if (isFirstFrame) {
                if (CurrentGrabbedInstance != this) {
                    StartGrab(dragParams.DragButton, dragParams.MouseUIPosition);
                }
                draggedInstance = this;
            }
        }

        public void CompleteMouseDrag (DragParams dragParams) {
            if (DragIsClick(dragParams) || TargetIsDraggableOnly(dragParams.DragButton)) {
                CompleteGrab();
            }
        }

        public void CancelMouseDrag () {
            if (draggedInstance == this) {
                CancelGrab();
            }
            draggedInstance = null;
        }

        private bool DragIsClick(DragParams dragParams) {
            return IsGrabbed && (ExceedsClickDuration() || ExceedsClickDistance(dragParams.MouseUIPosition));
        }

        private bool ExceedsClickDistance(Vector2 mousePosition) {
            var dragDistance = (mousePosition - grabStartPosition).magnitude;
            var targetDistance = InterfaceConfig.BoxedCanvasSize.y / 20;
            return dragDistance > targetDistance;
        }

        private bool ExceedsClickDuration() {
            var dragDuration = (Time.unscaledTime - grabStartTime);
            return dragDuration > MAX_CLICK_DURATION;
        }

    }

}