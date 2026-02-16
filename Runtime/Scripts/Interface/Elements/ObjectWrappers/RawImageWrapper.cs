using UnityEngine;
using UnityEngine.UI;
using System;

namespace LycheeLabs.FruityInterface.Elements {

	public class RawImageWrapper : RectTransformWrapper {

		protected readonly RawImage image;

		public RawImageWrapper (GameObject gameObject) : base(gameObject) {
			image = gameObject.GetComponent<RawImage>();
            if (image == null) {
                throw new NullReferenceException("No RawImage component found");
            }
        }

		public Vector2 ImageSize {
			get { return rectTransform.sizeDelta; }
			set { rectTransform.sizeDelta = value; }
		}

		public Texture Texture {
			get { return image.texture; }
			set { image.texture = value; }
		}

		public Material Material {
			get { return image.material; }
			set { image.material = value; }
		}

		public void Crop (float widthScale, float heightScale) {
			float xOffset = (1f - widthScale) / 2f;
			float yOffset = (1f - heightScale) / 2f;
			image.uvRect = new Rect(xOffset, yOffset, widthScale, heightScale);
		}

		public Vector2 Tiling {
			set { image.uvRect = new Rect(image.uvRect.x, image.uvRect.y, value.x, value.y); }
		}

		public Vector2 TileOffset {
			set { image.uvRect = new Rect(value.x, value.y, image.uvRect.width, image.uvRect.height); }
		}

		public Color Color {
			get { return image.color; }
			set { image.color = value; }
		}

		public bool Visible {
			set { image.enabled = value; }
		}

	}

}