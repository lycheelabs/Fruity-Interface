using UnityEngine;

namespace LycheeLabs.FruityInterface.Animation {
    // Squashes the transform along the Y axis, making it almost disappear
    public class FlattenSquashAnimation : SimpleJuicyAnimation {

        private float value = 1f;
        private readonly float speedScale;

        public FlattenSquashAnimation(float speedScale = 1f) {
            this.speedScale = Mathf.Max(speedScale, 0.1f) * 3f;
        }

        public override void Update(ref TransformData transform, float deltaTime) {
            value = value.MoveTowardsDelta(0, speedScale * deltaTime);
            var tweened = Mathf.Sin(Tweens.EaseOutQuad(1 - value) * Mathf.PI);

            var squashScale = new Vector3(1, 1 - tweened * 0.9f, 1);

            var existingScale = transform.scale;
            transform.scale = new Vector3(existingScale.x * squashScale.x, existingScale.y * squashScale.y, existingScale.z);
        }

        public override bool ReadyToFinish => value <= 0;

    }

}