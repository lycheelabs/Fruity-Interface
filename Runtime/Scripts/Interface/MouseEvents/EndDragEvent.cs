using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class EndDragEvent : InterfaceEvent {
        public DragParams Params;
        public bool WasCancelled;
        public void Activate(bool logging) {
            if (FruityUI.DraggedTarget != null) {
                if (Params.Target != FruityUI.DraggedTarget) {
                    if (logging) {
                        Debug.Log("Cancel Drag: " + FruityUI.DraggedTarget);
                    }
                    FruityUI.DraggedTarget.CancelMouseDrag();
                    FruityUI.DraggedTarget = null;
                }
                else {
                    if (WasCancelled) {
                        if (logging) {
                            Debug.Log("Cancel Drag: " + FruityUI.DraggedTarget);
                        }
                        FruityUI.DraggedTarget.CancelMouseDrag();
                        FruityUI.DraggedTarget = null;
                    }
                    else  {
                        if (logging) {
                            Debug.Log("Dragged: " + FruityUI.DraggedTarget);
                        }
                        FruityUI.DraggedTarget.CompleteMouseDrag(Params);
                        FruityUI.DraggedTarget = null;
                    }
                }
            }
        }
    }
}