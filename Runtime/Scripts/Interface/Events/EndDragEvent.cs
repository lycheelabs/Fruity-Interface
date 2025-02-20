using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class EndDragEvent : InterfaceEvent {
        public DragParams Params;
        public bool WasCancelled;
        public void Activate(bool logging) {
            if (InterfaceTargets.Dragged != null) {
                if (Params.Target != InterfaceTargets.Dragged) {
                    if (logging) {
                        Debug.Log("Cancel Drag: " + InterfaceTargets.Dragged);
                    }
                    InterfaceTargets.Dragged.CancelMouseDrag();
                    InterfaceTargets.Dragged = null;
                }
                else {
                    if (WasCancelled) {
                        if (logging) {
                            Debug.Log("Cancel Drag: " + InterfaceTargets.Dragged);
                        }
                        InterfaceTargets.Dragged.CancelMouseDrag();
                        InterfaceTargets.Dragged = null;
                    }
                    else  {
                        if (logging) {
                            Debug.Log("Dragged: " + InterfaceTargets.Dragged);
                        }
                        InterfaceTargets.Dragged.CompleteMouseDrag(Params);
                        InterfaceTargets.Dragged = null;
                    }
                }
            }
        }
    }
}