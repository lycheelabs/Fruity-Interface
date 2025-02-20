using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Profiling;

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
        public static bool MouseIsDragging => InterfaceTargets.Dragged != null;
        public static bool DisableMouse { get; set; }
        public static bool IsIdle => InterfaceTargets.Highlighted == null && InterfaceTargets.Dragged == null;
        public static MouseTarget HighlightTarget => InterfaceTargets.Highlighted;
        public static MouseTarget DragOverTarget => currentDragOverTarget;

        private static MouseRaycaster Raycaster = new MouseRaycaster();
        
        // Targets
        private static MouseTarget currentDragOverTarget;

        // Events
        public delegate void TargetDelegate(ClickTarget newTarget);
        public static TargetDelegate OnNewPress;
        //public static TargetDelegate OnNewClick;

        // Click tracking
        private static bool mouseIsPressed;
        private static bool pressIsClick;
        private static bool pressIsDrag;
        private static MouseButton pressButton;
        private static ClickParams pressClickParams = ClickParams.blank;
        
        private static Vector3 oldMousePosition;
        private static Vector2 pressPixel;
        private static Vector3 pressPosition;
        private static float lastClickTime;
        private static bool selectionLock;

        public static void Update () {
            if (!Application.isFocused || DisableMouse) {
                return;
            }
            
            // Search
            var highlightParams = GetHighlightParams();
            QueueHighlightEvent(highlightParams);
            
            if (!mouseIsPressed) {
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
            if (pressClickParams.Target == target) {
                heldButton = pressButton;
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

            if (!mouseIsPressed && newPressedButton != MouseButton.None && Time.time > lastClickTime + 0.05f) {

                // Start a click-press (if applicable)
                if (clickTarget != null) {
                    PressClick(highlightParams, clickTarget, newPressedButton);

                    // Trigger an immediate click (if configured)
                    var clickOnPress = pressClickParams.Target?.ClickOnMouseDown == true;
                    if (clickOnPress) {
                        ReleaseClick();
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
            
            // Active drag params
            var dragParams = DragParams.Null;
            if (pressIsDrag) {
                dragParams = new DragParams(InterfaceTargets.Dragged, mouseTarget,
                    pressPixel, Input.mousePosition, pressButton);
            }

            // Release mouse press
            if (mouseIsPressed && !Input.GetMouseButton((int)pressButton)) {
                if (pressIsDrag) {
                    QueueDragCompleteEvent(dragParams);
                }
                if (pressIsClick && pressClickParams.Target == mouseTarget) {
                    ReleaseClick();
                }
                ClearPress();
                return;
            }

            // Update or cancel drag
            if (pressIsDrag) {
                UpdateDrag(dragParams);
            }
        }

        private static void PressClick(HighlightParams highlightParams, ClickTarget clickTarget, MouseButton pressedButton) {
            mouseIsPressed = true;
            pressIsClick = true;
            pressButton = pressedButton;

            lastClickTime = Time.time;
            pressPixel = Input.mousePosition;
            pressPosition = highlightParams.MouseWorldPosition;
            pressClickParams = new ClickParams(clickTarget, pressPosition, pressedButton);

            OnNewPress?.Invoke(pressClickParams.Target);
        }

        private static void StartDrag(DragTarget dragTarget, MouseTarget mouseTarget, MouseButton pressedButton) {
            mouseIsPressed = true;
            pressIsDrag = true;
            pressButton = pressedButton;

            var dragParams = new DragParams(dragTarget, mouseTarget,
                pressPixel, pressPixel, pressButton);
            QueueDragStartEvent(dragParams);
        }

        private static void UpdateDrag(DragParams dragParams) {
            var manualDragCancel = InterfaceTargets.Dragged?.DraggingIsEnabled == false ||
                                   (dragParams.DragButton == MouseButton.Left && Input.GetMouseButtonDown((int)MouseButton.Right)) ||
                                   (dragParams.DragButton == MouseButton.Right && Input.GetMouseButtonDown((int)MouseButton.Left));

            if (mouseIsPressed && !manualDragCancel) {
                QueueDragUpdateEvent(dragParams);
            } else {
                QueueDragCancelEvent();
                pressIsDrag = false;
            }
        }

        private static void ReleaseClick() {
            pressClickParams.HeldDuration = Time.time - lastClickTime;
            QueueClickEvent(pressClickParams);
            ClearPress();
        }

        private static void ClearPress()  {
            mouseIsPressed = false;
            pressIsClick = false;
            pressIsDrag = false;
            pressClickParams = ClickParams.blank;
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