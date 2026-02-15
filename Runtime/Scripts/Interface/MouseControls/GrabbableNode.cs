using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// An abstract UINode that provides grabbing (drag and pickup) behaviour to GameObjects.
    /// The actual drag/pickup logic is handled by MouseState - this class just provides the interface.
    /// </summary>
    public abstract class GrabbableNode : InterfaceNode, ClickTarget, DragTarget {

        private bool _isHovering;

        public bool IsHighlighted => _isHovering;
        public bool IsDragging => FruityUI.DraggedTarget == this;

        // DragTarget implementation - can be overridden
        public virtual DragTarget.DragMode GetDragMode(MouseButton dragButton) =>
            (dragButton == MouseButton.Left) ? DragTarget.DragMode.DragOrPickUp : DragTarget.DragMode.Disabled;

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

        // Abstract methods for subclasses to implement
        public abstract void OnHovering(bool isFirstFrame);
        public abstract void OnHoverEnd();
        public abstract void OnButtonClick(MouseButton button);
        public abstract void OnGrabbing(bool isFirstFrame, MouseTarget draggingOver);
        public abstract void OnGrabCompleted(MouseTarget draggingOver);
        public abstract void OnGrabCancelled();

    }

}