using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public static class Mouse {

        public static bool MouseIsInBounds =>
            Input.mousePosition.x >= 1  && 
            Input.mousePosition.y >= 1 && 
            Input.mousePosition.x < (Screen.width - 1) && 
            Input.mousePosition.y < (Screen.height - 1);

        public static Vector2 MouseScreenPosition => ((Vector2)Input.mousePosition * InterfaceConfig.UIScaling) - InterfaceConfig.LetterboxOffset;
        public static Vector3 MouseWorldPosition => InterfaceConfig.MouseToWorldPoint();
        public static bool MouseIsMoving { get; private set; }
        public static bool DisableMouse { get; set; }
        public static MouseTarget HighlightTarget => InterfaceTargets.Highlighted;
        public static MouseTarget DragOverTarget => currentDragOverTarget;

        // Components
        private static MouseRaycaster Raycaster = new MouseRaycaster();
        
        // Targets
        private static MouseTarget currentDragOverTarget;

        // Events
        public delegate void TargetDelegate(ClickTarget newTarget);
        public static TargetDelegate OnNewPress;
        //public static TargetDelegate OnNewClick;

        // Click tracking
        private static MousePress press;
        private static float lastPressTime;
        public static Vector3 oldMousePosition;

        public static void Update () {
            if (!Application.isFocused || DisableMouse) {
                return;
            }
            
            // Search
            var highlightParams = GetHighlightParams();
            QueueHighlightEvent(highlightParams);
            
            if (!press.isPressed) {
                CheckForMousePress(highlightParams);
            } else {
                UpdateMousePress(highlightParams);
            }

        }

        public static void Click (ClickTarget target, MouseButton button, Vector3 clickWorldPosition = default) {
            QueueClickEvent(new ClickParams(target, clickWorldPosition, button));
        }

        #region Raycasting
        private static HighlightParams GetHighlightParams () {

            // Check for movement
            var newMousePosition = Input.mousePosition;
            MouseIsMoving = (newMousePosition != oldMousePosition);
            oldMousePosition = newMousePosition;

            // Resolve mouse highlighting
            var highlightParams = GetRaycast();
            
            // Override
            currentDragOverTarget = highlightParams.Target;
            OverrideHighlight(ref highlightParams);
            return highlightParams;
            
        }

        private static HighlightParams GetRaycast () {
            if (!MouseIsInBounds) {
                return HighlightParams.blank;
            }
            
            // Raycast
            Raycaster.CollideAndResolve(out var target, out var targetPoint);
            if (target == null) {
                return HighlightParams.blank;
            }
            
            var heldButton = MouseButton.None;
            if (press.target == target) {
                heldButton = press.button;
            }
            return new HighlightParams(target, targetPoint, heldButton);
        }

        private static void OverrideHighlight(ref HighlightParams highlightParams) {
            if (InterfaceTargets.Dragged != null) {
                highlightParams.Target = InterfaceTargets.Dragged;
            }
            else if (GrabTarget.CurrentGrabbedInstance != null) {
                highlightParams.Target = GrabTarget.CurrentGrabbedInstance;
            }
        }
        
        #endregion

        #region Clicking
        private static void CheckForMousePress (HighlightParams highlightParams) {
            var mouseTarget = highlightParams.Target;
            var clickTarget = mouseTarget as ClickTarget;
            var dragTarget = mouseTarget as DragTarget;
            
            // Find pressed button
            var newPressedButton = MouseButton.None;
            if (Input.GetMouseButtonDown((int)MouseButton.Left)) newPressedButton = MouseButton.Left;
            if (Input.GetMouseButtonDown((int)MouseButton.Right)) newPressedButton = MouseButton.Right;

            if (!press.isPressed && newPressedButton != MouseButton.None && Time.time > lastPressTime + 0.05f) {
                press.pressPosition = highlightParams.MouseWorldPosition;
                
                // Start a click-press (if applicable)
                if (clickTarget != null) {
                    PressClick(clickTarget, newPressedButton);

                    // Trigger an immediate click (if configured)
                    var clickOnPress = clickTarget.ClickOnMouseDown == true;
                    if (clickOnPress) {
                        ReleaseClick(clickTarget);
                        return; // Don't drag!
                    }
                }

                // Start a drag (if applicable)
                if (dragTarget != null && dragTarget.DraggingIsEnabled)  {
                    StartDrag(dragTarget, mouseTarget, newPressedButton);
                }
            }
        }

        private static void UpdateMousePress(HighlightParams highlightParams) {
            var mouseTarget = highlightParams.Target;
            var clickTarget = mouseTarget as ClickTarget;
            
            // Active drag params
            var dragParams = DragParams.Null;
            if (press.pressIsDrag) {
                var dragPosition = Camera.main.WorldToScreenPoint(press.pressPosition);
                dragParams = new DragParams(InterfaceTargets.Dragged, mouseTarget,
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
                press.Release();
                return;
            }

            // Update or cancel drag
            if (press.pressIsDrag) {
                UpdateDrag(dragParams);
            }
        }

        private static void PressClick(ClickTarget clickTarget, MouseButton pressedButton) {
            press.StartClick(clickTarget, pressedButton);
            lastPressTime = Time.time;
            
            OnNewPress?.Invoke(clickTarget);
        }

        private static void StartDrag(DragTarget dragTarget, MouseTarget mouseTarget, MouseButton pressedButton) {
            press.StartDrag(dragTarget, pressedButton);

            var dragParams = new DragParams(dragTarget, mouseTarget,
                MouseWorldPosition, MouseWorldPosition, press.button);
            QueueDragStartEvent(dragParams);
        }

        private static void UpdateDrag(DragParams dragParams) {
            var manualDragCancel = InterfaceTargets.Dragged?.DraggingIsEnabled == false ||
                   (dragParams.DragButton == MouseButton.Left && Input.GetMouseButtonDown((int)MouseButton.Right)) ||
                   (dragParams.DragButton == MouseButton.Right && Input.GetMouseButtonDown((int)MouseButton.Left));

            if (press.isPressed && !manualDragCancel) {
                QueueDragUpdateEvent(dragParams);
            } else {
                QueueDragCancelEvent();
                press.pressIsDrag = false;
            }
        }

        private static void ReleaseClick(ClickTarget target) {
            var clickParams = new ClickParams(target, press.pressPosition, press.button);
            clickParams.HeldDuration = Time.time - lastPressTime;
            
            QueueClickEvent(clickParams);
            press.Release();
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