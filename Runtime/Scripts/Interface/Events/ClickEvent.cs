using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class ClickEvent : InterfaceEvent  {
        public ClickParams Params;
        public void Activate(bool logging)
        {
            var newTarget = Params.Target;

            // Cancel drag if needed
            if (InterfaceTargets.Dragged != null) {
                if (logging) {
                    Debug.Log("Drag cancelled: " + InterfaceTargets.Dragged);
                }
                InterfaceTargets.Dragged.CancelMouseDrag();
                InterfaceTargets.Dragged = null;
            }
            
            // Try unclicking current target
            if (InterfaceTargets.Selected != null && newTarget != InterfaceTargets.Selected) {
                var unclicked = InterfaceTargets.Selected.TryMouseUnclick(Params);
                if (!unclicked) {
                    if (logging) {
                        Debug.Log("Unclick blocked: " + InterfaceTargets.Selected);
                    }
                    return;
                }
            }

            if (logging) {
                Debug.Log("Click: " + newTarget);
            }
            //OnNewClick?.Invoke(pressedClickParams.Target);

            InterfaceTargets.Selected = newTarget;
            InterfaceTargets.Selected?.MouseClick(Params);
        }
    }
}