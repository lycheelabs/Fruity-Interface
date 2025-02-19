using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public enum Anchor {
        BottomLeft,
        Bottom,
        BottomRight,

        Left,
        Center,
        Right,

        TopLeft,
        Top,
        TopRight,
    }

    public static class AnchorExtensions {

        private static Vector2[] vectors;

        static AnchorExtensions () {
            vectors = new Vector2[System.Enum.GetValues(typeof(Anchor)).Length];

            vectors[(int)Anchor.BottomLeft] = new Vector2(0, 0);
            vectors[(int)Anchor.Bottom] = new Vector2(0.5f, 0);
            vectors[(int)Anchor.BottomRight] = new Vector2(1f, 0);

            vectors[(int)Anchor.Left] = new Vector2(0, 0.5f);
            vectors[(int)Anchor.Center] = new Vector2(0.5f, 0.5f);
            vectors[(int)Anchor.Right] = new Vector2(1f, 0.5f);

            vectors[(int)Anchor.TopLeft] = new Vector2(0, 1f);
            vectors[(int)Anchor.Top] = new Vector2(0.5f, 1f);
            vectors[(int)Anchor.TopRight] = new Vector2(1f, 1f);
        }

        public static Vector2 ToVector (this Anchor anchor) {
            return vectors[(int)anchor];
        }

        public static void SetAnchor (this RectTransform transform, Anchor newAnchor) {
            var vector = newAnchor.ToVector();
            transform.anchorMin = vector;
            transform.anchorMax = vector;
        }

        public static void SetAnchorAndPosition (this RectTransform transform, Vector3 newPosition, Anchor newAnchor = Anchor.Center) {
            var vector = newAnchor.ToVector();
            transform.anchorMin = vector;
            transform.anchorMax = vector;

            transform.anchoredPosition = newPosition;
        }

    }

}