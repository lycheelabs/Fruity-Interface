using LycheeLabs.FruityInterface.Animation;
using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    // TODO: Reveal RectTransform properties like anchor and pivot

    /// <summary>
    /// This MonoBehaviour allows the easy layering of simple code-based animations onto a transform. 
    /// The animations will be applied relative to the basePosition, baseRotation and baseScale properties, and applied in the order they were queued. 
    /// Note that the transform's position, rotation and scale will be overwritten every frame!
    /// </summary>
    [ExecuteAlways]
    public class JuicyAnimator : MonoBehaviour {

        private RectTransform rectTransform;

        private void Awake () {
            rectTransform = GetComponent<RectTransform>();
            BasePosition = (rectTransform == null) ? transform.localPosition : rectTransform.anchoredPosition;
            BaseRotation = transform.localEulerAngles;
            BaseScale = transform.localScale;
        }

        private void OnDestroy () {
            transform.hideFlags = HideFlags.None;
        }

        /// <summary> Animations will be applied relative to this localPosition vector, then the transform will be overwritten with it. </summary>
        public Vector3 BasePosition { 
            get { return basePosition; } 
            set { basePosition = value; isIdle = false; }
        }
        
        /// <summary> Animations will be applied relative to this localEulerAngles vector, then the transform will be overwritten with it. </summary>
        public Vector3 BaseRotation {
            get { return baseRotation; }
            set { baseRotation = value; isIdle = false; }
        }
        
        /// <summary> Animations will be applied relative to this localScale vector, then the transform will be overwritten with it. </summary>
        public Vector3 BaseScale {
            get { return baseScale; }
            set { baseScale = value; isIdle = false; }
        }

        public bool IsIdle => isIdle;

        /// <summary> Affects the deltaTime of all my animations. </summary>
        public float timeScaling = 1f;

        [SerializeField] private Vector3 basePosition;
        [SerializeField] private Vector3 baseRotation;
        [SerializeField] private Vector3 baseScale = Vector3.one;

        private JuicyAnimation BaseAnimation;
        private List<JuicyAnimation> LayeredAnimations = new List<JuicyAnimation>();
        private bool isIdle;

        public bool IsFull(int maxCount) {
            return LayeredAnimations.Count >= maxCount;
        }

        public void Play (JuicyAnimation animation) {
            if (animation.Mode == JuicyAnimationMode.BASE) {
                BaseAnimation = animation;
            } else {
                LayeredAnimations.Add(animation);
            }
            isIdle = false;
        }

        void LateUpdate() {
            if (isIdle) return;

            var transformData = new TransformData {
                position = Vector3.zero,
                rotation = Vector3.zero,
                scale = Vector3.one,
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
        }

        protected virtual void Apply (ref TransformData transformData) {
            transformData.position += basePosition;
            transformData.scale = Vector3.Scale(transformData.scale, baseScale);
            transformData.rotation += baseRotation;

            if (rectTransform == null) {
                transform.localPosition = transformData.position;
            } else {
                rectTransform.anchoredPosition = transformData.position;
            }
            transform.localEulerAngles = transformData.rotation;
            transform.localScale = transformData.scale;
        }

        private void OnValidate() {
            isIdle = false;
        }

    }

}