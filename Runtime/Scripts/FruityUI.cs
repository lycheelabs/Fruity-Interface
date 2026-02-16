using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Static access point for UI system state and utilities.
    /// </summary>
    public static class FruityUI {

        // ----------------------- Projection -----------------------

        /// <summary>The camera used for UI raycasting and coordinate conversion.</summary>
        public static Camera UICamera { get; private set; }
        
        /// <summary>The plane used for 3D world position calculations.</summary>
        public static Plane WorldPlane { get; private set; }

        // ----------------------- Mouse Position -----------------------
        
        /// <summary>Current mouse position in UI coordinates (accounting for letterboxing).</summary>
        public static Vector2 MouseScreenPosition => ((Vector2)Input.mousePosition * ScreenBounds.UIScaling) - ScreenBounds.LetterboxOffset;
        
        /// <summary>Current mouse position projected onto the world plane.</summary>
        public static Vector3 MouseWorldPosition => InterfaceHelpers.ScreenPointToWorldPoint(UICamera, Input.mousePosition, WorldPlane);
        
        /// <summary>Convert a screen position to world position on the world plane.</summary>
        public static Vector3 ScreenPointToWorldPoint(Vector2 screenPosition) => 
            InterfaceHelpers.ScreenPointToWorldPoint(UICamera, screenPosition, WorldPlane);
        
        /// <summary>Convert a world position to screen position.</summary>
        public static Vector3 WorldPointToScreenPoint(Vector3 worldPosition) => 
            InterfaceHelpers.WorldPointToScreenPoint(UICamera, worldPosition);
        
        /// <summary>Project a world position onto the world plane along the camera's view direction.</summary>
        public static Vector3 IntersectWithWorldPlane(this Vector3 worldPosition) =>
            InterfaceHelpers.IntersectWithPlane(UICamera, worldPosition, WorldPlane);
        
        /// <summary>True if the mouse cursor is within the screen bounds.</summary>
        public static bool MouseIsOnscreen =>
            Input.mousePosition.x >= 1 && 
            Input.mousePosition.y >= 1 && 
            Input.mousePosition.x < (Screen.width - 1) && 
            Input.mousePosition.y < (Screen.height - 1);

        // ----------------------- Mouse Targets -----------------------

        /// <summary>
        /// The target currently being highlighted (receiving MouseHovering calls).
        /// During a drag, this is the dragged target, not what's under the mouse.
        /// </summary>
        public static MouseTarget HighlightedTarget { get; internal set; }
 
        /// <summary>
        /// The target that was last clicked (received MouseClick).
        /// </summary>
        public static ClickTarget SelectedTarget { get; internal set; }
        
        /// <summary>
        /// The target currently being dragged (receiving MouseDragging calls).
        /// Null when no drag is active.
        /// </summary>
        public static DragTarget DraggedTarget { get; internal set; }
        
        /// <summary>
        /// The target currently under the mouse cursor (raw raycast result).
        /// During a drag, this is what the dragged item is being dragged over.
        /// </summary>
        public static MouseTarget DraggedOverTarget { get; internal set; }

        /// <summary>
        /// The DragOverTarget currently under the mouse cursor during a drag.
        /// This is the filtered version of DraggedOverTarget, only set if it implements DragOverTarget.
        /// Null when no drag is active or when dragging over non-DragOverTarget objects.
        /// </summary>
        public static DragOverTarget DraggedOverDragTarget { get; internal set; }
        
        // ----------------------- Lock State -----------------------

        /// <summary>True if the UI is locked to a specific node hierarchy.</summary>
        public static bool InterfaceIsLocked => LockedNode != null;
        
        /// <summary>When set, only this node and its children can receive mouse events.</summary>
        public static InterfaceNode LockedNode { get; private set; }
        
        /// <summary>When true, all mouse input is disabled.</summary>
        public static bool DisableInput { get; set; }
        
        // ----------------------- Methods -----------------------

        public static void SetUICamera(Camera camera) {
            UICamera = camera;
        }

        public static void SetWorldPlane(Plane plane) {
            WorldPlane = plane;
        }

        public static void LockUI(InterfaceNode newLockedRoot) {
            LockedNode = newLockedRoot;
        }

        public static void UnlockUI() {
            LockedNode = null;
        }
        
        /// <summary>
        /// Programmatically trigger a click on a target.
        /// The click is buffered and processed as synthetic input in the next available frame.
        /// Supports both click and drag behaviors based on the target's implemented interfaces.
        /// If a press is currently active, it will be force-completed before the buffered click is processed.
        /// Targets that don't implement ClickTarget or DragTarget are silently ignored.
        /// </summary>
        public static void TriggerNewClick(MouseTarget target, MouseButton button) {
            FruityUIManager.TriggerNewClick(target, button);
        }

        /// <summary>
        /// Cancel the current drag operation if the specified target is being dragged.
        /// </summary>
        public static void CancelDrag(DragTarget target) {
            if (DraggedTarget == target) {
                DraggedTarget.CancelMouseDrag();
                DraggedTarget = null;
            }
        }

    }

}