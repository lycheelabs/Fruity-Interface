using UnityEngine;

namespace LycheeLabs.InputTree {

    public class ScreenBounds {

        // Canvas
        public Vector2 WindowCanvasSize { get; private set; }
        public float WindowAspectRatio { get; private set; }

        // Letterboxing
        public Vector2 BoxedCanvasSize { get; private set; }
        public float BoxedAspectRatio { get; private set; }
        public float CameraScaling { get; private set; }
        public float UIScaling { get; private set; }

        public Vector2 LetterboxOffset => new Vector2(LetterboxWidth, LetterboxHeight) / 2f;
        public float LetterboxWidth { get; private set; }
        public float LetterboxHeight { get; private set; }
        
        public ScreenBounds() {
            // Safe initial values
            WindowCanvasSize = Vector2.one;
            WindowAspectRatio = 1;
            BoxedCanvasSize = Vector2.one;
            BoxedAspectRatio = 1;
            CameraScaling = 1;
            UIScaling = 1;
        }

        public void Update (float minAspect = Interface.DefaultMinAspect, float maxAspect = Interface.DefaultMaxAspect) {
            ApplySize(Screen.width, Screen.height, minAspect, maxAspect);
        }

        internal void ApplySize (float width, float height, float minAspect, float maxAspect) {
            float screenWidth = Mathf.Max(width, 1);
            float screenHeight = Mathf.Max(height, 1);

            // Clamp aspect ratio
            var minRatio = minAspect;
            var maxRatio = maxAspect;
            WindowAspectRatio = screenWidth / screenHeight;
            BoxedAspectRatio = Mathf.Clamp(WindowAspectRatio, minRatio, maxRatio);
            BoxedCanvasSize = new Vector2(1080f * BoxedAspectRatio, 1080f);

            // Calculate UI scaling for aspect ratio
            float screenScale = BoxedCanvasSize.y / screenHeight;
            if (WindowAspectRatio <= minRatio) {
                screenScale = BoxedCanvasSize.x / screenWidth;
            }

            // Calculate letterboxes
            LetterboxWidth = screenWidth * screenScale - BoxedCanvasSize.x;
            LetterboxHeight = screenHeight * screenScale - BoxedCanvasSize.y;
            WindowCanvasSize = BoxedCanvasSize + new Vector2(LetterboxWidth, LetterboxHeight);
            UIScaling = WindowCanvasSize.y / screenHeight;

            // Narrow windows require orthographic camera resize
            CameraScaling = WindowCanvasSize.y / BoxedCanvasSize.y;

        }

    }

}
