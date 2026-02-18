using LycheeLabs.FruityInterface.Animation;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// An extensions class for easily instantiating new JuicyAnimations
    /// </summary>
    public static class JuicyAnimations {

        public static void Squash (this JuicyAnimator animator, float sizeScale = 1f, float speedScale = 1f, int cycles = 3, Tween tween = null) {
            animator?.Play(new SquashAnimation(sizeScale, speedScale, cycles, tween));
        }

        public static void Bulge (this JuicyAnimator animator, float sizeScale = 1f, float speedScale = 1f, Tween tween = null) {
            animator?.Play(new BulgeAnimation(sizeScale, speedScale, tween));
        }

        public static void Nudge (this JuicyAnimator animator, Vector3 direction, float sizeScale = 1f, float speedScale = 1f) {
            animator?.Play(new NudgeAnimation(direction, sizeScale, speedScale));
        }

        public static void Wiggle(this JuicyAnimator animator, float sizeScale = 1f, float speedScale = 1f) {
            animator?.Play(new WiggleAnimation(sizeScale, speedScale));
        }

        public static void LongWiggle(this JuicyAnimator animator, float sizeScale = 1f, float speedScale = 1f) {
            animator?.Play(new LongWiggleAnimation(sizeScale, speedScale));
        }

        public static void SmoothBulge (this JuicyAnimator animator, float sizeScale = 1f, float speedScale = 1f) {
            Bulge(animator, sizeScale, speedScale * 0.5f, Tweens.EaseInOutQuadTween);
        }

        public static void SquashBulge (this JuicyAnimator animator, float sizeScale = 1f, float speedScale = 1f) {
            SmoothBulge(animator, sizeScale, speedScale);
            Squash(animator, sizeScale * 1.2f, speedScale * 0.8f, 3);
        }

        public static void CrazySquash(this JuicyAnimator animator, float sizeScale = 1f, float speedScale = 1f) {
            Squash(animator, sizeScale * 2f, speedScale * 0.66f, 6);
        }

        public static void ExtraCrazySquash (this JuicyAnimator animator, float sizeScale = 1f, float speedScale = 1f) {
            Squash(animator, sizeScale * 2f, speedScale * 0.33f, 12);
        }

        public static void FlattenSquash(this JuicyAnimator animator, float speedScale = 1f) {
            animator?.Play(new FlattenSquashAnimation(speedScale));
        }

    }

}