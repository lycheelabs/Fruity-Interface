using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class MouseState {

        public static bool MouseIsMoving { get; private set; }
        public static bool DisableMouse { get; set; }

        private MouseRaycaster Raycaster;
        private MouseButton activeButton;
        private MousePress press;
        private float lastPressTime;
        private Vector3 oldMousePosition;

        // Delegates
        public delegate void TargetDelegate(ClickTarget newTarget);
        public static TargetDelegate OnNewPress;

        public MouseState() {
            Raycaster = new MouseRaycaster();
            activeButton = MouseButton.None;
        }
        
        public void Update () {
            if (!Application.isFocused || DisableMouse) {
                return;
            }

            UpdateActiveButton();

            // Hovering
            var hover = GetHoverParams();
            QueueHighlightEvent(hover);

            // Pressing
            if (!press.isPressed) {
                CheckForMousePress(hover);
            } else {
                UpdateMousePress(hover);
            }
        }

        public void ClickNow(ClickTarget target, MouseButton button, Vector3 clickWorldPosition = default) {
            QueueClickEvent(new ClickParams(target, clickWorldPosition, button));
        }

        #region Raycasting
        private void UpdateActiveButton() {
            if (Input.GetMouseButtonDown((int)MouseButton.Left)) {
                activeButton = MouseButton.Left;
            }
            else if (Input.GetMouseButtonDown((int)MouseButton.Right)) {
                activeButton = MouseButton.Right;
            }
            else {
                if (!Input.GetMouseButton((int)MouseButton.Left) &&
                    !Input.GetMouseButton((int)MouseButton.Right)) {
                    activeButton = MouseButton.None;
                }
            }
        }

        private HighlightParams GetHoverParams () {

            // Check for movement
            var newMousePosition = Input.mousePosition;
            MouseIsMoving = (newMousePosition != oldMousePosition);
            oldMousePosition = newMousePosition;

            // Resolve mouse highlighting
            var highlightParams = GetRaycast();
            
            // Override
            FruityUI.DraggedOverTarget = highlightParams.Target;
            OverrideHighlight(ref highlightParams);
            return highlightParams;
            
        }

        private HighlightParams GetRaycast () {
            if (!FruityUI.MouseIsOnscreen) {
                return HighlightParams.blank;
            }

            // Raycast
            var button = (press.isPressed) ? press.button : activeButton;
            Raycaster.CollideAndResolve(button, out var target, out var targetNode, out var targetPoint);
            
            if (target == null) {
                return HighlightParams.blank;
            }     
            return new HighlightParams(target, targetNode, targetPoint, button);
        }

        private void OverrideHighlight(ref HighlightParams highlightParams) {
            if (FruityUI.DraggedTarget != null) {
                highlightParams.Target = FruityUI.DraggedTarget;
            }
            else if (GrabTarget.CurrentGrabbedInstance != null) {
                highlightParams.Target = GrabTarget.CurrentGrabbedInstance;
            }
        }
        
        #endregion

        #region Clicking
        private void CheckForMousePress (HighlightParams highlightParams) {
            var mouseTarget = highlightParams.Target;
            var clickTarget = mouseTarget as ClickTarget;
            var dragTarget = mouseTarget as DragTarget;
            
            // Find pressed button
            var newPressedButton = MouseButton.None;
            if (Input.GetMouseButtonDown((int)MouseButton.Left)) newPressedButton = MouseButton.Left;
            if (Input.GetMouseButtonDown((int)MouseButton.Right)) newPressedButton = MouseButton.Right;

            if (!press.isPressed && newPressedButton != MouseButton.None && Time.time > lastPressTime + 0.05f) {
                press.pressPosition = highlightParams.MouseWorldPosition;

                var pressed = false;
                var dragged = false;

                // Start a click-press (if applicable)
                if (clickTarget != null) {
                    PressClick(clickTarget, newPressedButton);
                    pressed = true;

                    // Trigger an immediate click (if configured)
                    var clickOnPress = clickTarget.ClickOnMouseDown == true;
                    if (clickOnPress) {
                        press.pressIsClick = false;
                        ReleaseClick(clickTarget);
                        pressed = false;
                    }
                }

                // Start a drag (if applicable)
                if (dragTarget != null && dragTarget.DraggingIsEnabled(newPressedButton)) {
                    var dragPosition = Camera.main.WorldToScreenPoint(press.pressPosition);
                    var dragParams = new DragParams(dragTarget, mouseTarget,
                        dragPosition, Input.mousePosition, newPressedButton);
                    StartDrag(dragParams);
                    dragged = true;
                }

                if (!pressed && !dragged) {
                    press.Clear();
                }
            }
        }

        private void UpdateMousePress(HighlightParams highlightParams) {
            var mouseTarget = highlightParams.Target;
            var clickTarget = mouseTarget as ClickTarget;
            
            // Active drag params
            var dragParams = DragParams.Null;
            if (press.pressIsDrag) {
                var dragPosition = Camera.main.WorldToScreenPoint(press.pressPosition);
                dragParams = new DragParams(FruityUI.DraggedTarget, mouseTarget,
                    dragPosition, Input.mousePosition, press.button);
            }

            // Release mouse press
            if (press.isPressed && !Input.GetMouseButton((int)press.button)) {
                if (press.pressIsDrag) {
                    QueueDragCompleteEvent(dragParams);
                }
                if (press.pressIsClick && press.target == clickTarget) {
                    ReleaseClick(clickTarget);
                }
                press.Clear();
                return;
            }

            // Update or cancel drag
            if (press.pressIsDrag && FruityUI.DraggedTarget != null) {
                UpdateDrag(dragParams);
            }
        }

        private void PressClick(ClickTarget clickTarget, MouseButton pressedButton) {
            press.StartClick(clickTarget, pressedButton);
            lastPressTime = Time.time;
            
            OnNewPress?.Invoke(clickTarget);
        }

        private void StartDrag(DragParams dragParams) {
            press.StartDrag(dragParams.Target, dragParams.DragButton);
            QueueDragStartEvent(dragParams);
        }

        private void UpdateDrag(DragParams dragParams) {

            // Check drag is enabled
            if (!FruityUI.DraggedTarget.DraggingIsEnabled(dragParams.DragButton)) {
                QueueDragCancelEvent();
                press.pressIsDrag = false;
            }

            // Check for manual drag cancel
            var manualDragCancel =
                   (dragParams.DragButton == MouseButton.Left && Input.GetMouseButtonDown((int)MouseButton.Right)) ||
                   (dragParams.DragButton == MouseButton.Right && Input.GetMouseButtonDown((int)MouseButton.Left));

            if (press.isPressed && !manualDragCancel) {
                QueueDragUpdateEvent(dragParams);
            } else {
                QueueDragCancelEvent();
                press.pressIsDrag = false;
            }
        }

        private void ReleaseClick(ClickTarget target) {
            var clickParams = new ClickParams(target, press.pressPosition, press.button);
            clickParams.HeldDuration = Time.time - lastPressTime;
            
            QueueClickEvent(clickParams);
            press.ReleaseClick();
        }
        
        #endregion

        #region Events
        private static void QueueHighlightEvent (HighlightParams highlightParams) {
            FruityUIManager.Queue(new HighlightEvent {
                Params = highlightParams
            });
        }

        private static void QueueClickEvent (ClickParams clickParams) {
            FruityUIManager.Queue(new ClickEvent {
                Params = clickParams
            });
        }
        
        private static void QueueDragStartEvent(DragParams dragParams) {
            FruityUIManager.Queue(new StartDragEvent {
                Params = dragParams
            });
        }

        private static void QueueDragUpdateEvent (DragParams dragParams) {
            FruityUIManager.Queue(new UpdateDragEvent {
                Params = dragParams
            });
        }

        private static void QueueDragCompleteEvent (DragParams dragParams) {
            FruityUIManager.Queue(new EndDragEvent {
                Params = dragParams,
                WasCancelled = false
            });
        }

        private static void QueueDragCancelEvent () {
            FruityUIManager.Queue(new EndDragEvent {
                Params = default,
                WasCancelled = true
            });
        }
        #endregion

    }
}