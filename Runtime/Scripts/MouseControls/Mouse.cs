using UnityEngine;
using UnityEngine.Profiling;

namespace LycheeLabs.FruityInterface {

    public static class Mouse {

        private static bool enableLogging = false;

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
        public static TargetDelegate OnNewClick;

        // Click tracking
        private static Vector3 oldMousePosition;
        private static bool mouseIsPressed;
        private static Vector2 pressPixel;
        private static Vector3 pressPosition;
        private static ClickParams pressedClickParams = ClickParams.blank;
        private static float lastClickTime;
        private static bool selectionLock;

        private static bool queuedClick;
        private static ClickParams queuedClickParams;

        public static void Update (bool logEvents) {
            enableLogging = logEvents;
            CastRays();

            if (queuedClick) {
                InstantClick(queuedClickParams);
                queuedClick = false;
            }
        }

        public static void Click (ClickTarget target, MouseButton button, Vector3 clickWorldPosition = default) {
            queuedClick = true;
            queuedClickParams = new ClickParams(Camera.main, target, clickWorldPosition, button);
        }

        #region Raycasting
        private static void CastRays () {
            if (!Application.isFocused || DisableMouse) {
                return;
            }

            // Check for movement
            var newMousePosition = Input.mousePosition;
            MouseIsMoving = (newMousePosition != oldMousePosition);
            oldMousePosition = newMousePosition;

            // Ray cast
            Profiler.BeginSample("Mouse Raycasting");
            HighlightParams highlightParams = GetRaycastTarget();

            if (!MouseIsInBounds) {
                highlightParams = HighlightParams.blank;
            }
            Profiler.EndSample();

            // Override
            var mouseTarget = highlightParams.Target;
            var appliedHighlightParams = highlightParams;
            if (InterfaceTargets.Dragged != null) {
                appliedHighlightParams.Target = InterfaceTargets.Dragged;
            }
            else if (GrabTarget.CurrentGrabbedInstance != null) {
                appliedHighlightParams.Target = GrabTarget.CurrentGrabbedInstance;
            }
            currentDragOverTarget = mouseTarget;

            // Selection
            Highlight(appliedHighlightParams);
            ProcessClick(mouseTarget, highlightParams.MouseWorldPosition);
        }

        private static HighlightParams GetRaycastTarget () {
            if (!MouseIsInBounds) {
                return HighlightParams.blank;
            }

            Raycaster.CastAndCollide(out var target, out var targetPoint);

            // Return highlight
            if (target == null) {
                return HighlightParams.blank;
            }
            var heldButton = MouseButton.None;
            if (pressedClickParams.Target == target) {
                heldButton = pressedClickParams.ClickButton;
            }
            return new HighlightParams(Camera.main, target, targetPoint, heldButton);
        }
        #endregion

        #region Clicking
        private static void ProcessClick (MouseTarget mouseTarget, Vector3 collisionPosition) {
            var pressButton = MouseButton.None;
            if (Input.GetMouseButtonDown((int)MouseButton.Left)) pressButton = MouseButton.Left;
            if (Input.GetMouseButtonDown((int)MouseButton.Right)) pressButton = MouseButton.Right;

            // Prepare drag params
            DragParams dragParams = new DragParams(Camera.main, InterfaceTargets.Dragged, mouseTarget,
                pressPixel, Input.mousePosition, pressedClickParams.ClickButton);

            // Mouse pressing
            if (!mouseIsPressed && pressButton != MouseButton.None && Time.time > lastClickTime + 0.05f) {
                mouseIsPressed = true;
                lastClickTime = Time.time;
                pressPixel = Input.mousePosition;
                pressPosition = collisionPosition;
                pressedClickParams = new ClickParams(Camera.main, mouseTarget as ClickTarget, collisionPosition, pressButton);

                // Start dragging
                var newDragTarget = mouseTarget as DragTarget;
                if (newDragTarget != null && newDragTarget.DraggingIsEnabled) {
                    dragParams = new DragParams(Camera.main, newDragTarget, mouseTarget,
                        pressPixel, pressPixel, pressedClickParams.ClickButton);
                    StartDrag(dragParams);
                }

                // Press
                if (pressedClickParams.Target != InterfaceTargets.Dragged) {
                    OnNewPress?.Invoke(pressedClickParams.Target);
                }
                var clickOnPress = pressedClickParams.Target?.ClickOnMouseDown == true;
                if (clickOnPress) {
                    pressedClickParams.HeldDuration = Time.time - lastClickTime;
                    Select(pressedClickParams);
                }
                return;
            }

            // Mouse releasing (+ Finish Dragging)
            if (mouseIsPressed && !Input.GetMouseButton((int)pressedClickParams.ClickButton)) {

                // Apply drag
                if (InterfaceTargets.Dragged != null) {
                    CompleteDrag(dragParams);
                }

                // Click
                var clickOnPress = pressedClickParams.Target?.ClickOnMouseDown == true;
                if (pressedClickParams.Target == mouseTarget && !clickOnPress) {
                    pressedClickParams.HeldDuration = Time.time - lastClickTime;
                    Select(pressedClickParams);
                }

                mouseIsPressed = false;
                pressedClickParams = ClickParams.blank;          
            }

            // Update or cancel drag
            var cancelDrag = InterfaceTargets.Dragged?.DraggingIsEnabled == false ||
                             (dragParams.DragButton == MouseButton.Left && Input.GetMouseButtonDown((int)MouseButton.Right)) ||
                             (dragParams.DragButton == MouseButton.Right && Input.GetMouseButtonDown((int)MouseButton.Left));

            if (mouseIsPressed && InterfaceTargets.Dragged != null && !cancelDrag) {
                UpdateDrag(dragParams);
            } else {
                CancelDrag();
            }
        }

        private static void InstantClick (ClickParams clickParams) {
            CancelDrag();
            Select(clickParams);

            mouseIsPressed = false;
            pressedClickParams = ClickParams.blank;
        }
        #endregion

        #region Events
        private static void Highlight (HighlightParams highlightParams) {
            FruityUIManager.Queue(new HighlightEvent {
                Params = highlightParams
            });
        }

        private static void Select (ClickParams clickParams) {
            FruityUIManager.Queue(new ClickEvent {
                Params = clickParams
            });
        }
        
        private static void StartDrag(DragParams dragParams) {
            FruityUIManager.Queue(new StartDragEvent {
                Params = dragParams
            });
        }

        private static void UpdateDrag (DragParams dragParams) {
            FruityUIManager.Queue(new UpdateDragEvent {
                Params = dragParams
            });
        }

        private static void CompleteDrag (DragParams dragParams) {
            FruityUIManager.Queue(new EndDragEvent {
                Params = dragParams,
                WasCancelled = false
            });
        }

        private static void CancelDrag () {
            FruityUIManager.Queue(new EndDragEvent {
                Params = default,
                WasCancelled = true
            });
        }
        #endregion

    }
}