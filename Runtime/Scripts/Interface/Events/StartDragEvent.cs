using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class StartDragEvent : InterfaceEvent  {
        public DragParams Params;
        public void Activate(bool logging) {
            if (InterfaceTargets.Dragged != null) {
                if (logging) {
                    Debug.Log("Cancel Drag: " + InterfaceTargets.Dragged);
                }
                InterfaceTargets.Dragged.CancelMouseDrag();
            }
            
            if (logging) {
                Debug.Log("Dragging: " + Params.Target);
            }
            InterfaceTargets.Dragged = Params.Target;
            InterfaceTargets.Dragged.StartMouseDrag(Params);
        }
    }
}