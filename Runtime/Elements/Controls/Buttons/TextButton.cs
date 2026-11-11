using System;
using LycheeLabs.FruityInterface.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Implementation of a button using the TextButton prefab.
    /// </summary>
    public class TextButton : ButtonNode, SelectableButton, DragTarget {

        public Image BackingImage;
        public TextMeshProUGUI ButtonText;
        public Image IconImage;
        public BoxCollider BoxCollider;
        public Image ChargeGraphic;

        [SerializeField] private float height = 50;
        [SerializeField] private float maxWidth = 200;
        [SerializeField] private bool cropWidth = false;
        [SerializeField] private Sprite iconSprite = null;

        [Range(0f, 2f)] [SerializeField] private float fontHeightScaling = 1f;

        private bool holdEnabled;
        private HoldCharge charge;
        private MouseButton heldButton;
        private Color chargeColor = Color.white;

        public float ChargeFill => charge.Fill;
        public float ChargeAlpha => charge.FadeAlpha;

        private void Awake () {
            if (ChargeGraphic != null) ChargeGraphic.gameObject.SetActive(false);
        }

        private void OnEnable () {
            RefreshLayoutDeferred();
        }

        /// <summary>
        /// Makes this button require a hold to trigger its effect. Holding fills the charge over duration seconds,
        /// drawn in fillColor on ChargeGraphic. Call from Start or later so the graphic is not hidden again by Awake.
        /// </summary>
        public void ConfigureHold (float duration, Color fillColor) {
            holdEnabled = true;
            charge.Configure(duration);
            chargeColor = fillColor;
            if (ChargeGraphic != null) ChargeGraphic.gameObject.SetActive(true);
        }

        public MouseDragMode GetDragMode (MouseButton dragButton) {
            if (!holdEnabled || TryGetEffect == null) return MouseDragMode.Disabled;
            return TryGetEffect.MouseButtonIsPermitted(dragButton) ? MouseDragMode.DragOnly : MouseDragMode.Disabled;
        }

        public void UpdateMouseDragging (bool isFirstFrame, DragParams dragParams) {
            if (!isFirstFrame) return;
            heldButton = dragParams.DragButton;
            charge.Press();
        }

        public void ApplyMouseDrag (DragParams dragParams) {
            charge.Release();
        }

        public void CancelMouseDrag () {
            charge.Release();
        }

        public override void ApplyMouseClick (ClickParams clickParams) {
            if (!holdEnabled) base.ApplyMouseClick(clickParams);
        }

        protected override void OnUpdate () {
            if (!holdEnabled) return;

            if (charge.Tick(Time.unscaledDeltaTime)) {
                FruityUI.CancelDrag(this);
                AnimateClick();
                TryGetEffect.Activate(heldButton);
                TryGetSFX?.OnClick();
            }

            if (ChargeGraphic != null) {
                ChargeGraphic.fillAmount = charge.Fill;
                var color = chargeColor;
                color.a *= charge.FadeAlpha;
                ChargeGraphic.color = color;
            }

            if (charge.IsCharging) TryGetEffect?.OnHoldProgress(charge.Fill);
        }

        public void SetText (string text) {
            ButtonText.text = text;
            RefreshLayoutDeferred();
        }

        public void SetWidth (float maxWidth, bool crop = false) {
            this.maxWidth = maxWidth;
            this.cropWidth = crop;
            RefreshLayoutDeferred();
        }

        public void SetHeight (float height) {
            this.height = height;
            SetFontScale(fontHeightScaling);
            RefreshLayoutDeferred();
        }

        public void SetFontScale (float heightScale) {
            this.fontHeightScaling = heightScale;
            RefreshLayoutDeferred();
        }

        public void SetColor (Color color) {
            BackingImage.color = color;
        }

        public void SetBackingColor (Color color) {
            BackingImage.color = color;
        }

        protected override void AnimateHover (float highlightTween, float heldTween) {
            /*var scaleShift = 0.03f * Tweens.EaseOutQuad(highlightTween) - 0.05f * Tweens.EaseOutQuad(heldTween);
            float highlightScale = 1 + scaleShift * AnimationScaling;
            ButtonAnimator.OverlayScale(Vector3.one * highlightScale * BaseScale);*/

            var scaleShift = 0.03f * Tweens.EaseOutQuad(highlightTween);
            float highlightScale = 1 + scaleShift * AnimationScaling;
            ButtonAnimator.OverlayScale(Vector3.one * highlightScale * BaseScale);

            var heldShift = new Vector3(0, -Tweens.EaseOutQuad(heldTween), 0) *10 * AnimationScaling;
            ButtonAnimator.OverlayPosition(heldShift * BaseScale);
        }

        protected override void AnimateClick () {
            ButtonAnimator.Squash(0.5f * AnimationScaling);
        }

        // ---------------------------------------------

        protected override void RefreshLayout() {

            // Override sizes
            if (LayoutDriver != null && LayoutDriver.isActiveAndEnabled) {
                maxWidth = LayoutDriver.width;
                height = LayoutDriver.height;
                cropWidth = LayoutDriver.cropWidth;
                LayoutPaddingPixels = LayoutDriver.layoutPadding;
                fontHeightScaling = LayoutDriver.fontHeightScaling;
            }

            // Update icon
            var hasIcon = iconSprite != null;
            IconImage.sprite = iconSprite;
            IconImage.gameObject.SetActive(hasIcon);

            // Calculate sizes
            var textWidth = ButtonText.preferredWidth;
            if (hasIcon) textWidth += height;
            var width = Mathf.Min(maxWidth, textWidth + 38);
            if (!cropWidth) width = maxWidth;
            var size = new Vector2(width, height);

            // Apply sizes
            LayoutSizePixels = size;
            rectTransform.sizeDelta = LayoutSizePixels;

            BoxCollider.size = new Vector3(LayoutSizePixels.x, LayoutSizePixels.y, 1);
            IconImage.rectTransform.sizeDelta = new Vector2(height, height) * 0.8f;
            IconImage.rectTransform.localPosition = new Vector3(height * 0.5f, 0);

            ButtonText.fontSizeMax = height * 0.7f * fontHeightScaling;
            ButtonText.fontSizeMin = height * 0.4f * fontHeightScaling;

            var offsetMin = ButtonText.rectTransform.offsetMin;
            if (hasIcon) {
                ButtonText.rectTransform.offsetMin = new Vector2(height + 4, offsetMin.y);
            } else {
                ButtonText.rectTransform.offsetMin = new Vector2(16, offsetMin.y);
            }

        }

    }

}
