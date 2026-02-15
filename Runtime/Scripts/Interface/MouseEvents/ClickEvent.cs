using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired when a click is detected.
    /// Handles unclick of previous target and calls MouseClick on the new target.
    /// </summary>
    public class ClickEvent : InterfaceEvent {
        public ClickParams Params;
        
        public void Activate(bool logging) {
            var newTarget = Params.Target;

            // Cancel any active drag
            if (FruityUI.DraggedTarget != null) {
                if (logging) Debug.Log("Drag cancelled by click: " + FruityUI.DraggedTarget);
                FruityUI.DraggedTarget.CancelMouseDrag();
                FruityUI.DraggedTarget = null;
            }
            
            // Try to unclick the currently selected target
            if (FruityUI.SelectedTarget != null && newTarget != FruityUI.SelectedTarget) {
                if (!FruityUI.SelectedTarget.TryMouseUnclick(Params)) {
                    if (logging) Debug.Log("Unclick blocked by: " + FruityUI.SelectedTarget);
                    return;
                }
            }

            if (logging) Debug.Log("Click: " + newTarget);

            FruityUI.SelectedTarget = newTarget;
            FruityUI.SelectedTarget?.MouseClick(Params);
        }
    }
}