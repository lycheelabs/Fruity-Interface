using UnityEngine;

namespace LycheeLabs.InputTree {

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
            if (IsAlive(Interface.DraggedTarget)) {
                if (logging) Debug.Log("Drag cancelled by click: " + Interface.DraggedTarget);
                Interface.DraggedTarget.CancelMouseDrag();
                Interface.DraggedTarget = null;
            } else {
                Interface.DraggedTarget = null;
            }
            
            // Try to unclick the currently selected target
            if (!IsAlive(Interface.SelectedTarget)) {
                Interface.SelectedTarget = null;
            } else if (Target != Interface.SelectedTarget) {
                if (!Interface.SelectedTarget.TryMouseUnclick(Params)) {
                    if (logging) Debug.Log("Unclick blocked by: " + Interface.SelectedTarget);
                    return;
                }
            }

            if (logging) Debug.Log("Click: " + Target);

            Interface.SelectedTarget = Target;
            Interface.SelectedTarget?.ApplyMouseClick(Params);
        }

    }

}
