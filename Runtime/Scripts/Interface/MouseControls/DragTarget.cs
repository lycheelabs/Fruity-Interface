namespace LycheeLabs.FruityInterface {

    public interface DragTarget : MouseTarget {

        /// <summary>
        /// Describes how a drag target responds to drag input.
        /// </summary>
        public enum DragMode {
            /// <summary>No drag behavior for this button.</summary>
            Disabled,
            
            /// <summary>Standard press-and-hold drag behavior.</summary>
            DragOnly,
            
            /// <summary>Click once to pick up, click again to place (no press-and-hold drag).</summary>
            PickUpOnly,
            
            /// <summary>Supports both press-and-hold drag AND click-to-pickup behaviors.</summary>
            DragOrPickUp
        }

        /// <summary>
        /// Determines how this target responds to drag input for a mouse button.
        /// </summary>
        DragMode GetDragMode (MouseButton dragButton);

        /// <summary>
        /// Called on every frame of a mouse drag
        /// </summary>
        void MouseDragging (bool isFirstFrame, DragParams dragParams);

        /// <summary>
        /// Called on the last frame of a mouse drag, if the drag was completed
        /// </summary>
        void CompleteMouseDrag (DragParams dragParams);

        /// <summary>
        /// Called on the last frame of a mouse drag, if the drag was cancelled (by clicking a different mouse button)
        /// </summary>
        void CancelMouseDrag ();


    }

}