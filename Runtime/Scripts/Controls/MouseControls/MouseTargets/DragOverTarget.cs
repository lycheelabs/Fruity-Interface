namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Interface for objects that can respond when being dragged over.
    /// Independent from MouseTarget hover behavior.
    /// Only objects implementing this interface will receive drag-over events during a drag operation.
    /// </summary>
    public interface DragOverTarget : MouseTarget {
        
        /// <summary>
        /// Called every frame while a drag operation is hovering over this target.
        /// </summary>
        /// <param name="isFirstFrame">True on the first frame the drag enters this target.</param>
        /// <param name="dragParams">Contains full drag state including what's being dragged and mouse position.</param>
        void MouseDragOver(bool isFirstFrame, DragParams dragParams);
        
        /// <summary>
        /// Called when the drag operation leaves this target.
        /// </summary>
        void MouseDragOverEnd();
    }

}
