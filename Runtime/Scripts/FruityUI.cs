using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public static class FruityUI {

        // Projection
        public static Camera UICamera { get; private set;}
        public static Plane WorldPlane { get; private set; }
        
        // Mouse position
        public static Vector2 MouseScreenPosition => ((Vector2)Input.mousePosition * InterfaceConfig.UIScaling) - InterfaceConfig.LetterboxOffset;
        public static Vector3 MouseWorldPosition => UIHelpers.ScreenPointToWorldPoint(UICamera, Input.mousePosition, WorldPlane);
        public static Vector3 ScreenPointToWorldPoint(Vector2 screenPosition) => 
            UIHelpers.ScreenPointToWorldPoint (UICamera, screenPosition, WorldPlane);
        public static Vector3 WorldPointToScreenPoint(Vector3 worldPosition) => 
            UIHelpers.WorldPointToScreenPoint (UICamera, worldPosition);
        public static Vector3 IntersectWithWorldPlane(this Vector3 worldPosition) =>
            UIHelpers.IntersectWithPlane(UICamera, worldPosition, WorldPlane);
        
        public static bool MouseIsOnscreen =>
            Input.mousePosition.x >= 1  && 
            Input.mousePosition.y >= 1 && 
            Input.mousePosition.x < (Screen.width - 1) && 
            Input.mousePosition.y < (Screen.height - 1);

        // Mouse targets
        public static MouseTarget HighlightedTarget { get; internal set; }
        public static ClickTarget SelectedTarget { get; internal set; }
        public static DragTarget DraggedTarget { get; internal set; }
        public static MouseTarget DraggedOverTarget { get; internal set; }
        public static DragTarget GrabbedTarget => GrabTarget.CurrentGrabbedInstance;
        public static MouseTarget GrabbedOverTarget => 
            (GrabTarget.CurrentGrabbedInstance != null ? HighlightedTarget : null);

        // Lock state
        public static bool InterfaceIsLocked => LockedNode != null;
        public static InterfaceNode LockedNode { get; private set; }
        public static bool DisableInput { get; set; }
        
        // -----------------------------------------------------------

        public static void SetUICamera(Camera camera) {
            UICamera = camera;
        }

        public static void SetWorldPlane(Plane plane) {
            WorldPlane = plane;
        }

        public static void LockUI (InterfaceNode newLockedRoot) {
            LockedNode = newLockedRoot;
        }

        public static void UnlockUI () {
            LockedNode = null;
        }
        
        public static void TriggerNewClick (ClickTarget target, MouseButton button) {
            FruityUIManager.TriggerNewClick(target, button);
        }

    }

}