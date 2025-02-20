using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class ClickEvent : InterfaceEvent  {
        public ClickParams Params;
        public void Activate(bool logging)
        {
            var newTarget = Params.Target;

            // Cancel drag if needed
            if (FruityUI.DraggedTarget != null) {
                if (logging) {
                    Debug.Log("Drag cancelled: " + FruityUI.DraggedTarget);
                }
                FruityUI.DraggedTarget.CancelMouseDrag();
                FruityUI.DraggedTarget = null;
            }
            
            // Try unclicking current target
            if (FruityUI.SelectedTarget != null && newTarget != FruityUI.SelectedTarget) {
                var unclicked = FruityUI.SelectedTarget.TryMouseUnclick(Params);
                if (!unclicked) {
                    if (logging) {
                        Debug.Log("Unclick blocked: " + FruityUI.SelectedTarget);
                    }
                    return;
                }
            }

            if (logging) {
                Debug.Log("Click: " + newTarget);
            }
            //OnNewClick?.Invoke(pressedClickParams.Target);

            FruityUI.SelectedTarget = newTarget;
            FruityUI.SelectedTarget?.MouseClick(Params);
        }
    }
}