namespace LycheeLabs.FruityInterface {

    public interface DragTarget : MouseTarget {

        /// <summary>
        /// Describes how a drag target responds to drag input.
        /// </summary>
        public enum DragMode {
            /// <summary>No drag behavior for this button.</summary>
            Disabled,
            /// <summary>Standard drag: press and hold to drag, release to complete.</summary>
            Hold,
            /// <summary>Grab drag: click to pick up, click again to place.</summary>
            Grab
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