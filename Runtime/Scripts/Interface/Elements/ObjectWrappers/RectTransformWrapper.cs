using UnityEngine;
using System;

namespace LycheeLabs.FruityInterface.Elements {

    // Must replace localposition with anchoredposition, for correct positioning with anchors.
    public class RectTransformWrapper : TransformWrapper {

        protected readonly RectTransform rectTransform;

        public RectTransformWrapper (GameObject gameObject) : base(gameObject) {
            rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform == null) {
                throw new NullReferenceException("No RectTransform component found");
            }
        }

        public RectTransformWrapper (Transform transform) : this(transform.gameObject) { }

        // Replace position
        public override Vector3 LocalPosition {
            get { return rectTransform.anchoredPosition3D; }
            set { rectTransform.anchoredPosition3D = value; }
        }

        /*public override UIPosition UIPosition {
            get { return new UIPosition2D(rectTransform.position); }
        }*/

        // Replace position
        public int ZOffset {
            set {
                Vector3 pos = rectTransform.localPosition;
                Vector3 offset = new Vector3(pos.x, pos.y, value);
                rectTransform.anchoredPosition3D = offset;
            }
        }

        public Vector2 Anchor {
            set {
                rectTransform.anchorMin = value;
                rectTransform.anchorMax = value;
            }
        }

        public void SetAnchors (Vector2 min, Vector2 max) {
            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
        }

        public void SetFillParent () {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
        }

        public Vector2 Pivot {
            set { rectTransform.pivot = value; }
            get { return rectTransform.pivot; }
        }

        public Vector2 RectSize {
            get { return rectTransform.sizeDelta; }
            set { rectTransform.sizeDelta = value; }
        }

        public RectTransform RectTransform {
            get { return rectTransform; }
        }

    }

}