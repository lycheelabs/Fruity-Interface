namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Interface for objects that can be dragged with the mouse.
    /// Extend MouseTarget to also receive hover events.
    /// </summary>
    public interface DragTarget : MouseTarget {

        /// <summary>
        /// Determines how this target responds to drag input for a mouse button.
        /// Called when the mouse button is pressed to determine behavior.
        /// Also called during drag to check if drag should be cancelled.
        /// </summary>
        MouseDragMode GetDragMode(MouseButton dragButton);

        /// <summary>
        /// Called every frame while this target is being dragged.
        /// </summary>
        /// <param name="isFirstFrame">True on the first frame of the drag.</param>
        /// <param name="dragParams">
        /// Contains drag state including:
        /// - Target: this drag target
        /// - DraggingOver: the MouseTarget under the cursor (for drop detection)
        /// - Position information for the drag start and current mouse position
        /// </param>
        void UpdateMouseDragging(bool isFirstFrame, DragParams dragParams);

        /// <summary>
        /// Called when the drag is completed (mouse released or second click in pickup mode).
        /// </summary>
        /// <param name="dragParams">Final drag state including what the target was dropped over.</param>
        void ApplyMouseDrag(DragParams dragParams);

        /// <summary>
        /// Called when the drag is cancelled (by clicking a different mouse button, or if GetDragMode returns Disabled).
        /// </summary>
        void CancelMouseDrag();

    }

    /// <summary>
    /// Optional extension of DragTarget that supports multi-place behaviour in pickup (two-click) mode.
    /// When AllowMultiPlace returns true after a placement, the drag remains active so the target
    /// can be placed again on the next click. The loop continues until AllowMultiPlace returns false
    /// or the drag is cancelled. Multi-place does not apply to held-button drags.
    /// </summary>
    public interface MultiPlaceDragTarget : DragTarget {

        /// <summary>
        /// Called after ApplyMouseDrag in pickup mode to determine whether the drag should remain
        /// active for another placement. Return true to keep dragging, false to end the drag.
        /// </summary>
        bool AllowMultiPlace(DragParams dragParams);

    }

    /// <summary>
    /// Describes how a drag target responds to drag input.
    /// </summary>
    public enum MouseDragMode {
        /// <summary>No drag behavior for this button.</summary>
        Disabled,

        /// <summary>
        /// Standard press-and-hold drag behavior.
        /// Mouse down starts drag, mouse up completes it.
        /// </summary>
        DragOnly,

        /// <summary>
        /// Click-to-pickup behavior only.
        /// First click picks up, second click places. No press-and-hold drag.
        /// </summary>
        PickUpOnly,

        /// <summary>
        /// Supports both behaviors:
        /// - Quick click: converts to pickup mode (click again to place)
        /// - Press and hold: standard drag behavior
        /// </summary>
        DragOrPickUp
    }

}