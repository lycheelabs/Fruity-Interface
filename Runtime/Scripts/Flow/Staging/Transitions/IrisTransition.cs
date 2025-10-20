using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Flow {

    public class IrisTransition : ScreenTransition {

        public Image image;
        private Material material;
        private Color colorA;
        private Color colorB;

        protected override float TransitionSpeed => 1.66f;

        private void Start() {
            InitMaterial();
            image.enabled = false;
            SetColor(Color.black);
        }

        private void InitMaterial () {
            material = new Material(image.material);
            image.material = material;
            material.SetFloat("_Tween", 0);
        }

        public override void SetColor(Color color) {
            colorA = color;
            colorB = color;
        }

        protected override void Refresh(bool isEntering, float tween) {
            if (image != null) {
                image.material.SetFloat("_Tween", Tweens.EaseOutQuad(tween));
                image.color = Color.Lerp(colorA, colorB, Tweens.EaseInOutQuad(tween));
                image.enabled = tween > 0;
            }
        }

        public void ClearTargetPoint() {
            //
        }

        public void SetTargetPoint(Vector2 viewportOrigin) {
            material.SetVector("_Origin", viewportOrigin);
        }

    }

}