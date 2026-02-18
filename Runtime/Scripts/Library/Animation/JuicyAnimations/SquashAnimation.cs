using UnityEngine;

namespace LycheeLabs.FruityInterface.Animation {

    // Pinches the transform, making it bulge in the Y axis and then in the X axis
    public class SquashAnimation : SimpleJuicyAnimation {

        private float value = 1f;
        private readonly float sizeScale;
        private readonly float speedScale;
        private readonly float cycles;
        private readonly Tween tween;

        public SquashAnimation (float sizeScale = 1f, float speedScale = 1f, float cycles = 3, Tween tween = null) {
            this.sizeScale = sizeScale * 0.15f;
            this.speedScale = Mathf.Max (speedScale, 0.1f) * 3f;
            this.cycles = cycles;
            this.tween = tween;
        }

        public override void Update (ref TransformData transform, float deltaTime) {
            value = value.MoveTowardsDelta(0, speedScale * deltaTime);
            var tweened = (tween != null) ? tween.ApplyInverted(value) : value;

            var squash = Mathf.Sin(tweened * Mathf.PI * cycles) * tweened * sizeScale * sizeScale;
            var squashScale = new Vector3(1 - squash, 1 + squash, 1);

            var existingScale = transform.scale;
            transform.scale = new Vector3(existingScale.x * squashScale.x, existingScale.y * squashScale.y, existingScale.z);
        }

        public override bool ReadyToFinish => value <= 0;

    }

}