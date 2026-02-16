using UnityEngine;
using System.Collections.Generic;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Manages mouse input state, raycasting, and event generation for the UI system.
    /// </summary>
    public class MouseState {

        /// <summary> Mouse button press event to be processed. </summary>
        private struct PressEvent {
            public MouseTarget target;
            public MouseButton button;
            public Vector3 worldPosition;
        }

        public static bool MouseIsMoving { get; private set; }
        public static bool DisableMouse { get; set; }

        // ---------------------------------------------

        private MouseRaycaster raycaster;
        private Queue<PressEvent> pressEventQueue;

        private MouseButton activeButton;
        private MousePress activePress;
        private float lastPressTime;
        private Vector3 oldMousePosition;

        // Delegates
        public delegate void TargetDelegate (ClickTarget newTarget);
        public static TargetDelegate OnNewPress;

        public MouseState () {
            raycaster = new MouseRaycaster();
            pressEventQueue = new Queue<PressEvent>();
            activeButton = MouseButton.None;
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
            if (pressEventQueue.Count == 0 && !activePress.isPressed) {
                CheckForMousePress(raycastTarget, raycastWorldPos);
            }

            // Process next queued press
            if (pressEventQueue.Count > 0) {
                ProcessNextQueuedEvent();
            }

            // Update ongoing press
            if (activePress.isPressed) {
                UpdateMousePress(highlightTarget);
            }
        }

        /// <summary>
        /// Process the next queued press event.
        /// If a press is active, force-completes it before processing the queued event.
        /// </summary>
        private void ProcessNextQueuedEvent () {
            // Force-complete current press before processing queued event
            if (activePress.isPressed) {
                ForceCompleteCurrentPress();
            }

            // Process queued event
            var pressEvent = pressEventQueue.Dequeue();
            ProcessPress(pressEvent);
        }

        /// <summary>
        /// Validate state invariants (editor only).
        /// </summary>
        private void ValidateState () {
            // Detect desync between FruityUI.DraggedTarget and press state
            if (FruityUI.DraggedTarget != null && !activePress.pressIsDrag) {
                Debug.LogWarning("[MouseState] State desync: DraggedTarget set but no press drag active");
                FruityUI.DraggedTarget = null;
            }

            if (activePress.pressIsDrag && FruityUI.DraggedTarget == null) {
                Debug.LogWarning("[MouseState] State desync: Press drag active but no DraggedTarget");
                activePress.pressIsDrag = false;
                activePress.isPickUpDrag = false;
            }

            // Prevent queue overflow (abuse protection)
            if (pressEventQueue.Count > 10) {
                Debug.LogError("[MouseState] Press event queue overflow - clearing to prevent abuse");
                pressEventQueue.Clear();
            }
        }

        /// <summary>
        /// Queue a synthetic press that will be processed as if the user clicked.
        /// Supports both clicks and drags, based on the MouseTarget's behaviour.
        /// (If the target doesn't implement ClickTarget or DragTarget, it is silently ignored.)
        /// </summary>
        public void QueueClick (MouseTarget target, MouseButton button) {
            if (target == null) return;

            // Only queue if target implements at least one of the required interfaces
            if ((target is ClickTarget) || (target is DragTarget)) {
                pressEventQueue.Enqueue(new PressEvent {
                    target = target,
                    button = button,
                    worldPosition = FruityUI.MouseWorldPosition
                });
            }
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
            return activePress.isPressed ? activePress.button : activeButton;
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

            raycaster.CollideAndResolve(GetRelevantButton(), out var target, out node, out worldPosition);
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
            pressEventQueue.Enqueue(new PressEvent {
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
                activePress.StartClick(clickTarget, pressEvent.button);
                lastPressTime = Time.unscaledTime;
                OnNewPress?.Invoke(clickTarget);
                startedClick = true;

                // Handle immediate click (ClickOnMouseDown)
                if (clickTarget.ClickOnMouseDown) {
                    var clickParams = new ClickParams(clickTarget, pressEvent.worldPosition, pressEvent.button);
                    QueueClickEvent(clickParams);
                    activePress.ReleaseClick();
                    startedClick = false;
                }
            }

            // Start a drag
            if (dragTarget != null && dragMode != DragTarget.DragMode.Disabled) {
                var screenPosition = (Vector2)Input.mousePosition;
                activePress.StartDrag(dragTarget, pressEvent.button, dragMode, pressEvent.worldPosition, screenPosition);

                // At drag start, Target and DraggingOver are the same (we clicked on the target)
                var dragParams = new DragParams(dragTarget, dragTarget, screenPosition, screenPosition, pressEvent.button);
                QueueDragStartEvent(dragParams);
                startedDrag = true;
            }

            if (!startedClick && !startedDrag) {
                activePress.Clear();
            }
        }

        private void ForceCompleteCurrentPress () {
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
        }

        private DragParams BuildCurrentDragParams () {
            if (!activePress.pressIsDrag || FruityUI.DraggedTarget == null) {
                return DragParams.Null;
            }

            return new DragParams(
                FruityUI.DraggedTarget,
                FruityUI.DraggedOverTarget,
                activePress.pressScreenPosition,
                (Vector2)Input.mousePosition,
                activePress.button
            );
        }

        private void UpdateMousePress (MouseTarget highlightTarget) {
            var clickTarget = highlightTarget as ClickTarget;
            var dragParams = BuildCurrentDragParams();

            // Pickup mode: complete on second click of same button
            if (activePress.pressIsDrag && activePress.isPickUpDrag) {
                if (Input.GetMouseButtonDown((int)activePress.button) && activePress.pressStartFrame != Time.frameCount) {
                    QueueDragCompleteEvent(dragParams);
                    activePress.Clear();
                    return;
                }
            }

            // Normal mode: handle mouse button release
            if (!activePress.isPickUpDrag && !Input.GetMouseButton((int)activePress.button)) {
                // Complete or convert drag
                if (activePress.pressIsDrag) {
                    // For DragOrPickUp: short click converts to pickup mode
                    var screenPosition = (Vector2)Input.mousePosition;
                    if (activePress.dragMode == DragTarget.DragMode.DragOrPickUp && !activePress.WasRealDrag(screenPosition)) {
                        activePress.ConvertToPickUp();
                        return;
                    }
                    QueueDragCompleteEvent(dragParams);
                }

                // Complete click
                if (activePress.pressIsClick && activePress.target == clickTarget) {
                    var clickParams = new ClickParams(clickTarget, activePress.pressWorldPosition, activePress.button);
                    clickParams.HeldDuration = Time.unscaledTime - lastPressTime;
                    QueueClickEvent(clickParams);
                    activePress.ReleaseClick();
                }

                activePress.Clear();
                return;
            }

            // Continue drag
            if (activePress.pressIsDrag && FruityUI.DraggedTarget != null) {
                // Check if drag mode is still enabled
                var currentMode = FruityUI.DraggedTarget.GetDragMode(activePress.button);
                if (currentMode == DragTarget.DragMode.Disabled) {
                    QueueDragCancelEvent();
                    activePress.pressIsDrag = false;
                    activePress.isPickUpDrag = false;
                    return;
                }

                // Check for cancel via opposite button
                var cancelButton = (activePress.button == MouseButton.Left) ? MouseButton.Right : MouseButton.Left;
                if (Input.GetMouseButtonDown((int)cancelButton)) {
                    QueueDragCancelEvent();
                    activePress.pressIsDrag = false;
                    activePress.isPickUpDrag = false;
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
