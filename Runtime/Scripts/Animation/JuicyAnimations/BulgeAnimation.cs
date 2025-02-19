using UnityEngine;

namespace LycheeLabs.FruityInterface.Animation {

    // Makes the transform bulge in the X and Y axes together
    public class BulgeAnimation : SimpleJuicyAnimation {

        private float value = 1f;
        private readonly float sizeScale;
        private readonly float speedScale;
        private readonly Tween tween;

        public BulgeAnimation (float sizeScale = 1f, float speedScale = 1f, Tween tween = null) {
            this.sizeScale = sizeScale * 0.15f;
            this.speedScale = Mathf.Max(speedScale, 0.1f) * 4f;
            this.tween = tween;
        }

        public override void Update (ref TransformData transform, float timeScaling) {
            value = value.MoveTowards(0, speedScale * timeScaling);
            var tweened = (tween != null) ? tween.ApplyInverted(value) : value;

            var squash = Mathf.Sin(tweened * Mathf.PI) * tweened * sizeScale;
            var squashScale = new Vector3(1 + squash, 1 + squash, 1);

            var existingScale = transform.scale;
            transform.scale = new Vector3(existingScale.x * squashScale.x, existingScale.y * squashScale.y, existingScale.z);
        }

        public override bool ReadyToFinish => value <= 0;

    }

}