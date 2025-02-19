using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// A MouseTarget (both ClickTarget and Dragtarget) that converts basic mouse events 
    /// into grab events, which are  passed to a GrabBehaviour instance.
    /// 'Grabbing' means the object can be dragged normally, 
    /// and also dragged by clicking once to pick up and clicking again to place.
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
        private float clickStartTime;
        private float clickDistance;
        private int grabEndFrame;

        public GrabTarget(GrabBehaviour callbacks) {
            if (callbacks == null) {
                throw new NullReferenceException();
            }
            Behaviour = callbacks;
            CurrentGrabButton = MouseButton.None;
        }

        public bool IsHighlighted { get; private set; }
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
                    CancelDrag();
                } else {
                    Behaviour.OnGrabUpdate(Mouse.DragOverTarget);
                }
            }
        }

        public void CancelDrag () {
            Behaviour.OnGrabCancelled();
            Behaviour.OnGrabEnd();

            draggedInstance = null;
            clickedInstance = null;
            CurrentGrabButton = MouseButton.None;
            grabEndFrame = Time.frameCount;
        }

        private void CompleteDrag () {
            Behaviour.OnGrabCompleted(Mouse.DragOverTarget);
            Behaviour.OnGrabEnd();

            draggedInstance = null;
            clickedInstance = null;
            CurrentGrabButton = MouseButton.None;
            grabEndFrame = Time.frameCount;
        }

        private void Highlight () {
            Behaviour.OnHighlight(!IsHighlighted);
            IsHighlighted = true;
        }

        // -------------------------------------------------

        public void MouseHighlight (bool firstFrame, HighlightParams highlightParams) {
            var otherIsClicked = clickedInstance != this && clickedInstance != null;
            if (!DraggingIsEnabled || otherIsClicked) {
                MouseDehighlight();
                return;
            }
            Highlight();
        }

        public void MouseDehighlight () {
            Behaviour.OnDehighlight();
            IsHighlighted = false;
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

            // Cancel/complete drag
            if (TargetIsDisabled(clickParams.ClickButton)) {
                CancelDrag();
                return;
            }
            if (ClickIsExtended()) {
                if (ClickDistanceHasPassed()) {
                    CompleteDrag();
                } else {
                    CancelDrag();
                }
                return;
            } 

            // Start drag
            if (DraggingIsEnabled && !IsGrabbed && !TargetIsDraggableOnly(clickParams.ClickButton)) {
                if (CurrentGrabbedInstance != this) {
                    CurrentGrabButton = clickParams.ClickButton;
                    Behaviour.OnGrabStart();
                    clickStartTime = Time.unscaledTime;
                }
            }
            clickedInstance = this;
            
        }

        public bool TryMouseUnclick (ClickParams clickParams) {
            if (CurrentGrabbedInstance == this) {
                if (DraggingIsEnabled && clickParams.ClickButton == CurrentGrabButton) {
                    if (Behaviour.CanMultiPlace (Mouse.HighlightTarget)) {
                        // Complete but dont end
                        Behaviour.OnGrabCompleted(Mouse.HighlightTarget);
                        return false;
                    } else {
                        // Complete and end
                        CompleteDrag();
                        var newDragger = Mouse.HighlightTarget as GrabTarget;
                        return newDragger != null && !Behaviour.CanPassGrabTo(newDragger);
                    }
                } else {
                    CancelDrag();
                }
            }
            clickedInstance = null;
            return true;
        }

        public void StartMouseDrag(DragParams dragParams) {
            if (!TargetIsDraggable(dragParams.DragButton)) {
                if (IsGrabbed) {
                    CancelDrag();
                }
                return;
            }

            var alreadyDragging = clickedInstance != null && clickedInstance != this;
            if (alreadyDragging) {
                clickedInstance.CancelDrag();
                clickedInstance = null;
                Highlight();
            }

            if (CurrentGrabbedInstance != this) {
                CurrentGrabButton = dragParams.DragButton;
                Behaviour.OnGrabStart();
                clickStartTime = Time.unscaledTime;
            }
            draggedInstance = this;
        }

        public void UpdateMouseDrag (DragParams dragParams) {
            if (!TargetIsDraggable(dragParams.DragButton) && IsGrabbed) {
                CancelDrag();
            }
        }

        public void CompleteMouseDrag (DragParams dragParams) {
            clickDistance = dragParams.UIDragDisplacement.magnitude;
            if (ClickIsExtended() || TargetIsDraggableOnly(dragParams.DragButton)) {
                CompleteDrag();
            }
        }

        public void CancelMouseDrag () {
            if (draggedInstance == this) {
                CancelDrag();
            }
            draggedInstance = null;
        }

        private bool ClickIsExtended() {
            return CurrentGrabbedInstance == this && (ClickDurationHasPassed() || ClickDistanceHasPassed());
        }

        private bool ClickDurationHasPassed () {
            var dragDuration = (Time.unscaledTime - clickStartTime);
            return dragDuration > MAX_CLICK_DURATION;
        }

        private bool ClickDistanceHasPassed() {
            return clickDistance > 40;
        }

    }

}