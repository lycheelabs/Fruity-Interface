namespace LycheeLabs.FruityInterface {

    public interface DragTarget : MouseTarget {

        /// <summary>
        /// If false, this target will not recieve drag events (and any current drag will be cancelled)
        /// </summary>
        bool DraggingIsEnabled (MouseButton dragButton);

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