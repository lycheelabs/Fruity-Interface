namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Base interface for objects that can receive mouse hover events.
    /// Extended by ClickTarget (for clickable objects) and DragTarget (for draggable objects).
    /// </summary>
    public interface MouseTarget {

        /// <summary>
        /// Called every frame while the mouse is hovering over this target.
        /// During a drag, this is called on the dragged target, not what's under the mouse.
        /// </summary>
        /// <param name="isFirstFrame">True on the first frame of hover.</param>
        /// <param name="highlightParams">Contains mouse position and button state.</param>
        void UpdateMouseHover(bool isFirstFrame, HoverParams highlightParams);

        /// <summary>
        /// Called when the mouse stops hovering over this target.
        /// </summary>
        void EndMouseHover();

    }

}