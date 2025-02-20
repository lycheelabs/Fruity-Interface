using LycheeLabs.FruityInterface.Animation;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary> Abstract implementation of a simple button. </summary>
    public abstract class InterfaceButton : LayoutNode, ClickTarget {

        public float BaseScale = 1f;
        public float AnimationScaling = 1f;

        public bool IsHighlighted { get; protected set; }
        private float highlightTween;

        public bool IsHeld { get; private set; }
        private float heldTween;

        public JuicyAnimator ButtonAnimator;

        private Vector3 basePosition;
        public Vector3 BasePosition {
            get => basePosition;
            set { basePosition = value; rectTransform.anchoredPosition = basePosition + offset; }
        }

        private Vector3 offset;
        public Vector3 Offset {
            get => offset;
            set { offset = value; rectTransform.anchoredPosition = basePosition + offset; }
        }

        public Anchor Anchor {
            set { rectTransform.SetAnchor(value); }
        }

        private void Update () {
            highlightTween = highlightTween.MoveTowards(IsHighlighted, 8);
            heldTween = heldTween.MoveTowards(IsHeld, 8);
            var scaleShift = 0.1f * Tweens.EaseOutQuad(highlightTween) - 0.07f * Tweens.EaseOutQuad(heldTween);
            float highlightScale = 1 + scaleShift * AnimationScaling;
            ButtonAnimator.BaseScale = Vector3.one * highlightScale * BaseScale;
            OnUpdate();
        }

        public void MouseHighlight (bool firstFrame, HighlightParams highlightParams) {
            IsHighlighted = true;
            IsHeld = highlightParams.HeldButton != MouseButton.None;
            OnHighlight(firstFrame, highlightParams);
        }


        public void MouseDehighlight () {
            IsHighlighted = false;
            IsHeld = false;
            OnDehighlight();
        }

        protected virtual void OnUpdate() { }
        public abstract void MouseClick (ClickParams clickParams);
        protected abstract void OnHighlight (bool firstFrame, HighlightParams highlightParams);
        protected abstract void OnDehighlight ();

    }

}