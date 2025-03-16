using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class StartDragEvent : InterfaceEvent  {
        public DragParams Params;
        public void Activate(bool logging) {
            if (FruityUI.DraggedTarget != null) {
                if (logging) {
                    Debug.Log("Cancel Drag: " + FruityUI.DraggedTarget);
                }
                FruityUI.DraggedTarget.CancelMouseDrag();
            }
            
            if (logging) {
                Debug.Log("Dragging: " + Params.Target);
            }
            FruityUI.DraggedTarget = Params.Target;
            FruityUI.DraggedTarget.MouseDragging(true, Params);
        }
    }
}