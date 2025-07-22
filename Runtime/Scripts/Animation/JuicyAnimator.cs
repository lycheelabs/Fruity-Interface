using LycheeLabs.FruityInterface.Animation;
using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    // TODO: Reveal RectTransform properties like anchor and pivot

    /// <summary>
    /// This MonoBehaviour allows the easy layering of simple code-based animations onto a transform. 
    /// The animations will be applied relative to the basePosition, baseRotation and baseScale properties, 
    /// and applied in the order they were queued. 
    /// Note: This component overrides the Transform component!
    /// </summary>
    [ExecuteAlways]
    public class JuicyAnimator : MonoBehaviour {

        private RectTransform rectTransform;

        private void Reset() {
            basePosition = (rectTransform == null) ? transform.localPosition : rectTransform.anchoredPosition;
            baseRotation = transform.localEulerAngles;
            baseScale = transform.localScale;
        }

        private void OnEnable () {
            rectTransform = GetComponent<RectTransform>();
            isIdle = false;
        }

        private void OnDisable() {
            transform.hideFlags = HideFlags.None;
        }

        /// <summary> The Transform's position value is reset to this value every frame, before animations are applied. </summary>
        public Vector3 BasePosition { 
            get { return basePosition; }
            set { if (basePosition != value) { basePosition = value; isIdle = false; } }
        }

        /// <summary> The Transform's rotation value is reset to this value every frame, before animations are applied. </summary>
        public Vector3 BaseRotation {
            get { return baseRotation; }
            set { if (baseRotation != value) { baseRotation = value; isIdle = false; } }
        }

        /// <summary> The Transform's scale value is reset to this value every frame, before animations are applied. </summary>
        public Vector3 BaseScale {
            get { return baseScale; }
            set { if (baseScale != value) { baseScale = value; isIdle = false; } }
        }

        public bool IsIdle => isIdle;

        /// <summary> Affects the deltaTime of all my animations. </summary>
        public float timeScaling = 1f;

        [SerializeField] private Vector3 basePosition;
        [SerializeField] private Vector3 baseRotation;
        [SerializeField] private Vector3 baseScale = Vector3.one;

        private Vector3 animatePosition;
        private Vector3 animateRotation;
        private Vector3 animateScale = Vector3.one;

        private JuicyAnimation BaseAnimation;
        private List<JuicyAnimation> LayeredAnimations = new List<JuicyAnimation>();
        private bool isIdle;

        public bool IsFull(int maxCount) {
            return LayeredAnimations.Count >= maxCount;
        }

        /// <summary> Adjusts the BasePosition for one frame. </summary>
        public void OverlayPosition(Vector3 translation) {
            animatePosition += translation;
            isIdle = false;
        }

        /// <summary> Adjusts the BaseRotation for one frame. </summary>
        public void OverlayRotation(Vector3 rotation) {
            animateRotation += rotation;
            isIdle = false;
        }

        /// <summary> Adjusts the BaseScale for one frame. </summary>
        public void OverlayScale(Vector3 scale) {
            animateScale = Vector3.Scale(animateScale, scale);
            isIdle = false;
        }

        /// <summary> Queue an animation that plays each frame until completion. </summary>
        public void Play (JuicyAnimation animation) {
            if (animation.Mode == JuicyAnimationMode.BASE) {
                BaseAnimation = animation;
            } else {
                LayeredAnimations.Add(animation);
            }
            isIdle = false;
        }

        private void LateUpdate() {

            if (!Application.isPlaying) {
                basePosition = (rectTransform == null) ? transform.localPosition : rectTransform.anchoredPosition;
                baseRotation = transform.localEulerAngles;
                baseScale = transform.localScale;
                return;
            }

            if (isIdle) return;

            var transformData = new TransformData {
                position = animatePosition,
                rotation = animateRotation,
                scale = animateScale,
            };

            // Base animation
            if (BaseAnimation != null) {
                BaseAnimation.Update(ref transformData, timeScaling);
                if (BaseAnimation.ReadyToFinish) {
                    BaseAnimation = null;
                }
            }

            // Layered animations
            for (int i = LayeredAnimations.Count - 1; i >= 0; i--) {
                var animation = LayeredAnimations[i];
                animation.Update(ref transformData, timeScaling);
                if (animation.ReadyToFinish) {
                    LayeredAnimations.RemoveAt(i);
                }
            }

            // Apply to transform
            Apply (ref transformData);
            isIdle = BaseAnimation == null && LayeredAnimations.Count == 0;

            // Clear animated state
            animatePosition = Vector3.zero;
            animateRotation = Vector3.zero;
            animateScale = Vector3.one;

        }

        protected virtual void Apply (ref TransformData transformData) {
            transformData.position += basePosition;
            transformData.rotation += baseRotation;
            transformData.scale = Vector3.Scale(transformData.scale, baseScale);

            SetPosition(transformData.position);
            transform.localEulerAngles = transformData.rotation;
            transform.localScale = Attenuate(transformData.scale);
        }

        private void SetPosition (Vector3 newPosition) {
            if (rectTransform == null) {
                transform.localPosition = newPosition;
            } else {
                rectTransform.anchoredPosition = newPosition;
            }
        }

        private Vector3 Attenuate (Vector3 scale, float k = 4f, float maxScale = 1.5f) {
            return new Vector3(
                Attenuate(scale.x, k, maxScale),
                Attenuate(scale.y, k, maxScale),
                Attenuate(scale.z, k, maxScale));
        }

        private float Attenuate(float input, float k, float maxScale) {
            if (input <= 1f) {
                return input;
            }
            else {
                return 1f + (1f - 1f / (1f + (input - 1f) * k)) * (maxScale - 1f);
            }
        }

    }

}