using UnityEngine;
using System.Collections.Generic;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Manages mouse input state, raycasting, and event generation for the UI system.
    /// </summary>
    public partial class MouseState {
    
        private const float PRESS_DEBOUNCE_TIME = 0.05f;
        private const int MAX_QUEUED_EVENTS = 10;

        /// <summary> Mouse button press event to be processed. </summary>
        private struct PressEvent {
            public MouseTarget target;
            public MouseButton button;
            public Vector3 worldPosition;
        }

        // ---------------------------------------------------
        
        private static bool mouseIsMoving;
        public static bool MouseIsMoving => mouseIsMoving;
        private static bool disableMouse;
        public static bool DisableMouse {
            get => disableMouse;
            set => disableMouse = value;
        }
        public bool LogRaycasts { get; set; }
        public static event TargetDelegate OnNewPress;
        public delegate void TargetDelegate(ClickTarget newTarget);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad() {
            mouseIsMoving = false;
            disableMouse = false;
            OnNewPress = null;
        }
    
        private readonly MouseRaycaster raycaster;
        private readonly Queue<PressEvent> pressEventQueue;

        private MouseButton activeButton;
        private bool buttonDownThisEvent;
        private MouseButton buttonDownEventButton;
        private bool leftButtonHeld;
        private bool rightButtonHeld;
        private bool middleButtonHeld;
        private MousePress activePress;
        private float lastPressTime;
        private MouseTarget lastRaycastTarget;
        private InterfaceNode lastRaycastNode;
        private Vector3 lastRaycastWorldPos;
        private Vector2 oldMousePosition;
        private readonly Queue<RawInputEvent> inputEvents;

        public MouseState() {
            raycaster = new MouseRaycaster();
            pressEventQueue = new Queue<PressEvent>();
            inputEvents = new Queue<RawInputEvent>();
            activeButton = MouseButton.None;
        }

        internal void QueueInputEvent(RawInputEvent inputEvent) {
            inputEvents.Enqueue(inputEvent);
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

        /// <summary>
        /// Cancel the active drag if it matches the specified target.
        /// FruityUI.DraggedTarget is cleared immediately to keep state aligned within the same frame.
        /// A cancellation event is then queued to invoke CancelMouseDrag() on the target once
        /// the event queue processes, even though DraggedTarget is already null by that point.
        /// Safe to call at any time - even during event processing.
        /// </summary>
        public void CancelDrag(DragTarget target) {
            if (activePress.pressIsDrag && FruityUI.DraggedTarget == target) {
                // Null immediately so ValidateState() next frame sees a consistent state.
                // The target reference is passed to the event so CancelMouseDrag() is still called.
                FruityUI.DraggedTarget = null;

                // Queue the cancellation event with the captured target
                QueueDragCancelEvent(target);

                // Clear drag-over state
                ClearDragOverState();

                // Update local press state to prevent further drag updates
                CancelDragPress();
            }
        }

        public void Update() {
            //if (!Application.isFocused || DisableMouse) return;

#if UNITY_EDITOR
            ValidateState();
#endif
            while (inputEvents.Count > 0) {
                ProcessInputEvent(inputEvents.Dequeue());
            }

            // Keep hover state alive on frames without input transitions.
            buttonDownThisEvent = false;
            UpdateRaycasting();
            ProcessQueuedPress();
            UpdateActivePress();
        }

        private void ProcessInputEvent(RawInputEvent inputEvent) {
            FruityUI.SetRawMouseScreenPosition(inputEvent.ScreenPosition);
            buttonDownThisEvent = false;

            switch (inputEvent.Type) {
                case RawInputEventType.Move:
                    break;
                case RawInputEventType.ButtonDown:
                    activeButton = inputEvent.Button;
                    buttonDownThisEvent = true;
                    buttonDownEventButton = inputEvent.Button;
                    SetButtonHeld(inputEvent.Button, true);
                    break;
                case RawInputEventType.ButtonUp:
                    SetButtonHeld(inputEvent.Button, false);
                    if (!AnyMouseButtonHeld()) {
                        activeButton = MouseButton.None;
                    }
                    break;
                case RawInputEventType.Cancel:
                    CancelActiveInput();
                    break;
                case RawInputEventType.Scroll:
                    break;
            }

            UpdateRaycasting();
            if (buttonDownThisEvent) {
                CheckForNewPress();
            }
            ProcessQueuedPress();
            UpdateActivePress();
        }

        private void CancelActiveInput() {
            if (activePress.pressIsDrag) {
                QueueDragCancelEvent();
                ClearDragOverState();
            }
            activePress.Clear();
            activeButton = MouseButton.None;
            leftButtonHeld = false;
            rightButtonHeld = false;
            middleButtonHeld = false;
            buttonDownThisEvent = false;
            buttonDownEventButton = MouseButton.None;
        }

        private bool AnyMouseButtonHeld() {
            return IsButtonHeld(MouseButton.Left) ||
                   IsButtonHeld(MouseButton.Right) ||
                   IsButtonHeld(MouseButton.Middle);
        }

        private bool IsButtonHeld(MouseButton button) {
            return button == MouseButton.Left ? leftButtonHeld :
                button == MouseButton.Right ? rightButtonHeld :
                button == MouseButton.Middle && middleButtonHeld;
        }

        private void SetButtonHeld(MouseButton button, bool held) {
            if (button == MouseButton.Left) leftButtonHeld = held;
            if (button == MouseButton.Right) rightButtonHeld = held;
            if (button == MouseButton.Middle) middleButtonHeld = held;
        }

        /// <summary>
        /// Perform raycasting and queue hover event.
        /// </summary>
        private void UpdateRaycasting() {
            var previousTarget = lastRaycastTarget;
            GetRaycastTarget();

            if (LogRaycasts && previousTarget != lastRaycastTarget) {
                var targetName = lastRaycastTarget == null ? "<none>" : lastRaycastTarget.ToString();
                Debug.Log($"[MouseState] Raycast target: {targetName}, position={FruityUI.RawMouseScreenPosition}");
            }

            var hoverTarget = FruityUI.DraggedTarget ?? lastRaycastTarget;

            // Only pass button if this target is the one being pressed (dragged or clicked)
            var pressButton = (hoverTarget == FruityUI.DraggedTarget || hoverTarget == activePress.target) 
                ? activePress.button 
                : MouseButton.None;
            
            QueueHoverEvent(hoverTarget, new HoverParams(lastRaycastNode, lastRaycastWorldPos, pressButton));
        }

        /// <summary>
        /// Process the next queued press event.
        /// If a press is active, force-completes it before processing the queued event.
        /// </summary>
        private void ProcessQueuedPress() {
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

            var pressTarget = (MouseTarget)FruityUI.DraggedTarget ?? lastRaycastTarget;
            UpdateMousePress(pressTarget);
        }

        /// <summary>
        /// Check for a new mouse button press and queue it for processing.
        /// </summary>
        private void CheckForNewPress() {
            // activeButton is set by the most recent raw button-down event.
            if (activeButton == MouseButton.None) return;

            // Time-based debounce: prevent rapid re-clicks from faulty hardware
            if (Time.unscaledTime <= lastPressTime + PRESS_DEBOUNCE_TIME) return;

            if (lastRaycastTarget != null) {
                pressEventQueue.Enqueue(new PressEvent {
                    target = lastRaycastTarget,
                    button = activeButton,
                    worldPosition = FruityUI.MouseWorldPosition
                });
                lastPressTime = Time.unscaledTime;
            }
        }

        /// <summary>
        /// Process a press event: start a click and/or drag based on the target's configuration.
        /// </summary>
        private void ProcessPress(PressEvent pressEvent) {
            var clickTarget = pressEvent.target as ClickTarget;
            var dragTarget = pressEvent.target as DragTarget;
            var dragMode = dragTarget?.GetDragMode(pressEvent.button) ?? MouseDragMode.Disabled;

            var pressRemainsActive = false;

            // Start click if allowed
            if (clickTarget != null && ShouldProcessClick(dragMode)) {
                pressRemainsActive |= TryStartClick(clickTarget, pressEvent);
            }

            // Start drag if enabled
            if (dragTarget != null && dragMode != MouseDragMode.Disabled) {
                pressRemainsActive |= TryStartDrag(dragTarget, pressEvent, dragMode);
            }

            if (!pressRemainsActive) {
                if (pressEvent.target != null)
                    activePress.Hold(pressEvent.target, pressEvent.button);
                else
                    activePress.Clear();
            }
        }

        /// <summary>
        /// Determine if a click should be processed based on the drag mode.
        /// Click events are only sent when drag mode is Disabled or DragOnly
        /// (PickUpOnly and DragOrPickUp use clicking to start/complete pickup, not to trigger click events).
        /// </summary>
        private bool ShouldProcessClick(MouseDragMode dragMode) {
            return dragMode == MouseDragMode.Disabled || 
                   dragMode == MouseDragMode.DragOnly;
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
                QueueClickEvent(clickTarget, new ClickParams(pressEvent.worldPosition, pressEvent.button));
                activePress.ReleaseClick();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Attempt to start a drag press.
        /// Returns true if the drag was started successfully.
        /// </summary>
        private bool TryStartDrag(DragTarget dragTarget, PressEvent pressEvent, MouseDragMode dragMode) {
            var screenPosition = FruityUI.RawMouseScreenPosition;
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
                CancelDragPress();
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
        /// Stores results in lastRaycastTarget, lastRaycastNode, and lastRaycastWorldPos.
        /// </summary>
        private void GetRaycastTarget() {
            // Track mouse movement
            var newMousePosition = FruityUI.RawMouseScreenPosition;
            mouseIsMoving = (newMousePosition != oldMousePosition);
            oldMousePosition = newMousePosition;

            if (!FruityUI.MouseIsOnscreen) {
                lastRaycastTarget = null;
                lastRaycastNode = null;
                lastRaycastWorldPos = Vector3.zero;
                return;
            }

            raycaster.CollideAndResolve(GetRelevantButton(), 
                out lastRaycastTarget, out lastRaycastNode, out lastRaycastWorldPos);
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
                var clickParams = new ClickParams(activePress.pressWorldPosition, activePress.button);
                clickParams.HeldDuration = Time.unscaledTime - lastPressTime;
                QueueClickEvent(clickTarget, clickParams);
                activePress.ReleaseClick();
            }

            activePress.Clear();
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
                FruityUI.DraggedOverTarget,
                activePress.pressScreenPosition,
                FruityUI.RawMouseScreenPosition,
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
                if (buttonDownThisEvent && buttonDownEventButton == activePress.button && activePress.pressStartFrame != Time.frameCount) {
                    // Build params after hierarchy has processed to get current DraggedOverTarget
                    var dragParams = BuildCurrentDragParams();

                    // Cancel drag if no valid drop target, otherwise complete
                    if (dragParams.DraggingOver == null) {
                        QueueDragCancelEvent();
                        ClearDragOverState();
                        activePress.Clear();
                    } else {
                        // Check if multi-place should keep the drag alive after placement
                        var multiPlaceTarget = FruityUI.DraggedTarget as MultiPlaceDragTarget;
                        var isMultiPlace = multiPlaceTarget != null && multiPlaceTarget.AllowMultiPlace(dragParams);
                        QueueDragCompleteEvent(dragParams, isMultiPlace);
                        if (!isMultiPlace) {
                            ClearDragOverState();
                            activePress.Clear();
                        }
                        // else: press and drag-over state are kept active for the next placement
                    }
                    return;
                }
            }

            // Normal mode: handle mouse button release
            var buttonWasReleased = !activePress.isPickUpDrag && !IsButtonHeld(activePress.button);
            if (buttonWasReleased) {
                // Complete or convert drag
                if (activePress.pressIsDrag) {
                    // For DragOrPickUp: short click converts to pickup mode
                    var screenPosition = FruityUI.RawMouseScreenPosition;
                    if (activePress.dragMode == MouseDragMode.DragOrPickUp && !activePress.WasRealDrag(screenPosition)) {
                        activePress.ConvertToPickUp();
                        return;
                    }
                    
                    // Build params after hierarchy has processed to get current DraggedOverTarget
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
                    var clickParams = new ClickParams(activePress.pressWorldPosition, activePress.button);
                    clickParams.HeldDuration = Time.unscaledTime - lastPressTime;
                    QueueClickEvent(clickTarget, clickParams);
                    activePress.ReleaseClick();
                }

                activePress.Clear();
                return;
            }

            // Continue drag
            if (activePress.pressIsDrag && FruityUI.DraggedTarget != null) {
                // Check if drag mode is still enabled
                var currentMode = FruityUI.DraggedTarget.GetDragMode(activePress.button);
                if (currentMode == MouseDragMode.Disabled) {
                    QueueDragCancelEvent();
                    ClearDragOverState();
                    CancelDragPress();
                    return;
                }

                // Check for cancel via opposite button
                var cancelButton = (activePress.button == MouseButton.Left) ? MouseButton.Right : MouseButton.Left;
                if (buttonDownThisEvent && buttonDownEventButton == cancelButton) {
                    QueueDragCancelEvent();
                    ClearDragOverState();
                    CancelDragPress();
                    return;
                }

                // Queue drag-over update with complete raycast results
                QueueDragOverUpdateEvent(lastRaycastTarget, lastRaycastNode, lastRaycastWorldPos, activePress.button);
                
                var dragParams = BuildCurrentDragParams();
                QueueDragUpdateEvent(dragParams);
            }
        }

        /// <summary>
        /// Cancel the active drag without completing it.
        /// Clears local drag flags but does not clear the entire press.
        /// </summary>
        private void CancelDragPress() {
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
            FruityInputRuntime.QueueEvent(new HoverHierarchyEvent {
                Target = target,
                Params = hoverParams 
            });
        }

        private static void QueueClickEvent(ClickTarget target, ClickParams clickParams) {
            FruityInputRuntime.QueueEvent(new ClickEvent { Target = target, Params = clickParams });
        }

        private static void QueueDragStartEvent(DragParams dragParams) {
            FruityInputRuntime.QueueEvent(new StartDragEvent { Params = dragParams });
        }

        private static void QueueDragUpdateEvent(DragParams dragParams) {
            FruityInputRuntime.QueueEvent(new UpdateDragEvent { Params = dragParams });
        }

        private static void QueueDragOverUpdateEvent(MouseTarget raycastTarget, InterfaceNode raycastNode, Vector3 mouseWorldPos, MouseButton button) {
            FruityInputRuntime.QueueEvent(new DragOverHierarchyEvent {
                RaycastTarget = raycastTarget,
                RaycastNode = raycastNode,
                MouseWorldPosition = mouseWorldPos,
                DragButton = button
            });
        }

        private static void QueueDragCompleteEvent(DragParams dragParams, bool isMultiPlace = false) {
            FruityInputRuntime.QueueEvent(new EndDragEvent { Params = dragParams, WasCancelled = false, IsMultiPlace = isMultiPlace });
        }

        private static void QueueDragCancelEvent(DragTarget cancelTarget = null) {
            FruityInputRuntime.QueueEvent(new EndDragEvent { Params = default, WasCancelled = true, CancelTarget = cancelTarget });
        }

        #endregion

    }

}
