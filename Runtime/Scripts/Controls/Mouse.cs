using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace LycheeLabs.FruityInterface {

    public static class Mouse {

        /// <summary> Log all mouse events for debug purposes </summary>
        public static readonly bool LOG_EVENTS = false;

        /// <summary> The maxiumum number of raycast hits that will be considered along a single ray </summary>
        public static readonly int MAX_HITS = 100;

        /// <summary> The maxiumum raycasting distance </summary>
        public static readonly float MAX_DISTANCE = 9999;

        private static RaycastHit[] RaycastBuffer = new RaycastHit[MAX_HITS];

        public static bool MouseIsInBounds =>
            Input.mousePosition.x >= 1  && 
            Input.mousePosition.y >= 1 && 
            Input.mousePosition.x < (Screen.width - 1) && 
            Input.mousePosition.y < (Screen.height - 1);

        public static Vector2 MouseScreenPosition => ((Vector2)Input.mousePosition * UIConfig.UIScaling) - UIConfig.LetterboxOffset;
        public static Vector3 MouseWorldPosition => UIConfig.MouseToWorldPoint();

        public static bool MouseIsMoving { get; private set; }
        public static bool MouseIsDragging => currentDragTarget != null;
        public static bool DisableMouse { get; set; }
        public static bool IsIdle => currentHighlightTarget == null && currentDragTarget == null;
        public static MouseTarget HighlightTarget => currentHighlightTarget;
        public static MouseTarget DragOverTarget => currentDragOverTarget;

        // Targets
        private static MouseTarget currentHighlightTarget;
        private static ClickTarget currentSelectTarget;
        private static DragTarget currentDragTarget;
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

        public static void Update () {
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
            if (currentDragTarget != null) {
                appliedHighlightParams.Target = currentDragTarget;
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

            // Cast a ray, collecting all hits
            Ray ray = UIConfig.ScreenPointToRay(Input.mousePosition);
            int hitCount = Physics.RaycastNonAlloc(ray, RaycastBuffer, MAX_DISTANCE);

            // Extract closest enabled MouseTarget
            MouseTarget target = null;
            float targetDistance = MAX_DISTANCE;
            Vector3 targetPoint = default;

            for (int i = 0; i < hitCount; i++) {
                var hit = RaycastBuffer[i];
                if (hit.distance < targetDistance) {
                    var node = hit.collider.gameObject.GetComponent<UINode>();
                    var nodeTarget = node as MouseTarget;
                    nodeTarget = Resolve(nodeTarget, hit.point);

                    if (node != null && nodeTarget != null && node.InputEnabledInHierarchy) {
                        target = nodeTarget;
                        targetDistance = hit.distance;
                        targetPoint = hit.point;
                    }
                }
            }

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
            DragParams dragParams = new DragParams(Camera.main, currentDragTarget, mouseTarget,
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
                    currentDragTarget = newDragTarget;
                    dragParams = new DragParams(Camera.main, currentDragTarget, mouseTarget,
                        pressPixel, pressPixel, pressedClickParams.ClickButton);
                    StartDrag(newDragTarget, dragParams);
                }

                // Press
                if (pressedClickParams.Target != currentSelectTarget) {
                    OnNewPress?.Invoke(pressedClickParams.Target);
                }
                var clickOnPress = pressedClickParams.Target?.ClickOnMouseDown == true;
                if (clickOnPress) {
                    pressedClickParams.HeldDuration = Time.time - lastClickTime;
                    Select(pressedClickParams);
                }
                
            }

            // Mouse releasing (+ Finish Dragging)
            if (mouseIsPressed && !Input.GetMouseButton((int)pressedClickParams.ClickButton)) {

                // Apply drag
                if (currentDragTarget != null) {
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
            var cancelDrag = currentDragTarget?.DraggingIsEnabled == false ||
                (dragParams.DragButton == MouseButton.Left && Input.GetMouseButtonDown((int)MouseButton.Right)) ||
                (dragParams.DragButton == MouseButton.Right && Input.GetMouseButtonDown((int)MouseButton.Left));

            if (mouseIsPressed && currentDragTarget != null && !cancelDrag) {
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

        private static List<MouseTarget> resolutionStack = new List<MouseTarget>();
        private static MouseTarget Resolve (MouseTarget target, Vector3 point) {
            var nextTarget = target;
            resolutionStack.Clear();

            while (nextTarget != null && !resolutionStack.Contains(nextTarget)) {
                resolutionStack.Add(nextTarget);
                target = nextTarget;
                nextTarget = target.ResolveTarget(point);
            }
            return target;
        }
        #endregion

        #region Events
        private static void Highlight (HighlightParams highlightParams) {
            var newTarget = highlightParams.Target;
            var firstFrame = (newTarget != currentHighlightTarget);

            if (firstFrame) {
                currentHighlightTarget?.MouseDehighlight();
                currentHighlightTarget = newTarget;
                if (LOG_EVENTS) {
                    Debug.Log("Highlight: " + newTarget);
                }
            }

            currentHighlightTarget?.MouseHighlight(firstFrame, highlightParams);
        }

        private static void Select (ClickParams clickParams) {
            selectionLock = true;
            var newTarget = clickParams.Target;

            // Try unclicking current target
            if (currentSelectTarget != null && newTarget != currentSelectTarget) {
                var unclicked = currentSelectTarget.TryMouseUnclick(clickParams);
                if (!unclicked) {
                    if (LOG_EVENTS) {
                        Debug.Log("Unclick blocked: " + currentSelectTarget);
                    }
                    selectionLock = false;
                    return;
                }
            }

            if (LOG_EVENTS) {
                Debug.Log("Click: " + newTarget);
            }

            OnNewClick?.Invoke(pressedClickParams.Target);

            currentSelectTarget = newTarget;
            currentSelectTarget?.MouseClick(clickParams);
            selectionLock = false;

        }

        public static void TrySelect (ClickTarget target, MouseButton button) {
            if (!selectionLock) {
                Select(new ClickParams(Camera.main, target, default, button));
            }
        }

        private static void StartDrag(DragTarget newTarget, DragParams dragParams) {
            currentDragTarget = newTarget;
            if (LOG_EVENTS) {
                Debug.Log("Dragging: " + newTarget);
            }
            currentDragTarget.StartMouseDrag(dragParams);
        }

        private static void UpdateDrag (DragParams dragParams) {
            if (currentDragTarget != null) {
                currentDragTarget.UpdateMouseDrag(dragParams);
            }
        }

        private static void CompleteDrag (DragParams dragParams) {
            if (currentDragTarget != null) {
                if (LOG_EVENTS) {
                    Debug.Log("Dragged: " + currentDragTarget);
                }
                currentDragTarget.CompleteMouseDrag(dragParams);
                currentDragTarget = null;
            }
        }

        private static void CancelDrag () {
            if (currentDragTarget != null) {
                if (LOG_EVENTS) {
                    Debug.Log("Drag cancelled: " + currentDragTarget);
                }
                currentDragTarget.CancelMouseDrag();
                currentDragTarget = null;
            }
        }
        #endregion

    }

}