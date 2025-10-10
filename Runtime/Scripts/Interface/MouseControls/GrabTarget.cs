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
        private bool isReleasing;

        public GrabTarget(GrabBehaviour callbacks) {
            if (callbacks == null) {
                throw new NullReferenceException();
            }
            Behaviour = callbacks;
            CurrentGrabButton = MouseButton.None;
        }

        public bool IsHovering { get; private set; }
        public bool IsGrabbed => draggedInstance == this || clickedInstance == this;
        public bool DraggingIsEnabled(MouseButton dragButton) => true;// TargetIsDraggable(dragButton);

        private bool TargetIsDisabled(MouseButton button) => !Behaviour.GrabbingIsEnabled ||
            Behaviour.GetMode(button) == GrabBehaviour.Mode.DISABLED;

        private bool TargetIsDraggable(MouseButton button) => Behaviour.GrabbingIsEnabled &&
            (Behaviour.GetMode(button) == GrabBehaviour.Mode.GRAB ||
            Behaviour.GetMode(button) == GrabBehaviour.Mode.ONLY_DRAG);

        private bool TargetIsDraggableOnly (MouseButton button) => Behaviour.GrabbingIsEnabled &&
            Behaviour.GetMode(button) == GrabBehaviour.Mode.ONLY_DRAG;

        private bool TargetIsClickableOnly(MouseButton button) => Behaviour.GrabbingIsEnabled &&
            Behaviour.GetMode(button) == GrabBehaviour.Mode.ONLY_CLICK;

        private void Hover () {
            Behaviour.OnHovering(!IsHovering);
            IsHovering = true;
        }

        private void StartGrab(MouseButton button, Vector2 uiPosition) {
            CurrentGrabButton = button;
            Behaviour.OnGrabbing(true, null);
            grabStartTime = Time.unscaledTime;
            grabStartPosition = uiPosition;
        }

        private void UpdateDrag () {
            if (IsGrabbed) {
                if (TargetIsClickableOnly(CurrentGrabButton)) {
                    CancelGrab();
                } else {
                    Behaviour.OnGrabbing(false, FruityUI.DraggedOverTarget);
                }
            }
        }

        private void CompleteGrab() {
            Behaviour.OnGrabCompleted(FruityUI.DraggedOverTarget);

            draggedInstance = null;
            clickedInstance = null;
            CurrentGrabButton = MouseButton.None;
            grabEndFrame = Time.frameCount;
        }

        public void CancelGrab () {
            if (IsGrabbed) {
                draggedInstance = null;
                clickedInstance = null;
                CurrentGrabButton = MouseButton.None;
                grabEndFrame = Time.frameCount;

                Behaviour.OnGrabCancelled();
            }
        }

        // -------------------------------------------------

        public void MouseHovering (bool firstFrame, HighlightParams highlightParams) {
            var otherIsClicked = clickedInstance != this && clickedInstance != null;
            if (otherIsClicked) {
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

            // Trigger button-mode click
            if (TargetIsClickableOnly(clickParams.ClickButton)) {
                if (!TargetIsDisabled(clickParams.ClickButton)) {
                    Behaviour.OnButtonClick(clickParams.ClickButton);
                }
                return;
            }

            // Start a grab (on injected click event)
            if (TargetIsDraggable(clickParams.ClickButton) && CurrentGrabbedInstance == null && !isReleasing) {
                StartGrab(clickParams.ClickButton, clickParams.MouseUIPosition);
                clickedInstance = this;
            }
            isReleasing = false;

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
                if (clickParams.ClickButton == CurrentGrabButton) {
                    if (Behaviour.CanMultiPlace (FruityUI.HighlightedTarget)) {

                        // Complete but don't end
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

            // Check automatic cancellation
            if (!TargetIsDraggable(dragParams.DragButton)) {
                if (IsGrabbed) {
                    CancelGrab();
                }
                return;
            }

            if (isFirstFrame) {
                // Complete a grab (on second click, mouse down)
                if (CurrentGrabbedInstance == this) {
                    CompleteGrab();
                    isReleasing = true;
                }

                // Start a grab (on drag start)
                else if (CurrentGrabbedInstance == null) {
                    StartGrab(dragParams.DragButton, dragParams.MouseUIPosition);
                    draggedInstance = this;
                }
            }
        }

        public void CompleteMouseDrag (DragParams dragParams) {
            // Complete a grab (on drag)
            if (draggedInstance != null) {
                if (DragIsClick(dragParams) || TargetIsDraggableOnly(dragParams.DragButton)) {
                    CompleteGrab();
                }
            }
        }

        public void CancelMouseDrag () {
            // Cancel a grab (on drag)
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
            var targetDistance = ScreenBounds.BoxedCanvasSize.y / 20;
            return dragDistance > targetDistance;
        }

        private bool ExceedsClickDuration() {
            var dragDuration = (Time.unscaledTime - grabStartTime);
            return dragDuration > MAX_CLICK_DURATION;
        }

    }

}