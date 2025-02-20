using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public enum ScreenAspect {
         ULTRATALL, TALLSCREEN, STANDARD, WIDESCREEN, ULTRAWIDE,
    }

    public static class InterfaceConfig {

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

        // Input
        public static InterfaceNode LockedNode { get; private set; }
        public static bool DisableInput { get; set; }
        public static Plane MousePlane = new Plane(Vector3.up, Vector3.zero);

        static InterfaceConfig () {
            // Safe initial values
            WindowCanvasSize = Vector2.one;
            WindowAspectRatio = 1;
            BoxedCanvasSize = Vector2.one;
            BoxedAspectRatio = 1;
            CameraScaling = 1;
            UIScaling = 1;
        }

        public static void Update (ScreenAspect minAspect = ScreenAspect.STANDARD, ScreenAspect maxAspect = ScreenAspect.WIDESCREEN) {
            UpdateCanvas(minAspect, maxAspect);
        }

        private static void UpdateCanvas (ScreenAspect minAspect, ScreenAspect maxAspect) {
            float screenWidth = Mathf.Max(Screen.width, 1);
            float screenHeight = Mathf.Max(Screen.height, 1);

            // Clamp aspect ratio
            var minRatio = minAspect.Calculate();
            var maxRatio = maxAspect.Calculate();
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

        public static void LockUI (InterfaceNode newLockedRoot) {
            LockedNode = newLockedRoot;
        }

        public static void UnlockUI () {
            LockedNode = null;
        }

        public static Vector3 MouseToWorldPoint () {
            return ScreenPointToWorldPoint(Input.mousePosition);
        }

        public static Vector3 ScreenPointToWorldPoint (Vector3 mousePosition) {
            if (Camera.main == null) return default;
            return Camera.main.ScreenToWorldPoint(mousePosition).IntersectWithPlane(MousePlane);
        }

        public static Ray ScreenPointToRay (Vector3 mousePosition) {
            if (Camera.main == null) return default;
            return Camera.main.ScreenPointToRay(mousePosition);
        }

        public static Vector3 IntersectWithPlane(this Vector3 vector) {
            return IntersectWithPlane(vector, MousePlane);
        }

        public static Vector3 IntersectWithPlane (this Vector3 vector, Plane plane) {

            // Already intersecting?
            if (vector.y == 0 || Camera.main == null) {
                return vector;
            }


            //Project the ray forwards
            float collisionDistance = 0;
            Ray ray = new Ray(vector, Camera.main.transform.forward);
            if (plane.Raycast(ray, out collisionDistance)) {
                // get the hit point:
                return ray.GetPoint(collisionDistance);
            }

            //Project the ray backwards
            ray = new Ray(vector, -Camera.main.transform.forward);
            if (plane.Raycast(ray, out collisionDistance)) {
                // get the hit point:
                return ray.GetPoint(collisionDistance);
            }

            // Failure! Get approximate projection to infinity
            return ray.GetPoint(99999);
        }

        public static float Calculate (this ScreenAspect aspect) {
            if (aspect == ScreenAspect.WIDESCREEN) return 16f / 9f;
            if (aspect == ScreenAspect.TALLSCREEN) return 9f / 16f;
            if (aspect == ScreenAspect.ULTRAWIDE) return 21f / 9f;
            if (aspect == ScreenAspect.ULTRATALL) return 9f / 21f;
            return 4f / 3f;
        }

    }

}