using UnityEngine;
using UnityEngine.UI;
using System;

namespace LycheeLabs.FruityInterface.Elements {

    public class ImageWrapper : RectTransformWrapper {

        protected readonly Image image;

        public ImageWrapper (GameObject gameObject) : base(gameObject) {
            image = gameObject.GetComponent<Image>();
            if (image == null) {
                throw new NullReferenceException("No Image component found");
            }
        }

        public ImageWrapper (Transform transform) : this(transform.gameObject) { }

        public Vector2 ImageSize {
            get { return rectTransform.sizeDelta; }
            set { rectTransform.sizeDelta = value; }
        }

        public Sprite Sprite {
            get { return image.sprite; }
            set { image.sprite = value; }
        }

        public Material Material {
            get { return image.material; }
            set { image.material = value; }
        }

        public Image.Type ImageType {
            set { image.type = value; }
        }

        public Color Color {
            get { return image.color; }
            set { image.color = value; }
        }

        public Image.FillMethod FillType {
            set { image.fillMethod = Image.FillMethod.Horizontal; }
        }

        public float FillAmount {
            set { image.fillAmount = value; }
            get { return image.fillAmount; }
        }

        public bool FillClockwise {
            set { image.fillClockwise = value; }
        }

        public bool Visible {
            get { return image.enabled; }
            set { image.enabled = value; }
        }

        public void AddMask () {
            if (gameObject.GetComponent<Mask>() != null) {
                return;
            }
            gameObject.AddComponent<Mask>();
        }

    }

}

