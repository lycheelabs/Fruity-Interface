using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class UpdateDragEvent : InterfaceEvent  {
        public DragParams Params;
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
                    InterfaceTargets.Dragged.UpdateMouseDrag(Params);
                }
            }
        }
    }
}