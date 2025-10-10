using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public enum CameraFitMode { FitWidthAndHeight, FitHeightToLevelY, FitHeightToLevelZ }

    public class CameraPositioner {

        private bool isConfigured;

        private Vector3 cameraSize;
        private Vector3 cameraPosition;
        private float uiMarginTop;
        private float uiMarginBottom;
        private float uiMarginRight;
        private float uiMarginLeft;

        private Vector3 levelBottomCorner;
        private Vector3 levelTopCorner;
        private CameraFitMode fitMode;
        private float zoom = 1;

        private Vector3 fudgeOffset;

        private Vector3 appliedCameraSize;
        private Vector3 appliedCameraPosition;
        private float appliedZoom = 1;
        private bool shiftedThisFrame;

        private bool isFocused;
        private Vector3 focusPosition;
        private float focusZoom;
        private float focusTween;

        /// <summary> The world point at the center of the camera view </summary>
        public Vector3 WorldViewCentre => appliedCameraPosition;

        /// <summary> The size of the viewed area in worldspace units </summary>
        public Vector3 WorldViewSize => appliedCameraSize;

        /// <summary> The current zoom multiplier. </summary>
        public float AppliedZoom => appliedZoom;

        /// <summary> Focusing the camera moves it smoothly to a fixed position. </summary>
        public bool IsFocused { get; }

        /// <summary>
        /// Sets the boundaries of the level space
        /// </summary>
        public void SetLevelSpace(Vector3 bottomLeftCorner, Vector3 topRightCorner) {
            if (!CheckRect(bottomLeftCorner, topRightCorner)) {
                Debug.LogWarning("Invalid camera rect!");
                return;
            }

            levelBottomCorner = bottomLeftCorner;
            levelTopCorner = topRightCorner;
        }

        /// <summary>
        /// Sets the size of the viewed area in world space
        /// </summary>
        public void SetCameraSize(Vector3 cameraSize, CameraFitMode fitMode = CameraFitMode.FitWidthAndHeight) {
            if (cameraSize.x < 0 || cameraSize.y < 0 || cameraSize.z < 0) {
                Debug.LogWarning("Invalid camera size!");
                return;
            }
            isConfigured = true;
            this.cameraSize = cameraSize;
            this.fitMode = fitMode;
        }

        /// <summary>
        /// Sets the center point of the viewed area in world space
        /// </summary>
        public void SetCameraPosition(Vector3 position) {
            cameraPosition = position;

        }

        /// <summary>
        /// Moves the center point of the viewed area in world space
        /// </summary>
        public void ShiftCameraPosition(Vector3 shift) {
            if (!shiftedThisFrame) {
                cameraPosition = appliedCameraPosition;
            }
            cameraPosition += shift;
            shiftedThisFrame = true;
        }

        /// <summary>
        /// Sets the zoom factor of the camera. 1 = no zoom.
        /// </summary>
        public void SetCameraZoom(float zoom) {
            this.zoom = zoom;
        }

        /// <summary>
        /// Sets the size of the HUD margins, relative to a 1080 pixel window height
        /// </summary>
        public void SetUIMargins(float top, float bottom, float left, float right) {
            uiMarginTop = top;
            uiMarginBottom = bottom;
            uiMarginLeft = left;
            uiMarginRight = right;
        }

        public void SetFudgeOffset(Vector3 fudgeOffset) {
            this.fudgeOffset = fudgeOffset;
        }

        public void Focus(Vector3 worldPosition, float zoom = 1f) {
            isFocused = true;
            focusPosition = worldPosition;
            focusZoom = zoom;
        }

        public void ReleaseFocus(bool keepCameraPosition = false) {
            isFocused = false;
            if (keepCameraPosition) {
                cameraPosition = appliedCameraPosition;
            }
        }

        public void UpdateAndApply(Camera camera, float viewDistance = 60) {
            focusTween = focusTween.MoveTowards(isFocused, 0.75f);
            var focusEase = Tweens.EaseInOutCube(focusTween);

            if (isConfigured) {

                appliedZoom = Mathf.Lerp(zoom, focusZoom, focusEase);

                // Viewport of the game panel (the canvas size within the margins)
                CalculateViewport(out var viewportSize, out var viewportOffset);
                Vector3 fitSize = CalculateFitSize(viewportSize, 1);//, appliedZoom);

                // Size of the viewed world area (in camera space)
                Vector2 viewedCameraSize = camera.worldToCameraMatrix * fitSize / appliedZoom;
                if (viewedCameraSize.x <= 0 || viewedCameraSize.y <= 0) {
                    Debug.LogError("Invalid camera setup");
                    return;
                }

                // Viewport transformed into world space 
                var worldViewportW = viewedCameraSize.x / viewportSize.x;
                var worldViewportH = viewedCameraSize.y / viewportSize.y;
                var worldViewportX = viewportOffset.x * worldViewportW;
                var worldViewportY = viewportOffset.y * worldViewportH;

                // Scaling and shift
                var marginShift = camera.transform.right * worldViewportX + camera.transform.up * worldViewportY;
                var largestAspect = Mathf.Max(worldViewportW / ScreenBounds.BoxedAspectRatio, worldViewportH);
                var scaling = largestAspect / 2f * ScreenBounds.CameraScaling;

                // Clamping 
                var clampedCameraPosition = CalculateClamp(camera, fitSize);

                // Focus
                var finalCameraPosition = Vector3.Lerp(clampedCameraPosition, focusPosition, focusEase);

                // Apply to camera
                var position = finalCameraPosition + marginShift / 2f - camera.transform.forward * viewDistance;
                camera.transform.localPosition = position + fudgeOffset;
                camera.orthographicSize = scaling;

                appliedCameraSize = fitSize;
                appliedCameraPosition = finalCameraPosition;
                shiftedThisFrame = false;

            }
        }

        private void CalculateViewport(out Vector2 size, out Vector2 offset) {

            // Size of the boxed game canvas
            var canvasW = ScreenBounds.BoxedCanvasSize.x;
            var canvasH = ScreenBounds.BoxedCanvasSize.y;

            // Size of the game panel (the canvas size within the UI margins)
            var panelW = Mathf.Max(1, canvasW - uiMarginRight - uiMarginLeft);
            var panelH = Mathf.Max(1, canvasH - uiMarginTop - uiMarginBottom);
            var panelX = uiMarginRight - uiMarginLeft;
            var panelY = uiMarginTop - uiMarginBottom;

            // Viewport of the game panel
            size = new Vector2(panelW / canvasW, panelH / canvasH);
            offset = new Vector2(panelX / canvasW, panelY / canvasH);
        }

        private Vector3 CalculateFitSize(Vector2 viewportSize, float zoom) {
            var fitSize = cameraSize / zoom;
            if (fitMode == CameraFitMode.FitHeightToLevelY) {
                var cameraHeight = cameraSize.y / zoom;
                var cameraWidth = cameraHeight * ScreenBounds.BoxedAspectRatio;
                fitSize = new Vector3(cameraWidth * viewportSize.x, cameraHeight * viewportSize.y, 0);
            }
            if (fitMode == CameraFitMode.FitHeightToLevelZ) {
                var cameraHeight = cameraSize.z / zoom;
                var cameraWidth = cameraHeight * ScreenBounds.BoxedAspectRatio;
                fitSize = new Vector3(cameraWidth * viewportSize.x, 0, cameraHeight * viewportSize.y);
            }

            return fitSize;
        }

        private Vector3 CalculateClamp(Camera camera, Vector3 fitSize) {
            var clampSize = fitSize;

            var projectionY = (camera.worldToCameraMatrix * new Vector3(0, 1, 0)).y;
            if (projectionY != 0) clampSize.y /= projectionY;
            var projectionZ = (camera.worldToCameraMatrix * new Vector3(0, 0, 1)).y;
            if (projectionZ != 0) clampSize.z /= projectionZ;

            var bottomCorner = levelBottomCorner + clampSize / 2f;
            var topCorner = levelTopCorner - clampSize / 2f;

            if (bottomCorner.x > topCorner.x) {
                var mid = (bottomCorner.x + topCorner.x) / 2f;
                bottomCorner.x = mid;
                topCorner.x = mid;
            }
            if (bottomCorner.y > topCorner.y) {
                var mid = (bottomCorner.y + topCorner.y) / 2f;
                bottomCorner.y = mid;
                topCorner.y = mid;
            }
            if (bottomCorner.z > topCorner.z) {
                var mid = (bottomCorner.z + topCorner.z) / 2f;
                bottomCorner.z = mid;
                topCorner.z = mid;
            }

            var clampedCameraPosition = cameraPosition;
            clampedCameraPosition = Vector3.Max(clampedCameraPosition, bottomCorner);
            clampedCameraPosition = Vector3.Min(clampedCameraPosition, topCorner);
            return clampedCameraPosition;
        }

        private bool CheckRect(Vector3 bottomLeftCorner, Vector3 topRightCorner) {
            if (topRightCorner.x < bottomLeftCorner.x) return false;
            if (topRightCorner.y < bottomLeftCorner.y) return false;
            if (topRightCorner.z < bottomLeftCorner.z) return false;
            if (bottomLeftCorner == topRightCorner) return false;
            return true;
        }

    }

}