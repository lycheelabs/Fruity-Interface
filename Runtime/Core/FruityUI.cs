using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Static access point for UI system state and utilities.
    /// </summary>
    public static class FruityUI {

        private const bool DEBUG_LAYER_LOCK = false;

        // ----------------------- Screen Bounds -----------------------

        private static ScreenBounds _screenBounds;
        private static AspectRatio _minAspect;
        private static AspectRatio _maxAspect;

        public static ScreenBounds ScreenBounds => _screenBounds;

        public static void SetAspect(AspectRatio min, AspectRatio max) {
            _minAspect = min;
            _maxAspect = max;
            _screenBounds = new ScreenBounds();
            _screenBounds.Update(_minAspect, _maxAspect);
        }

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
            Input.mousePosition.x >= 0 && 
            Input.mousePosition.y >= 0 && 
            Input.mousePosition.x < Screen.width && 
            Input.mousePosition.y < Screen.height;

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
        public static DraggedOverTarget DraggedOverTarget { get; internal set; }

        // ----------------------- Layer Lock State -----------------------

        private static readonly Dictionary<int, int> _layerLockCounts = new Dictionary<int, int>();
        private static int _activeLayerThreshold;

        /// <summary>The highest locked layer, or 0 if no layers are locked.
        /// Only nodes at this layer or above receive input.</summary>
        public static int ActiveLayerThreshold => _activeLayerThreshold;

        /// <summary>True if any layer is currently locked.</summary>
        public static bool InterfaceIsLocked => _activeLayerThreshold > 0;

        /// <summary>When true, all mouse input is disabled.
        /// (However - for safety, when InterfaceIsLocked the locked layer is never disabled)</summary>
        public static bool DisableInput { get; set; }

        /// <summary>Lock the given layer. Nodes below this layer will not receive input.
        /// Multiple locks on the same layer are refcounted.</summary>
        public static void LockLayer(int layer) {
            _layerLockCounts.TryGetValue(layer, out var count);
            _layerLockCounts[layer] = count + 1;
            var prevThreshold = _activeLayerThreshold;
            if (layer > _activeLayerThreshold) {
                _activeLayerThreshold = layer;
            }
            if (DEBUG_LAYER_LOCK) {
                Debug.Log($"[FruityUI] LockLayer({layer}) count={count + 1} threshold={_activeLayerThreshold}" +
                    (prevThreshold != _activeLayerThreshold ? " (raised from " + prevThreshold + ")" : ""));
            }
        }

        /// <summary>Unlock the given layer. If no locks remain on any layer, all nodes are active again.</summary>
        public static void UnlockLayer(int layer) {
            if (!_layerLockCounts.TryGetValue(layer, out var count)) {
                if (DEBUG_LAYER_LOCK) Debug.LogWarning($"[FruityUI] UnlockLayer({layer}) called but layer is not locked.");
                return;
            }
            var prevThreshold = _activeLayerThreshold;
            if (count <= 1) {
                _layerLockCounts.Remove(layer);
                _activeLayerThreshold = _layerLockCounts.Count > 0 ? _layerLockCounts.Keys.Max() : 0;
            } else {
                _layerLockCounts[layer] = count - 1;
            }
            var newCount = _layerLockCounts.TryGetValue(layer, out var nc) ? nc : 0;
            if (DEBUG_LAYER_LOCK) {
                Debug.Log($"[FruityUI] UnlockLayer({layer}) count={newCount} threshold={_activeLayerThreshold}" +
                    (prevThreshold != _activeLayerThreshold ? " (dropped from " + prevThreshold + ")" : ""));
            }
        }
        
        // ----------------------- Methods -----------------------

        public static void SetUICamera(Camera camera) {
            UICamera = camera;
        }

        public static void SetWorldPlane(Plane plane) {
            WorldPlane = plane;
        }

        /// <summary>
        /// Updates screen bounds to match current window/display dimensions.
        /// Called automatically by FruityUIManager each frame.
        /// </summary>
        public static void Update() {
            if (_screenBounds != null) {
                _screenBounds.Update(_minAspect, _maxAspect);
            }
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
        /// This queues a cancellation event and updates internal state to ensure proper event ordering.
        /// Safe to call at any time, even during event processing or from within drag callbacks.
        /// </summary>
        public static void CancelDrag(DragTarget target) {
            FruityUIManager.CancelDrag(target);
        }

    }

}
