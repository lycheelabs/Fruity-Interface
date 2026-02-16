using UnityEngine;
using System.Collections.Generic;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Manages mouse input state, raycasting, and event generation for the UI system.
    /// </summary>
    public class MouseState {
    
        private const float PRESS_DEBOUNCE_TIME = 0.05f;
        private const int MAX_QUEUED_EVENTS = 10;

        /// <summary> Mouse button press event to be processed. </summary>
        private struct PressEvent {
            public MouseTarget target;
            public MouseButton button;
            public Vector3 worldPosition;
        }

        // ---------------------------------------------------
        
        public static bool MouseIsMoving { get; private set; }
        public static bool DisableMouse { get; set; }
        public static event TargetDelegate OnNewPress;
        public delegate void TargetDelegate(ClickTarget newTarget);
    
        private readonly MouseRaycaster raycaster;
        private readonly Queue<PressEvent> pressEventQueue;

        private MouseButton activeButton;
        private MousePress activePress;
        private float lastPressTime;
        private int lastReleaseFrame = -1;
        private Vector3 oldMousePosition;

        public MouseState() {
            raycaster = new MouseRaycaster();
            pressEventQueue = new Queue<PressEvent>();
            activeButton = MouseButton.None;
            lastReleaseFrame = -1;
        }

        /// <summary>
        /// Queue a synthetic press that will be processed as if the user clicked.
        /// Supports both clicks and drags, based on the MouseTarget's behaviour.
        /// (If the target doesn't implement ClickTarget or DragTarget, it is silently ignored.)
        /// </summary>
        public void QueueClick(MouseTarget target, MouseButton button) {
            if (target == null || pressEventQueue.Count > MAX_QUEUED_EVENTS) return;

            // Only queue if target implements at least one of the required interfaces
            if (target is ClickTarget || target is DragTarget) {
                pressEventQueue.Enqueue(new PressEvent {
                    target = target,
                    button = button,
                    worldPosition = FruityUI.MouseWorldPosition
                });
            }
        }

        public void Update() {
            if (!Application.isFocused || DisableMouse) return;

#if UNITY_EDITOR
            ValidateState();
#endif
            // Update input state and detect new mouse presses
            UpdateInput();
            
            // Process any queued synthetic or real mouse presses
            ProcessQueuedEvents();
            
            // Update the ongoing mouse press (if any)
            UpdateActivePress();
        }

        /// <summary>
        /// Update input state: active button, raycasting, hovering, and detect new real mouse presses.
        /// </summary>
        private void UpdateInput() {
            UpdateActiveButton();
            UpdateRaycasting();
            
            // Only check for new input when idle
            if (pressEventQueue.Count == 0 && !activePress.isPressed) {
                CheckForNewPress();
            }
        }

        /// <summary>
        /// Update which mouse button is currently active.
        /// </summary>
        private void UpdateActiveButton() {
            if (Input.GetMouseButtonDown((int)MouseButton.Left)) {
                activeButton = MouseButton.Left;
            } else if (Input.GetMouseButtonDown((int)MouseButton.Right)) {
                activeButton = MouseButton.Right;
            } else if (!AnyMouseButtonHeld()) {
                activeButton = MouseButton.None;
            }
        }

        /// <summary>
        /// Check if any mouse button is currently held down.
        /// </summary>
        private bool AnyMouseButtonHeld() {
            return Input.GetMouseButton((int)MouseButton.Left) || 
                   Input.GetMouseButton((int)MouseButton.Right);
        }

        /// <summary>
        /// Perform raycasting and update hover state.
        /// Sets FruityUI.DraggedOverTarget to the current raycast result and queues a hover event.
        /// </summary>
        private void UpdateRaycasting() {
            var raycastTarget = GetRaycastTarget(out var raycastNode, out var raycastWorldPos);
            FruityUI.DraggedOverTarget = raycastTarget;

            var hoverTarget = FruityUI.DraggedTarget ?? raycastTarget;
            
            // Only pass button if this target is the one being pressed (dragged or clicked)
            var pressButton = (hoverTarget == FruityUI.DraggedTarget || hoverTarget == activePress.target) 
                ? activePress.button 
                : MouseButton.None;
            
            QueueHoverEvent(hoverTarget, new HoverParams(raycastNode, raycastWorldPos, pressButton));
        }

        /// <summary>
        /// Process the next queued press event.
        /// If a press is active, force-completes it before processing the queued event.
        /// </summary>
        private void ProcessQueuedEvents() {
            if (pressEventQueue.Count == 0) return;

            // Force-complete current press before processing queued event
            if (activePress.isPressed) {
                ForceCompleteCurrentPress();
            }

            var pressEvent = pressEventQueue.Dequeue();
            ProcessPress(pressEvent);
        }

        /// <summary>
        /// Update the active mouse press (if one exists).
        /// </summary>
        private void UpdateActivePress() {
            if (!activePress.isPressed) return;

            var pressTarget = FruityUI.DraggedTarget ?? FruityUI.DraggedOverTarget;
            UpdateMousePress(pressTarget);
        }

        /// <summary>
        /// Check for a new mouse button press and queue it for processing.
        /// </summary>
        private void CheckForNewPress() {
            // activeButton is already set by UpdateActiveButton() when GetMouseButtonDown fires
            if (activeButton == MouseButton.None) return;
            
            // Frame-based debounce: prevent queueing new press on same frame a press was just cleared
            // (Fixes double-pickup when pickup completion and new press detection happen in same frame)
            if (Time.frameCount == lastReleaseFrame) return;
            
            // Time-based debounce: prevent rapid re-clicks from faulty hardware
            if (Time.unscaledTime <= lastPressTime + PRESS_DEBOUNCE_TIME) return;

            var raycastTarget = FruityUI.DraggedOverTarget;
            pressEventQueue.Enqueue(new PressEvent {
                target = raycastTarget,
                button = activeButton,
                worldPosition = FruityUI.MouseWorldPosition
            });
            
            // Update debounce timestamp immediately to prevent same-frame double-press detection
            // (e.g., pickup completion click being detected as both completion AND new press)
            lastPressTime = Time.unscaledTime;
        }

        /// <summary>
        /// Process a press event: start a click and/or drag based on the target's configuration.
        /// </summary>
        private void ProcessPress(PressEvent pressEvent) {
            var clickTarget = pressEvent.target as ClickTarget;
            var dragTarget = pressEvent.target as DragTarget;
            var dragMode = dragTarget?.GetDragMode(pressEvent.button) ?? DragTarget.DragMode.Disabled;

            var pressRemainsActive = false;

            // Start click if allowed
            if (clickTarget != null && ShouldProcessClick(dragMode)) {
                pressRemainsActive |= TryStartClick(clickTarget, pressEvent);
            }

            // Start drag if enabled
            if (dragTarget != null && dragMode != DragTarget.DragMode.Disabled) {
                pressRemainsActive |= TryStartDrag(dragTarget, pressEvent, dragMode);
            }

            if (!pressRemainsActive) {
                activePress.Clear();
                lastReleaseFrame = Time.frameCount;
            }
        }

        /// <summary>
        /// Determine if a click should be processed based on the drag mode.
        /// Click events are only sent when drag mode is Disabled or DragOnly
        /// (PickUpOnly and DragOrPickUp use clicking to start/complete pickup, not to trigger click events).
        /// </summary>
        private bool ShouldProcessClick(DragTarget.DragMode dragMode) {
            return dragMode == DragTarget.DragMode.Disabled || 
                   dragMode == DragTarget.DragMode.DragOnly;
        }

        /// <summary>
        /// Attempt to start a click press.
        /// Returns true if the press was started and remains active, false if it was completed immediately.
        /// </summary>
        private bool TryStartClick(ClickTarget clickTarget, PressEvent pressEvent) {
            activePress.StartClick(clickTarget, pressEvent.button);
            OnNewPress?.Invoke(clickTarget);

            // Handle immediate click (ClickOnMouseDown)
            if (clickTarget.ClickOnMouseDown) {
                QueueClickEvent(new ClickParams(clickTarget, pressEvent.worldPosition, pressEvent.button));
                activePress.ReleaseClick();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Attempt to start a drag press.
        /// Returns true if the drag was started successfully.
        /// </summary>
        private bool TryStartDrag(DragTarget dragTarget, PressEvent pressEvent, DragTarget.DragMode dragMode) {
            var screenPosition = (Vector2)Input.mousePosition;
            activePress.StartDrag(dragTarget, pressEvent.button, dragMode, pressEvent.worldPosition, screenPosition);

            // Start with null DragOverTarget - will be updated on first drag update
            var dragParams = new DragParams(dragTarget, null, screenPosition, screenPosition, pressEvent.button);
            QueueDragStartEvent(dragParams);
            return true;
        }

        /// <summary>
        /// Validate state invariants (editor only).
        /// Detects and corrects desyncs between FruityUI drag state and active press state.
        /// </summary>
        private void ValidateState() {
            // Detect desync between FruityUI.DraggedTarget and press state
            if (FruityUI.DraggedTarget != null && !activePress.pressIsDrag) {
                Debug.LogWarning("[MouseState] State desync: DraggedTarget set but no press drag active");
                FruityUI.DraggedTarget = null;
            }

            if (activePress.pressIsDrag && FruityUI.DraggedTarget == null) {
                Debug.LogWarning("[MouseState] State desync: Press drag active but no DraggedTarget");
                CancelDrag();
            }

            // Prevent queue overflow (abuse protection)
            if (pressEventQueue.Count > MAX_QUEUED_EVENTS) {
                Debug.LogError("[MouseState] Press event queue overflow - clearing to prevent abuse");
                pressEventQueue.Clear();
            }
        }

        #region Raycasting

        /// <summary>
        /// Perform raycasting to find the target under the mouse.
        /// Also updates MouseIsMoving based on mouse position changes.
        /// </summary>
        private MouseTarget GetRaycastTarget(out InterfaceNode node, out Vector3 worldPosition) {
            // Track mouse movement
            var newMousePosition = Input.mousePosition;
            MouseIsMoving = (newMousePosition != oldMousePosition);
            oldMousePosition = newMousePosition;

            if (!FruityUI.MouseIsOnscreen) {
                node = null;
                worldPosition = Vector3.zero;
                return null;
            }

            raycaster.CollideAndResolve(GetRelevantButton(), out var target, out node, out worldPosition);
            return target;
        }

        #endregion

        #region Press Lifecycle

        /// <summary>
        /// Get the mouse button that should be used for raycasting and event generation.
        /// Returns the active press button if a press is active, otherwise the current active button.
        /// </summary>
        private MouseButton GetRelevantButton() {
            return activePress.isPressed ? activePress.button : activeButton;
        }

        /// <summary>
        /// Force-complete the current press (both click and drag components).
        /// Used when a queued press needs to interrupt an ongoing press.
        /// </summary>
        private void ForceCompleteCurrentPress() {
            if (!activePress.isPressed) return;

            // Complete drag if active
            if (activePress.pressIsDrag && FruityUI.DraggedTarget != null) {
                QueueDragCompleteEvent(BuildCurrentDragParams());
            }

            // Complete click if active
            if (activePress.pressIsClick && activePress.target is ClickTarget clickTarget) {
                var clickParams = new ClickParams(clickTarget, activePress.pressWorldPosition, activePress.button);
                clickParams.HeldDuration = Time.unscaledTime - lastPressTime;
                QueueClickEvent(clickParams);
                activePress.ReleaseClick();
            }

            activePress.Clear();
            lastReleaseFrame = Time.frameCount;
        }

        /// <summary>
        /// Build drag parameters for the current active drag.
        /// Returns DragParams.Null if no valid drag is active.
        /// </summary>
        private DragParams BuildCurrentDragParams() {
            if (!activePress.pressIsDrag || FruityUI.DraggedTarget == null) {
                return DragParams.Null;
            }

            return new DragParams(
                FruityUI.DraggedTarget,
                FruityUI.DraggedOverDragTarget,
                activePress.pressScreenPosition,
                (Vector2)Input.mousePosition,
                activePress.button
            );
        }

        /// <summary>
        /// Update the ongoing mouse press based on current input state.
        /// Handles completion, cancellation, and continuation of both clicks and drags.
        /// </summary>
        private void UpdateMousePress(MouseTarget pressTarget) {
            var clickTarget = pressTarget as ClickTarget;
            
            // Pickup mode: complete on second click of same button
            if (activePress.pressIsDrag && activePress.isPickUpDrag) {
                if (Input.GetMouseButtonDown((int)activePress.button) && activePress.pressStartFrame != Time.frameCount) {
                    // Build params after hierarchy has processed to get current DraggedOverDragTarget
                    var dragParams = BuildCurrentDragParams();
                    
                    // Cancel drag if no valid drop target, otherwise complete
                    if (dragParams.DraggingOver == null) {
                        QueueDragCancelEvent();
                    } else {
                        QueueDragCompleteEvent(dragParams);
                    }
                    ClearDragOverState();
                    activePress.Clear();
                    lastReleaseFrame = Time.frameCount;
                    return;
                }
            }

            // Normal mode: handle mouse button release
            var buttonWasReleased = !activePress.isPickUpDrag && !Input.GetMouseButton((int)activePress.button);
            if (buttonWasReleased) {
                // Complete or convert drag
                if (activePress.pressIsDrag) {
                    // For DragOrPickUp: short click converts to pickup mode
                    var screenPosition = (Vector2)Input.mousePosition;
                    if (activePress.dragMode == DragTarget.DragMode.DragOrPickUp && !activePress.WasRealDrag(screenPosition)) {
                        activePress.ConvertToPickUp();
                        return;
                    }
                    
                    // Build params after hierarchy has processed
                    var dragParams = BuildCurrentDragParams();
                    
                    // Cancel drag if no valid drop target, otherwise complete
                    if (dragParams.DraggingOver == null) {
                        QueueDragCancelEvent();
                    } else {
                        QueueDragCompleteEvent(dragParams);
                    }
                    ClearDragOverState();
                }

                // Complete click
                if (activePress.pressIsClick && activePress.target == clickTarget) {
                    var clickParams = new ClickParams(clickTarget, activePress.pressWorldPosition, activePress.button);
                    clickParams.HeldDuration = Time.unscaledTime - lastPressTime;
                    QueueClickEvent(clickParams);
                    activePress.ReleaseClick();
                }

                activePress.Clear();
                lastReleaseFrame = Time.frameCount;
                return;
            }

            // Continue drag
            if (activePress.pressIsDrag && FruityUI.DraggedTarget != null) {
                // Check if drag mode is still enabled
                var currentMode = FruityUI.DraggedTarget.GetDragMode(activePress.button);
                if (currentMode == DragTarget.DragMode.Disabled) {
                    QueueDragCancelEvent();
                    ClearDragOverState();
                    CancelDrag();
                    return;
                }

                // Check for cancel via opposite button
                var cancelButton = (activePress.button == MouseButton.Left) ? MouseButton.Right : MouseButton.Left;
                if (Input.GetMouseButtonDown((int)cancelButton)) {
                    QueueDragCancelEvent();
                    ClearDragOverState();
                    CancelDrag();
                    return;
                }

                // Queue drag-over update FIRST (updates FruityUI.DraggedOverDragTarget)
                // Then build params with the updated value for drag update event
                QueueDragOverUpdateEvent();
                
                var dragParams = BuildCurrentDragParams();
                QueueDragUpdateEvent(dragParams);
            }
        }

        /// <summary>
        /// Cancel the active drag without completing it.
        /// </summary>
        private void CancelDrag() {
            activePress.pressIsDrag = false;
            activePress.isPickUpDrag = false;
        }

        /// <summary>
        /// Clear drag-over state when drag ends.
        /// </summary>
        private static void ClearDragOverState() {
            DragOverHierarchyEvent.ClearState();
        }

        #endregion

        #region Event Queuing

        private static void QueueHoverEvent(MouseTarget target, HoverParams hoverParams) {
            FruityUIManager.Queue(new HoverHierarchyEvent { 
                Target = target,
                Params = hoverParams 
            });
        }

        private static void QueueClickEvent(ClickParams clickParams) {
            FruityUIManager.Queue(new ClickEvent { Params = clickParams });
        }

        private static void QueueDragStartEvent(DragParams dragParams) {
            FruityUIManager.Queue(new StartDragEvent { Params = dragParams });
        }

        private static void QueueDragUpdateEvent(DragParams dragParams) {
            FruityUIManager.Queue(new UpdateDragEvent { Params = dragParams });
        }

        private static void QueueDragOverUpdateEvent() {
            // Queue with minimal params - the event will read FruityUI.DraggedOverTarget directly
            FruityUIManager.Queue(new DragOverHierarchyEvent { 
                Params = new DragParams(
                    FruityUI.DraggedTarget,
                    null, // Will be populated by the hierarchy
                    Vector2.zero,
                    (Vector2)Input.mousePosition,
                    MouseButton.None
                )
            });
        }

        private static void QueueDragCompleteEvent(DragParams dragParams) {
            FruityUIManager.Queue(new EndDragEvent { Params = dragParams, WasCancelled = false });
        }

        private static void QueueDragCancelEvent() {
            FruityUIManager.Queue(new EndDragEvent { Params = default, WasCancelled = true });
        }

        #endregion

    }

}
