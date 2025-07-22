using UnityEngine;

namespace LycheeLabs.FruityInterface.Animation {

    // Rotates around the Z axis
    public class WiggleAnimation : SimpleJuicyAnimation {

        private float value = 1f;
        private readonly float sizeScale;
        private readonly float speedScale;
        private readonly int flip;

        public WiggleAnimation(float sizeScale = 1f, float speedScale = 1f) {
            this.sizeScale = sizeScale * 4f;
            this.speedScale = Mathf.Max(speedScale, 0.1f) * 3.3f;
            flip = (Random.value > 0.5f) ? 1 : -1;
        }

        public override void Update(ref TransformData transform, float timeScaling) {
            value = value.MoveTowards(0, speedScale * timeScaling);

            var wiggle = Mathf.Sin(value * Mathf.PI * 2) * sizeScale * flip * value;
            transform.rotation += new Vector3(0, 0, wiggle);
        }

        public override bool ReadyToFinish => value <= 0;

    }

    // Translates along the X axis
    public class ShakeAnimation : SimpleJuicyAnimation {

        private float value = 1f;
        private readonly float sizeScale;
        private readonly float speedScale;
        private readonly int flip;

        public ShakeAnimation(float sizeScale = 1f, float speedScale = 1f) {
            this.sizeScale = sizeScale * 0.1f;
            this.speedScale = Mathf.Max(speedScale, 0.1f) * 3.3f;
            flip = (Random.value > 0.5f) ? 1 : -1;
        }

        public override void Update(ref TransformData transform, float timeScaling) {
            value = value.MoveTowards(0, speedScale * timeScaling);

            var wiggle = Mathf.Sin(value * Mathf.PI * 3) * sizeScale * flip;
            transform.position += new Vector3(wiggle, 0);
        }

        public override bool ReadyToFinish => value <= 0;

    }

}