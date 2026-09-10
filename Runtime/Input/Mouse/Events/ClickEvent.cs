using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired when a click is detected.
    /// Handles unclick of previous target and calls MouseClick on the new target.
    /// </summary>
    public class ClickEvent : ControlEvent {

        public ClickTarget Target;
        public ClickParams Params;

        private static bool IsAlive(object target) {
            if (target == null) return false;
            return !(target is UnityEngine.Object unityObject) || unityObject != null;
        }
        
        public void Activate(bool logging) {
            if (!IsAlive(Target)) return;

            // Cancel any active drag
            if (IsAlive(FruityUI.DraggedTarget)) {
                if (logging) Debug.Log("Drag cancelled by click: " + FruityUI.DraggedTarget);
                FruityUI.DraggedTarget.CancelMouseDrag();
                FruityUI.DraggedTarget = null;
            } else {
                FruityUI.DraggedTarget = null;
            }
            
            // Try to unclick the currently selected target
            if (!IsAlive(FruityUI.SelectedTarget)) {
                FruityUI.SelectedTarget = null;
            } else if (Target != FruityUI.SelectedTarget) {
                if (!FruityUI.SelectedTarget.TryMouseUnclick(Params)) {
                    if (logging) Debug.Log("Unclick blocked by: " + FruityUI.SelectedTarget);
                    return;
                }
            }

            if (logging) Debug.Log("Click: " + Target);

            FruityUI.SelectedTarget = Target;
            FruityUI.SelectedTarget?.ApplyMouseClick(Params);
        }

    }

}
