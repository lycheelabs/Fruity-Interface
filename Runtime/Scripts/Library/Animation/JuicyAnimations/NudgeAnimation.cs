using UnityEngine;

namespace LycheeLabs.FruityInterface.Animation {

    // Bounces the transform's position in a direction
    public class NudgeAnimation : SimpleJuicyAnimation {

        private float value = 1f;
        private readonly Vector3 direction;
        private readonly float sizeScale;
        private readonly float speedScale;

        public NudgeAnimation (Vector3 direction, float sizeScale = 1f, float speedScale = 1f) {
            direction.z = 0;
            if (direction.magnitude != 0) {
                direction = direction.normalized;
            }

            this.direction = direction;
            this.sizeScale = sizeScale * 0.1f;
            this.speedScale = Mathf.Max(speedScale, 0.1f) * 8f;
        }

        public override void Update (ref TransformData transform, float deltaTime) {
            value = value.MoveTowardsDelta(0, speedScale * deltaTime);

            var nudge = Mathf.Sin(value * Mathf.PI) * sizeScale;
            var nudgePos = nudge * direction;

            var existingPos = transform.position;
            transform.position = new Vector3(
                existingPos.x + nudgePos.x, existingPos.y + nudgePos.y, existingPos.z);
        }

        public override bool ReadyToFinish => value <= 0;

    }

}