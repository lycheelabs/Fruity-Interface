using UnityEngine;
using System.Collections;

namespace LycheeLabs.FruityInterface {

    public struct ScreenPosition {

		public static ScreenPosition Lerp (ScreenPosition a, ScreenPosition b, float tween, Vector2 screenOffset = default) {
			var lerpPosition = Vector3.Lerp(a.position, b.position, tween);
			var lerpOffset = Vector2.Lerp(a.offset, b.offset, tween);
			return new ScreenPosition(lerpPosition, lerpOffset + screenOffset);
		}

		/// <summary>
		/// ScreenOffset is relative to screen center and measured in UIConfig units
		/// </summary>
		public static ScreenPosition FromCamera (Vector2 screenOffset, Camera camera = null) {
			camera = camera ?? Camera.main;
			var cameraCenter = camera.ScreenToWorldPoint(new Vector3 (Screen.width, Screen.height) / 2f);
			var position = new ScreenPosition(cameraCenter, screenOffset);
			position.Bake();
			return position;
		}

		// ---------------------------------------------------------------------

		private Vector3 position;
		private Vector2 offset;

		private Vector3 bakedPosition;
		private bool baked;

		public ScreenPosition (Vector3 worldPosition) {
			position = worldPosition;
			offset = default;
			bakedPosition = default;
			baked = false;
		}

		public ScreenPosition (Vector3 worldPosition, Vector2 screenOffset) : this (worldPosition) {
			offset = screenOffset;
		}

		/// <summary> Baking locks in the camera's current position.</summary>
		public ScreenPosition Bake () => Bake(Camera.main);

		/// <summary> Baking locks in the camera's current position.</summary>
		public ScreenPosition Bake(Camera camera) {
			bakedPosition = (Vector2)camera.WorldToScreenPoint(position);
			baked = true;
			return this;
		}


		/// <summary> Returns the position as a scaled screenspace vector, relative to the screen centre.</summary>
		public Vector3 ScreenVector () => ScreenVector(Camera.main);

		/// <summary> Returns the position as a scaled screenspace vector, relative to the screen centre.</summary>
		public Vector3 ScreenVector (Camera camera) {
			var canvasVector = RawScreenVector(camera) * ScreenBounds.UIScaling;
            var screenOffset = (Vector3)(-ScreenBounds.WindowCanvasSize / 2f);
			return canvasVector + screenOffset;
        }

        /// <summary> Returns the position as a raw screenspace vector, relative to the screen corner.</summary>
        public Vector3 RawScreenVector() => RawScreenVector(Camera.main);

        /// <summary> Returns the position as a raw screenspace vector, relative to the screen corner.</summary>
        public Vector3 RawScreenVector (Camera camera) {
            if (camera == null) {
                Debug.LogWarning("Resolving a ScreenPosition requires a Camera");
                return default;
            }

            var screenPosition = bakedPosition;
            if (!baked) {
				screenPosition = camera.WorldToScreenPoint(position);
            }
			return screenPosition + (Vector3)offset / ScreenBounds.UIScaling;
        }

        /// <summary> Returns the position as a raw viewport vector, relative to the screen corner.</summary>
        public Vector3 RawViewportVector() => RawViewportVector(Camera.main);

        /// <summary> Returns the position as a raw viewport vector, relative to the screen corner.</summary>
        public Vector2 RawViewportVector(Camera camera) {
            var rawVector = RawScreenVector(camera);
			return new Vector2(rawVector.x / Screen.width, rawVector.y / Screen.height);
        }

        /// <summary> Returns the position as a worldspace position.</summary>
        public Vector3 WorldVector () => WorldVector(Camera.main);

        /// <summary> Returns the position as a worldspace position.</summary>
        public Vector3 WorldVector (Camera camera) {
			var screenVector = ScreenVector(camera);
			var adjustedVector = (screenVector / ScreenBounds.UIScaling) + new Vector3(Screen.width, Screen.height) / 2f;
			var worldVector = Camera.main.ScreenToWorldPoint(adjustedVector);
			return worldVector.IntersectWithWorldPlane();
		}

	}

}