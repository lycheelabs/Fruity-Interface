using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Flow {

    public class IrisTransition : ScreenTransitionNode {

        public const string TargetKey = "Target";

        // --------------------------------------

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

        protected override void Configure(ScreenTransitionConfig config) {
            material.SetVector("_Origin", new Vector2(0.5f, 0.5f));

            if (config.TryGetConfig(TargetKey, out var target)) {
                if (target is ScreenPosition targetPosition) {
                    material.SetVector("_Origin", targetPosition.RawViewportVector());
                }
            }
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

    }

}