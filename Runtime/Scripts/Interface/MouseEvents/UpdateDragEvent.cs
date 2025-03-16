using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class UpdateDragEvent : InterfaceEvent  {
        public DragParams Params;
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
                    FruityUI.DraggedTarget.MouseDragging(false, Params);
                }
            }
        }
    }
}