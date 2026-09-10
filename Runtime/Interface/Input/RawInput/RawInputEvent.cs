using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// A device-independent pointer transition captured before UI processing.
    /// </summary>
    internal readonly struct RawInputEvent {

        public RawInputEvent(
            RawInputEventType type,
            int pointerId,
            MouseButton button,
            Vector2 screenPosition,
            Vector2 scrollDelta,
            double time) {
            Type = type;
            PointerId = pointerId;
            Button = button;
            ScreenPosition = screenPosition;
            ScrollDelta = scrollDelta;
            Time = time;
        }

        public RawInputEventType Type { get; }
        public int PointerId { get; }
        public MouseButton Button { get; }
        public Vector2 ScreenPosition { get; }
        public Vector2 ScrollDelta { get; }
        public double Time { get; }

        public static RawInputEvent Move(int pointerId, Vector2 screenPosition, double time) {
            return new RawInputEvent(
                RawInputEventType.Move,
                pointerId,
                MouseButton.None,
                screenPosition,
                Vector2.zero,
                time);
        }

        public static RawInputEvent ButtonDown(
            int pointerId,
            MouseButton button,
            Vector2 screenPosition,
            double time) {
            return new RawInputEvent(
                RawInputEventType.ButtonDown,
                pointerId,
                button,
                screenPosition,
                Vector2.zero,
                time);
        }

        public static RawInputEvent ButtonUp(
            int pointerId,
            MouseButton button,
            Vector2 screenPosition,
            double time) {
            return new RawInputEvent(
                RawInputEventType.ButtonUp,
                pointerId,
                button,
                screenPosition,
                Vector2.zero,
                time);
        }

        public static RawInputEvent Scroll(
            int pointerId,
            Vector2 screenPosition,
            Vector2 scrollDelta,
            double time) {
            return new RawInputEvent(
                RawInputEventType.Scroll,
                pointerId,
                MouseButton.None,
                screenPosition,
                scrollDelta,
                time);
        }

        public static RawInputEvent Cancel(
            int pointerId,
            Vector2 screenPosition,
            double time) {
            return new RawInputEvent(
                RawInputEventType.Cancel,
                pointerId,
                MouseButton.None,
                screenPosition,
                Vector2.zero,
                time);
        }

    }

}
