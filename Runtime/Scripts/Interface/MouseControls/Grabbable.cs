using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// An abstract class that provides grabbing (drag and pickup) behavior without requiring a MonoBehaviour.
    /// Implements ClickTarget and DragTarget directly.
    /// The actual drag/pickup logic is handled by MouseState - this class just provides the interface.
    /// </summary>
    public abstract class Grabbable : ClickTarget, DragTarget {

        private bool _isHovering;

        public bool IsHighlighted => _isHovering;
        public bool IsGrabbed => FruityUI.DraggedTarget == this;

        // Override this to control whether grabbing is enabled
        public virtual bool GrabbingIsEnabled => true;

        // DragTarget implementation
        public virtual DragTarget.DragMode GetDragMode(MouseButton dragButton) {
            if (!GrabbingIsEnabled) return DragTarget.DragMode.Disabled;
            return (dragButton == MouseButton.Left) ? DragTarget.DragMode.DragOrPickUp : DragTarget.DragMode.Disabled;
        }

        // ClickTarget implementation  
        public virtual bool ClickOnMouseDown => false;

        public void MouseHovering(bool isFirstFrame, HighlightParams highlightParams) {
            // Don't hover if something else is being dragged
            if (FruityUI.DraggedTarget != null && FruityUI.DraggedTarget != this) {
                MouseHoverEnd();
                return;
            }
            
            OnHovering(!_isHovering);
            _isHovering = true;
        }

        public void MouseHoverEnd() {
            OnHoverEnd();
            _isHovering = false;
        }

        public void MouseClick(ClickParams clickParams) {
            // Only called when DragMode is Disabled
            OnButtonClick(clickParams.ClickButton);
        }

        public bool TryMouseUnclick(ClickParams clickParams) => true;

        public void MouseDragging(bool isFirstFrame, DragParams dragParams) {
            OnGrabbing(isFirstFrame, FruityUI.DraggedOverTarget);
        }

        public void CompleteMouseDrag(DragParams dragParams) {
            OnGrabCompleted(FruityUI.DraggedOverTarget);
        }

        public void CancelMouseDrag() {
            OnGrabCancelled();
        }

        /// <summary>
        /// Cancel the current grab/drag operation.
        /// </summary>
        public void CancelGrab() {
            if (FruityUI.DraggedTarget == this) {
                // This will trigger CancelMouseDrag via the event system
                FruityUI.DraggedTarget.CancelMouseDrag();
                FruityUI.DraggedTarget = null;
            }
        }

        // Abstract methods for subclasses to implement
        public abstract void OnHovering(bool isFirstFrame);
        public abstract void OnHoverEnd();
        public abstract void OnButtonClick(MouseButton button);
        public abstract void OnGrabbing(bool isFirstFrame, MouseTarget draggingOver);
        public abstract void OnGrabCompleted(MouseTarget draggingOver);
        public abstract void OnGrabCancelled();

    }

}