using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Scripting.LifecycleManagement;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Static access point for UI system state and utilities.
    /// </summary>
    public static partial class FruityUI {

        private static readonly bool DEBUG_LAYER_LOCK = false;

        // ----------------------- Screen Bounds -----------------------

        public const float DefaultMinAspect = 9f / 21f;
        public const float DefaultMaxAspect = 21f / 9f;

        [AutoStaticsCleanup]
        private static ScreenBounds _screenBounds = CreateDefaultScreenBounds();
        [AutoStaticsCleanup]
        private static float _minAspect = DefaultMinAspect;
        [AutoStaticsCleanup]
        private static float _maxAspect = DefaultMaxAspect;

        private static ScreenBounds CreateDefaultScreenBounds() {
            var bounds = new ScreenBounds();
            bounds.Update(DefaultMinAspect, DefaultMaxAspect);
            return bounds;
        }

        public static ScreenBounds ScreenBounds => _screenBounds;

        public static void SetAspect(float min, float max) {
            if (!float.IsFinite(min) || !float.IsFinite(max) || min <= 0 || max <= 0 || min > max) {
                Debug.LogError($"Invalid FruityUI aspect range: {min} to {max}.");
                return;
            }

            _minAspect = min;
            _maxAspect = max;
            _screenBounds.Update(_minAspect, _maxAspect);
        }

        // ----------------------- Projection -----------------------

        /// <summary>The camera used for UI raycasting and coordinate conversion.</summary>
        [AutoStaticsCleanup]
        private static Camera uiCamera;
        public static Camera UICamera => uiCamera;
        
        /// <summary>The plane used for 3D world position calculations.</summary>
        [AutoStaticsCleanup]
        private static Plane worldPlane;
        public static Plane WorldPlane => worldPlane;

        // ----------------------- Mouse Position -----------------------

        [AutoStaticsCleanup]
        private static Vector2 _rawMouseScreenPosition;

        internal static Vector2 RawMouseScreenPosition => _rawMouseScreenPosition;

        internal static void SetRawMouseScreenPosition(Vector2 screenPosition) {
            _rawMouseScreenPosition = screenPosition;
        }
        
        /// <summary>Current mouse position in UI coordinates (accounting for letterboxing).</summary>
        public static Vector2 MouseScreenPosition => (_rawMouseScreenPosition * ScreenBounds.UIScaling) - ScreenBounds.LetterboxOffset;
        
        /// <summary>Current mouse position projected onto the world plane.</summary>
        public static Vector3 MouseWorldPosition => InterfaceHelpers.ScreenPointToWorldPoint(UICamera, _rawMouseScreenPosition, WorldPlane);
        
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
            _rawMouseScreenPosition.x >= 0 &&
            _rawMouseScreenPosition.y >= 0 &&
            _rawMouseScreenPosition.x < Screen.width &&
            _rawMouseScreenPosition.y < Screen.height;

        // ----------------------- Mouse Targets -----------------------

        /// <summary>
        /// The target currently being highlighted (receiving MouseHovering calls).
        /// During a drag, this is the dragged target, not what's under the mouse.
        /// </summary>
        [AutoStaticsCleanup]
        private static MouseTarget highlightedTarget;
        public static MouseTarget HighlightedTarget {
            get => highlightedTarget;
            internal set => highlightedTarget = value;
        }
 
        /// <summary>
        /// The target that was last clicked (received MouseClick).
        /// </summary>
        [AutoStaticsCleanup]
        private static ClickTarget selectedTarget;
        public static ClickTarget SelectedTarget {
            get => selectedTarget;
            internal set => selectedTarget = value;
        }
        
        /// <summary>
        /// The target currently being dragged (receiving MouseDragging calls).
        /// Null when no drag is active.
        /// </summary>
        [AutoStaticsCleanup]
        private static DragTarget draggedTarget;
        public static DragTarget DraggedTarget {
            get => draggedTarget;
            internal set => draggedTarget = value;
        }
        
        /// <summary>
        /// The target currently under the mouse cursor (raw raycast result).
        /// During a drag, this is what the dragged item is being dragged over.
        /// </summary>
        [AutoStaticsCleanup]
        private static DraggedOverTarget draggedOverTarget;
        public static DraggedOverTarget DraggedOverTarget {
            get => draggedOverTarget;
            internal set => draggedOverTarget = value;
        }

        // ----------------------- Layer Lock State -----------------------

        [AutoStaticsCleanup]
        private static readonly Dictionary<int, int> _layerLockCounts = new Dictionary<int, int>();
        [AutoStaticsCleanup]
        private static int _activeLayerThreshold;

        /// <summary>The highest locked layer, or 0 if no layers are locked.
        /// Only nodes at this layer or above receive input.</summary>
        public static int ActiveLayerThreshold => _activeLayerThreshold;

        /// <summary>True if any layer is currently locked.</summary>
        public static bool InterfaceIsLocked => _activeLayerThreshold > 0;

        /// <summary>When true, all mouse input is disabled.
        /// (However - for safety, when InterfaceIsLocked the locked layer is never disabled)</summary>
        [AutoStaticsCleanup]
        private static bool disableInput;
        public static bool DisableInput {
            get => disableInput;
            set => disableInput = value;
        }

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
            uiCamera = camera;
        }

        public static void SetWorldPlane(Plane plane) {
            worldPlane = plane;
        }

        /// <summary>
        /// Updates screen bounds to match current window/display dimensions.
        /// Called automatically by FruityInterfaceInputModule each frame.
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
            FruityInputRuntime.TriggerNewClick(target, button);
        }

        /// <summary>
        /// Cancel the current drag operation if the specified target is being dragged.
        /// This queues a cancellation event and updates internal state to ensure proper event ordering.
        /// Safe to call at any time, even during event processing or from within drag callbacks.
        /// </summary>
        public static void CancelDrag(DragTarget target) {
            FruityInputRuntime.CancelDrag(target);
        }

    }

}
