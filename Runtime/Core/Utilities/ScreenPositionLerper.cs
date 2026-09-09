using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class ScreenPositionLerper {

        private readonly Transform transform;
        private readonly ScreenAnchor startAnchor;

        private ScreenAnchor endAnchor;
        private float zOffset;

        public ScreenPositionLerper(Transform transform, WorldAnchor startPosition, WorldAnchor endPosition, float zOffset = 0) {
            if (transform == null) {
                throw new NullReferenceException();
            }
            this.transform = transform;
            this.startAnchor = startPosition.PinScreen();
            this.endAnchor = endPosition.PinScreen();
            this.zOffset = zOffset;
        }

        public void ChangeDestination(WorldAnchor endPosition, float zOffset = 0) {
            this.endAnchor = endPosition.PinScreen();
            this.zOffset = zOffset;
        }

        public void UpdatePosition(float lerp) {
            var lerpPosition = ScreenAnchor.Lerp(startAnchor, endAnchor, lerp);
            var worldVector = lerpPosition.WorldVector();

            transform.position = worldVector;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zOffset);
        }

    }

}
