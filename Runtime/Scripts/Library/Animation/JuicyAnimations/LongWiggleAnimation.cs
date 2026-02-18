using UnityEngine;

namespace LycheeLabs.FruityInterface.Animation {

    // Rotates around the Z axis, ramping up then ramping down
    public class LongWiggleAnimation : SimpleJuicyAnimation {

        private float value = 1f;
        private readonly float sizeScale;
        private readonly float speedScale;
        private readonly int flip;

        public LongWiggleAnimation(float sizeScale = 1f, float speedScale = 1f) {
            this.sizeScale = sizeScale * 6f;
            this.speedScale = Mathf.Max(speedScale, 0.1f) * 1f;
            flip = (Random.value > 0.5f) ? 1 : -1;
        }

        public override void Update(ref TransformData transform, float deltaTime) {
            value = value.MoveTowardsDelta(0, speedScale * deltaTime);

            var wiggle = Mathf.Sin(value * Mathf.PI * 8) * sizeScale * flip;
            var smoothing = Mathf.Sin(value * Mathf.PI);
            transform.rotation += new Vector3(0, 0, wiggle * smoothing);
        }

        public override bool ReadyToFinish => value <= 0;

    }

}