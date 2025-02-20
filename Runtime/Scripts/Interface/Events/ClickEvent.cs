using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class ClickEvent : InterfaceEvent  {
        public ClickParams Params;
        public void Activate(bool logging)
        {
            var newTarget = Params.Target;

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