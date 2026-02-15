using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// A MouseTarget (both ClickTarget and Dragtarget) that converts basic mouse events 
    /// into grab events, which are passed to a GrabBehaviour instance.
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

        public DragTarget.DragMode GetDragMode(MouseButton dragButton) {
            if (!Behaviour.GrabbingIsEnabled) {
                return DragTarget.DragMode.Disabled;
            }
            return Behaviour.GetDragMode(dragButton);
        }

        private bool TargetIsDisabled(MouseButton button) => 
            !Behaviour.GrabbingIsEnabled ||
            Behaviour.GetDragMode(button) == DragTarget.DragMode.Disabled;

        private bool TargetIsDraggable(MouseButton button) {
            if (!Behaviour.GrabbingIsEnabled) return false;
            var mode = Behaviour.GetDragMode(button);
            return mode == DragTarget.DragMode.DragOnly || 
                   mode == DragTarget.DragMode.DragOrPickUp;
        }

        private bool TargetIsDragOnly(MouseButton button) => 
            Behaviour.GrabbingIsEnabled &&
            Behaviour.GetDragMode(button) == DragTarget.DragMode.DragOnly;

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
                // Restore DraggedTarget if we're in clicked/picked-up state
                // (It gets cleared by EndDragEvent after converting from drag to pickup)
                if (FruityUI.DraggedTarget == null) {
                    FruityUI.DraggedTarget = this;
                }
                
                if (TargetIsDisabled(CurrentGrabButton)) {
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
            
            // Disabled mode - trigger button click if also a ClickTarget
            if (TargetIsDisabled(clickParams.ClickButton)) {
                Behaviour.OnButtonClick(clickParams.ClickButton);
                return;
            }

            // This shouldn't be called for draggable modes (MouseState blocks clicks)
            // But if it is, ignore it
        }

        public bool TryMouseUnclick (ClickParams clickParams) {
            if (CurrentGrabbedInstance == this) {
                if (clickParams.ClickButton == CurrentGrabButton) {
                    if (Behaviour.CanMultiPlace (FruityUI.HighlightedTarget)) {
                        Behaviour.OnGrabCompleted(FruityUI.HighlightedTarget);
                        return false;
                    } else {
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
                // Complete a grab (on second click, mouse down) - for PickUp modes
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
            // Complete a grab (on drag release)
            if (draggedInstance == this) {
                // DragOnly mode always completes on release
                // DragOrPickUp mode only completes if it was a real drag (not a quick click)
                if (TargetIsDragOnly(dragParams.DragButton) || WasRealDrag(dragParams)) {
                    CompleteGrab();
                } else {
                    // Short click in DragOrPickUp mode - convert to clicked/picked-up state
                    clickedInstance = this;
                    draggedInstance = null;
                    // Don't complete - wait for second click
                    // Note: FruityUI.DraggedTarget will be cleared by EndDragEvent, 
                    // but we'll restore it in UpdateDrag()
                }
            }
            
            // If we just converted to clicked state, restore the DraggedTarget that was cleared
            if (clickedInstance == this && FruityUI.DraggedTarget == null) {
                FruityUI.DraggedTarget = this;
            }
        }

        public void CancelMouseDrag () {
            if (draggedInstance == this) {
                CancelGrab();
            }
            draggedInstance = null;
        }

        private bool WasRealDrag(DragParams dragParams) {
            return ExceedsClickDuration() || ExceedsClickDistance(dragParams.MouseUIPosition);
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