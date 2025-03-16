using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class InterfaceConfig {

        // Canvas
        public static Vector2 WindowCanvasSize { get; private set; }
        public static float WindowAspectRatio { get; private set; }

        // Letterboxing
        public static Vector2 BoxedCanvasSize { get; private set; }
        public static float BoxedAspectRatio { get; private set; }
        public static float CameraScaling { get; private set; }
        public static float UIScaling { get; private set; }

        public static Vector2 LetterboxOffset => new Vector2(LetterboxWidth, LetterboxHeight) / 2f;
        public static float LetterboxWidth { get; private set; }
        public static float LetterboxHeight { get; private set; }
        
        public InterfaceConfig () {
            // Safe initial values
            WindowCanvasSize = Vector2.one;
            WindowAspectRatio = 1;
            BoxedCanvasSize = Vector2.one;
            BoxedAspectRatio = 1;
            CameraScaling = 1;
            UIScaling = 1;
        }

        public void Update (AspectRatio minAspect = AspectRatio.STANDARD, AspectRatio maxAspect = AspectRatio.WIDESCREEN) {
            float screenWidth = Mathf.Max(Screen.width, 1);
            float screenHeight = Mathf.Max(Screen.height, 1);

            // Clamp aspect ratio
            var minRatio = minAspect.Value();
            var maxRatio = maxAspect.Value();
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