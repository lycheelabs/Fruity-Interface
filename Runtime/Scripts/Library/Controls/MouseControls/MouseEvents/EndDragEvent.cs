using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired when a drag operation ends (completed or cancelled).
    /// Calls ApplyMouseDrag or CancelMouseDrag and clears FruityUI.DraggedTarget.
    /// CancelTarget is used when FruityUI.DraggedTarget was already cleared synchronously
    /// before this event fired (e.g. via MouseState.CancelDrag during a drag callback).
    /// </summary>
    public class EndDragEvent : ControlEvent {
        public DragParams Params;
        public bool WasCancelled;
        /// <summary>
        /// Captured target for deferred cancellations where FruityUI.DraggedTarget has
        /// already been nulled before this event activates.
        /// </summary>
        public DragTarget CancelTarget;

        public void Activate(bool logging) {
            // Prefer the live DraggedTarget; fall back to the captured reference for
            // deferred cancellations where it was already cleared synchronously.
            var dragTarget = FruityUI.DraggedTarget ?? CancelTarget;
            if (dragTarget == null) return;

            // Safety check for completion: params target should match
            if (!WasCancelled && Params.Target != dragTarget) {
                if (logging) Debug.Log("Drag target mismatch on complete, cancelling: " + dragTarget);
                dragTarget.CancelMouseDrag();
                FruityUI.DraggedTarget = null;
                return;
            }

            if (WasCancelled) {
                if (logging) Debug.Log("Cancel Drag: " + dragTarget);
                dragTarget.CancelMouseDrag();
            } else {
                if (logging) Debug.Log("Complete Drag: " + dragTarget);
                dragTarget.ApplyMouseDrag(Params);
            }

            FruityUI.DraggedTarget = null;
        }
    }
}