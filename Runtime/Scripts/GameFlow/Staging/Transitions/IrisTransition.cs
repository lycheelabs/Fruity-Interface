using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Flow {

    public class IrisTransition : ScreenTransitionNode {

        public const string TargetKey = "Target";
        public const string SpeedKey = "Speed";
        public const string PauseKey = "Pause";

        // --------------------------------------

        public Image image;
        private Material material;
        private Color colorA;
        private Color colorB;

        protected override float TransitionSpeed => 1.5f * speedScale;
        private float speedScale;

        private float adjustedTween;
        private bool pausing;
        private float pausePoint;
        private bool pauseCleared;
        private float pauseTween;

        private void Start() {
            InitMaterial();
            image.enabled = false;
            SetColor(Color.black);
        }

        private void InitMaterial () {
            material = new Material(image.material);
            image.material = material;
            material.SetFloat("_Tween", 0);
        }

        public override void SetColor(Color color) {
            colorA = color;
            colorB = color;
        }

        protected override void Configure(ScreenTransitionConfig config) {
            material.SetVector("_Origin", new Vector2(0.5f, 0.5f));
            speedScale = 1f;
            pausing = false;

            if (config.TryGetConfig(SpeedKey, out var speed)) {
                speedScale = (float)speed;
            }
            if (config.TryGetConfig(PauseKey, out var pausePoint)) {
                PauseAt((float)pausePoint);
            }
            if (config.TryGetConfig(TargetKey, out var target)) {
                if (target is ScreenPosition targetPosition) {
                    material.SetVector("_Origin", targetPosition.RawViewportVector());
                }
            }
        }

        protected override void Refresh(bool isEntering, float tween) {
            if (pausing) {
                var target = isEntering ? pausePoint : 1 - pausePoint;
                pauseTween = pauseTween.MoveTowards(!pauseCleared, TransitionSpeed * 2);
                adjustedTween = Mathf.Lerp(tween, target, Tweens.EaseInOutQuad(pauseTween));

                if (!isEntering && !pauseCleared) adjustedTween = Mathf.Max(adjustedTween,
                    1 - (Tweens.EaseOutQuad(pauseTween) * pausePoint)); 

            } else {
                adjustedTween = Tweens.EaseOutQuad(tween);
            }

            if (image != null) {
                image.material.SetFloat("_Tween", adjustedTween);
                image.color = Color.Lerp(colorA, colorB, Tweens.EaseInOutQuad(tween));
                image.enabled = adjustedTween > 0;
            }
        }

        public void PauseAt (float pausePoint) {
            pausing = true;
            pauseCleared = false;
            this.pausePoint = pausePoint;
        }

        public void Unpause () {
            pauseCleared = true;
        }

        // Block completion when pausing
        protected override bool BlockingCompletion => 
            adjustedTween != 0 && adjustedTween != 1;

    }

}