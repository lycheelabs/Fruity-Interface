using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements.Buttons {

    public class TabbingButton : LayoutNode {

        public enum Component {
            Main,
            LeftArrow,
            RightArrow
        }

        public TextButton MainButton;
        public IconButton LeftArrow;
        public IconButton RightArrow;

        [SerializeField] private float width = 200;
        [SerializeField] private float height = 50;
        [SerializeField] private float arrowMargin = 20;

        protected override void RefreshLayout () {
            LeftArrow.ConfigureSize(height);
            RightArrow.ConfigureSize(height);

            var buttonWidth = Mathf.Max(height, width - height * 2 - arrowMargin);
            MainButton.SetWidth(buttonWidth);
            MainButton.SetHeight(height);

            LayoutSizePixels = new Vector2(width, height);
            rectTransform.sizeDelta = new Vector2(width, height);
        }

        public void Activate (Component type, MouseButton clickButton) {
            //
        }

        public void MouseOver (Component type) {
            //
        }

        public void UpdateLayout (Component type) {
            //
        }

    }

}