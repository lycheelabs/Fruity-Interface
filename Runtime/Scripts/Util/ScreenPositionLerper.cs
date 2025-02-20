using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class ScreenPositionLerper {

		private readonly Transform transform;
		private readonly ScreenPosition startPosition;

		private ScreenPosition endPosition;
		private float zOffset;

		public ScreenPositionLerper (Transform transform, ScreenPosition startPosition, ScreenPosition endPosition, float zOffset = 0) {
			if (transform == null) {
				throw new NullReferenceException();
			}
			this.transform = transform;
			this.startPosition = startPosition;
			this.endPosition = endPosition;
			this.zOffset = zOffset;
		}

		public void ChangeDestination (ScreenPosition endPosition, float zOffset = 0) {
			this.endPosition = endPosition;
			this.zOffset = zOffset;
		}

		public void UpdatePosition (float lerp) {
			var lerpPosition = ScreenPosition.Lerp(startPosition, endPosition, lerp);
			var worldVector = lerpPosition.WorldVector();

			transform.position = worldVector;
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zOffset);
		}

	}

}