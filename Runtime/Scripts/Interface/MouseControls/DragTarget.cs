namespace LycheeLabs.FruityInterface {

    public interface DragTarget : MouseTarget {

        /// <summary>
        /// If false, this target will not recieve drag events (and any current drag will be cancelled)
        /// </summary>
        bool DraggingIsEnabled { get; }

        /// <summary>
        /// Called on the first frame of a mouse drag
        /// </summary>
        void StartMouseDrag(DragParams dragParams);

        /// <summary>
        /// Called on every frame of a mouse drag, including the first
        /// </summary>
        void UpdateMouseDrag (DragParams dragParams);

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