using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public enum TabbingSelectorComponent {
        Main,
        LeftArrow,
        RightArrow
    }

    public class TabbingSelector : ControlNode, ControlLayoutDriver {

        public TextButton MainButton;
        public IconButton LeftArrow;
        public IconButton RightArrow;

        [SerializeField] private float width = 200;
        [SerializeField] private float height = 50;
        [SerializeField] private float fontHeightScaling = 1;
        [SerializeField] private float iconScaling = 1;
        [SerializeField] private float arrowMargin = 20;

        [SerializeField] private Color arrowColorEnabled = Color.white;
        [SerializeField] private Color arrowColorDisabled = Color.gray;

        private float ButtonWidth => Mathf.Max(height, width - height * iconScaling * 2 - arrowMargin);

        public float DrivenWidth => ButtonWidth;
        public float DrivenHeight => height;
        public Vector2 DrivenLayoutPadding => default;
        public bool DrivenCropWidth => false;
        public float DrivenFontHeightScaling => fontHeightScaling;
        public float DrivenIconScale => iconScaling;
        public float DrivenInteriorMargins => arrowMargin;

        private TabbingSelectorEffect _effect;
        public TabbingSelectorEffect TryGetEffect {
            get {
                _effect = _effect ?? GetComponent<TabbingSelectorEffect>();
                return _effect;
            }
        }

        private void OnEnable () {
            if (TryGetEffect != null) {
                TryGetEffect.Initialise();
                ApplyOption(TryGetEffect.SelectedOption());
            }
        }

        public void Select (TabbingSelectorOption newOption) {
            if (TryGetEffect != null) {
                var options = TryGetEffect.ListAllOptions();
                if (options.Contains(newOption)) {
                    ApplyOption(newOption);
                }
            }
        }

        private void ApplyOption (TabbingSelectorOption option) {
            if (option != null) {
                MainButton.SetText(option.Name);
                MainButton.SetColor(option.Color);

                if (TryGetEffect != null) {
                    TryGetEffect.OnSelectionSet(option);
                }
            }
        }

        private void Update () {
            if (TryGetEffect != null) {
                MainButton.SetInputDisabled(!TryGetEffect.MainButtonIsSelectable);;

                var canTabLeft = TryGetEffect.CanTabLeft();
                LeftArrow.SetInputDisabled(!canTabLeft);
                LeftArrow.SetColor(canTabLeft ? arrowColorEnabled : arrowColorDisabled);

                var canTabRight = TryGetEffect.CanTabRight();
                RightArrow.SetInputDisabled(!canTabRight);
                RightArrow.SetColor(canTabRight ? arrowColorEnabled : arrowColorDisabled);
            }
        }

        protected override void RefreshLayout () {

            // Override layout
            if (LayoutDriver != null && LayoutDriver.isActiveAndEnabled) {
                width = LayoutDriver.width;
                height = LayoutDriver.height;
                LayoutPaddingPixels = LayoutDriver.layoutPadding;
                fontHeightScaling = LayoutDriver.fontHeightScaling;
                iconScaling = LayoutDriver.iconScaling;
                arrowMargin = LayoutDriver.interiorMargins;
            }

            // Pass layout to children
            var childDriver = GetComponent<ControlLayoutStyle>();
            if (childDriver != null) {
                childDriver.width = ButtonWidth;
                childDriver.height = height;
                childDriver.layoutPadding = default;
                childDriver.fontHeightScaling = fontHeightScaling;
                childDriver.iconScaling = iconScaling;
                childDriver.interiorMargins = 0;

                LeftArrow.BasePosition = new Vector3(height * iconScaling / 2f, 0);
                RightArrow.BasePosition = new Vector3(-height * iconScaling / 2f, 0);

                MainButton.RefreshLayoutDeferred();
                LeftArrow.RefreshLayoutDeferred();
                RightArrow.RefreshLayoutDeferred();
            }

            // Apply size to self
            LayoutSizePixels = new Vector2(width, height);
            rectTransform.sizeDelta = new Vector2(width, height);

        }

        public void Activate (TabbingSelectorComponent type, MouseButton clickButton) {
            if (TryGetEffect != null) {
                if (clickButton == MouseButton.Left) {
                    ActivateComponent(type);
                }
            } else {
                Debug.LogWarning("TabbingSelectorEffect component not found on " + name);
            }
        }

        private void ActivateComponent (TabbingSelectorComponent type) {
            if (type == TabbingSelectorComponent.Main) {
                TryGetEffect.ActivateMainButton();
                MainButton.ButtonAnimator.Squash(3);
            }
            if (type == TabbingSelectorComponent.LeftArrow) {
                ApplyOption(TryGetEffect.TabLeft());
                MainButton.ButtonAnimator.Squash(3);
            } 
            if (type == TabbingSelectorComponent.RightArrow) {
                ApplyOption(TryGetEffect.TabRight());
                MainButton.ButtonAnimator.Squash(3);
            }
        }

        public void MouseOver (TabbingSelectorComponent type) {
            if (TryGetEffect != null) {
                TryGetEffect.MouseOverComponent(type);
            }
        }

        public void UpdateLayout (TabbingSelectorComponent type) {
            //
        }

    }

}