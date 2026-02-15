using UnityEngine;
using System.Collections.Generic;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Manages mouse input state, raycasting, and event generation for the UI system.
    /// </summary>
    public class MouseState {

        public static bool MouseIsMoving { get; private set; }
        public static bool DisableMouse { get; set; }

        private MouseRaycaster Raycaster;
        private MouseButton activeButton;
        private MousePress press;
        private float lastPressTime;
        private Vector3 oldMousePosition;

        /// <summary>
        /// Press events to be processed (both real and synthetic input).
        /// </summary>
        private struct PressEvent {
            public MouseTarget target;
            public MouseButton button;
            public Vector3 worldPosition;
        }

        private Queue<PressEvent> pressEvents;

        // Delegates
        public delegate void TargetDelegate (ClickTarget newTarget);
        public static TargetDelegate OnNewPress;

        public MouseState () {
            Raycaster = new MouseRaycaster();
            activeButton = MouseButton.None;
            pressEvents = new Queue<PressEvent>();
        }

        public void Update () {
            if (!Application.isFocused || DisableMouse) {
                return;
            }

#if UNITY_EDITOR
            ValidateState();
#endif

            UpdateActiveButton();

            // Raycasting
            var raycastTarget = GetRaycastTarget(out var raycastNode, out var raycastWorldPos);
            FruityUI.DraggedOverTarget = raycastTarget;

            // Highlighting
            var highlightTarget = (FruityUI.DraggedTarget != null) ? FruityUI.DraggedTarget : raycastTarget;
            QueueHighlightEvent(new HighlightParams(highlightTarget, raycastNode, raycastWorldPos, GetRelevantButton()));

            // Check for new real input (only when idle)
            if (pressEvents.Count == 0 && !press.isPressed) {
                CheckForMousePress(raycastTarget, raycastWorldPos);
            }

            if (pressEvents.Count > 0) {
                // Process next queued press
                ProcessNextQueuedEvent();
            } 
            else if (press.isPressed) {
                // Update ongoing press
                UpdateMousePress(highlightTarget);
            }
        }

        /// <summary>
        /// Attempt to process a queued press event.
        /// Returns true if processing occurred (skip normal update this frame).
        /// </summary>
        private bool ProcessNextQueuedEvent () {
            if (pressEvents.Count == 0) return false;

            // Force-complete current press before processing queued event
            if (press.isPressed) {
                ForceCompleteCurrentPress();
                return true; // Wait for next frame for events to process
            }

            // Process queued event
            var pressEvent = pressEvents.Dequeue();
            ProcessPress(pressEvent);
            return true; // Processed, skip normal update
        }

        /// <summary>
        /// Validate state invariants (editor only).
        /// </summary>
        private void ValidateState () {
            // Detect desync between FruityUI.DraggedTarget and press state
            if (FruityUI.DraggedTarget != null && !press.pressIsDrag) {
                Debug.LogWarning("[MouseState] State desync: DraggedTarget set but no press drag active");
                FruityUI.DraggedTarget = null;
            }

            if (press.pressIsDrag && FruityUI.DraggedTarget == null) {
                Debug.LogWarning("[MouseState] State desync: Press drag active but no DraggedTarget");
                press.pressIsDrag = false;
                press.isPickUpDrag = false;
            }

            // Prevent queue overflow (abuse protection)
            if (pressEvents.Count > 10) {
                Debug.LogError("[MouseState] Press event queue overflow - clearing to prevent abuse");
                pressEvents.Clear();
            }
        }

        /// <summary>
        /// Queue a synthetic press that will be processed as if the user clicked.
        /// Supports both clicks and drags (based on target's DragMode).
        /// Queued presses are processed in the next frame when no press is active.
        /// If the target doesn't implement ClickTarget or DragTarget, it is silently ignored.
        /// </summary>
        public void QueueClick (MouseTarget target, MouseButton button) {
            if (target == null) return;

            // Only queue if target implements at least one of the required interfaces
            if (!(target is ClickTarget) && !(target is DragTarget)) {
                return; // Silently skip incompatible targets
            }

            pressEvents.Enqueue(new PressEvent {
                target = target,
                button = button,
                worldPosition = FruityUI.MouseWorldPosition
            });
        }

        /// <summary>
        /// Immediately trigger a click event on a target (bypasses normal mouse flow).
        /// Use QueueClick instead for drag-capable targets.
        /// </summary>
        public void ClickNow (ClickTarget target, MouseButton button, Vector3 clickWorldPosition = default) {
            QueueClickEvent(new ClickParams(target, clickWorldPosition, button));
        }

        #region Input State

        private void UpdateActiveButton () {
            if (Input.GetMouseButtonDown((int)MouseButton.Left)) {
                activeButton = MouseButton.Left;
            } else if (Input.GetMouseButtonDown((int)MouseButton.Right)) {
                activeButton = MouseButton.Right;
            } else if (!Input.GetMouseButton((int)MouseButton.Left) && !Input.GetMouseButton((int)MouseButton.Right)) {
                activeButton = MouseButton.None;
            }
        }

        private MouseButton GetRelevantButton () {
            return press.isPressed ? press.button : activeButton;
        }

        #endregion

        #region Raycasting

        private MouseTarget GetRaycastTarget (out InterfaceNode node, out Vector3 worldPosition) {
            // Track mouse movement
            var newMousePosition = Input.mousePosition;
            MouseIsMoving = (newMousePosition != oldMousePosition);
            oldMousePosition = newMousePosition;

            if (!FruityUI.MouseIsOnscreen) {
                node = null;
                worldPosition = Vector3.zero;
                return null;
            }

            Raycaster.CollideAndResolve(GetRelevantButton(), out var target, out node, out worldPosition);
            return target;
        }

        #endregion

        #region Press Handling

        private void CheckForMousePress (MouseTarget raycastTarget, Vector3 raycastWorldPos) {
            // Check for new button press
            var pressedButton = MouseButton.None;
            if (Input.GetMouseButtonDown((int)MouseButton.Left)) pressedButton = MouseButton.Left;
            else if (Input.GetMouseButtonDown((int)MouseButton.Right)) pressedButton = MouseButton.Right;

            if (pressedButton == MouseButton.None) return;
            if (Time.unscaledTime <= lastPressTime + 0.05f) return; // Debounce

            // Queue the real mouse press
            pressEvents.Enqueue(new PressEvent {
                target = raycastTarget,
                button = pressedButton,
                worldPosition = raycastWorldPos
            });
        }

        private void ProcessPress (PressEvent pressEvent) {
            var clickTarget = pressEvent.target as ClickTarget;
            var dragTarget = pressEvent.target as DragTarget;

            // Determine drag mode
            var dragMode = DragTarget.DragMode.Disabled;
            if (dragTarget != null) {
                dragMode = dragTarget.GetDragMode(pressEvent.button);
            }

            // Click events are only sent when drag mode is Disabled or DragOnly
            // (PickUpOnly and DragOrPickUp use clicking to start/complete pickup, not to trigger click events)
            var allowClick = (dragMode == DragTarget.DragMode.Disabled || dragMode == DragTarget.DragMode.DragOnly);

            var startedClick = false;
            var startedDrag = false;

            // Start a click
            if (clickTarget != null && allowClick) {
                press.StartClick(clickTarget, pressEvent.button);
                lastPressTime = Time.unscaledTime;
                OnNewPress?.Invoke(clickTarget);
                startedClick = true;

                // Handle immediate click (ClickOnMouseDown)
                if (clickTarget.ClickOnMouseDown) {
                    var clickParams = new ClickParams(clickTarget, pressEvent.worldPosition, pressEvent.button);
                    QueueClickEvent(clickParams);
                    press.ReleaseClick();
                    startedClick = false;
                }
            }

            // Start a drag
            if (dragTarget != null && dragMode != DragTarget.DragMode.Disabled) {
                var screenPosition = (Vector2)Input.mousePosition;
                press.StartDrag(dragTarget, pressEvent.button, dragMode, pressEvent.worldPosition, screenPosition);

                // At drag start, Target and DraggingOver are the same (we clicked on the target)
                var dragParams = new DragParams(dragTarget, dragTarget, screenPosition, screenPosition, pressEvent.button);
                QueueDragStartEvent(dragParams);
                startedDrag = true;
            }

            if (!startedClick && !startedDrag) {
                press.Clear();
            }
        }

        private void ForceCompleteCurrentPress () {
            if (!press.isPressed) return;

            // Complete drag if active
            if (press.pressIsDrag && FruityUI.DraggedTarget != null) {
                QueueDragCompleteEvent(BuildCurrentDragParams());
            }

            // Complete click if active
            if (press.pressIsClick && press.target is ClickTarget clickTarget) {
                var clickParams = new ClickParams(clickTarget, press.pressWorldPosition, press.button);
                clickParams.HeldDuration = Time.unscaledTime - lastPressTime;
                QueueClickEvent(clickParams);
                press.ReleaseClick();
            }

            press.Clear();
        }

        private DragParams BuildCurrentDragParams () {
            if (!press.pressIsDrag || FruityUI.DraggedTarget == null) {
                return DragParams.Null;
            }

            return new DragParams(
                FruityUI.DraggedTarget,
                FruityUI.DraggedOverTarget,
                press.pressScreenPosition,
                (Vector2)Input.mousePosition,
                press.button
            );
        }

        private void UpdateMousePress (MouseTarget highlightTarget) {
            var clickTarget = highlightTarget as ClickTarget;
            var dragParams = BuildCurrentDragParams();

            // Pickup mode: complete on second click of same button
            if (press.pressIsDrag && press.isPickUpDrag) {
                if (Input.GetMouseButtonDown((int)press.button) && press.pressStartFrame != Time.frameCount) {
                    QueueDragCompleteEvent(dragParams);
                    press.Clear();
                    return;
                }
            }

            // Normal mode: handle mouse button release
            if (!press.isPickUpDrag && !Input.GetMouseButton((int)press.button)) {
                // Complete or convert drag
                if (press.pressIsDrag) {
                    // For DragOrPickUp: short click converts to pickup mode
                    var screenPosition = (Vector2)Input.mousePosition;
                    if (press.dragMode == DragTarget.DragMode.DragOrPickUp && !press.WasRealDrag(screenPosition)) {
                        press.ConvertToPickUp();
                        return;
                    }
                    QueueDragCompleteEvent(dragParams);
                }

                // Complete click
                if (press.pressIsClick && press.target == clickTarget) {
                    var clickParams = new ClickParams(clickTarget, press.pressWorldPosition, press.button);
                    clickParams.HeldDuration = Time.unscaledTime - lastPressTime;
                    QueueClickEvent(clickParams);
                    press.ReleaseClick();
                }

                press.Clear();
                return;
            }

            // Continue drag
            if (press.pressIsDrag && FruityUI.DraggedTarget != null) {
                // Check if drag mode is still enabled
                var currentMode = FruityUI.DraggedTarget.GetDragMode(press.button);
                if (currentMode == DragTarget.DragMode.Disabled) {
                    QueueDragCancelEvent();
                    press.pressIsDrag = false;
                    press.isPickUpDrag = false;
                    return;
                }

                // Check for cancel via opposite button
                var cancelButton = (press.button == MouseButton.Left) ? MouseButton.Right : MouseButton.Left;
                if (Input.GetMouseButtonDown((int)cancelButton)) {
                    QueueDragCancelEvent();
                    press.pressIsDrag = false;
                    press.isPickUpDrag = false;
                    return;
                }

                QueueDragUpdateEvent(dragParams);
            }
        }

        #endregion

        #region Event Queuing

        private static void QueueHighlightEvent (HighlightParams highlightParams) {
            FruityUIManager.Queue(new HighlightEvent { Params = highlightParams });
        }

        private static void QueueClickEvent (ClickParams clickParams) {
            FruityUIManager.Queue(new ClickEvent { Params = clickParams });
        }

        private static void QueueDragStartEvent (DragParams dragParams) {
            FruityUIManager.Queue(new StartDragEvent { Params = dragParams });
        }

        private static void QueueDragUpdateEvent (DragParams dragParams) {
            FruityUIManager.Queue(new UpdateDragEvent { Params = dragParams });
        }

        private static void QueueDragCompleteEvent (DragParams dragParams) {
            FruityUIManager.Queue(new EndDragEvent { Params = dragParams, WasCancelled = false });
        }

        private static void QueueDragCancelEvent () {
            FruityUIManager.Queue(new EndDragEvent { Params = default, WasCancelled = true });
        }

        #endregion

    }

}
