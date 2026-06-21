using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Flow {

    public class WipeTransition : ScreenTransitionNode {

        public const string AngleKey = "Angle";

        // --------------------------------------

        public Image image;
        private Material material;

        public float angle = -45;

        protected override float TransitionSpeed => 1.9f;

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
            image.color = color;
        }

        protected override void Configure(ScreenTransitionConfig config) {
            angle = -45;
            if (config.TryGetConfig(AngleKey, out var target)) {
                if (target is float targetAngle) {
                    angle = targetAngle;
                }
            }
        }

        protected override void Refresh(bool isEntering, float tween) {
            if (image != null) {
                image.enabled = tween > 0;
            }

            if (material != null) {
                material.SetFloat("_Tween", Tweens.EaseOutQuad(tween * 0.66f));
                material.SetFloat("_Angle", angle);
                material.SetFloat("_Flip", isEntering ? 0 : 1);
            }
        }

        public void ClearTargetPoint() {
            //
        }

    }

}