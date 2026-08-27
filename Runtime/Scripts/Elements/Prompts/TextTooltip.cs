using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class TextTooltip : InterfaceNode {

        public TextBoxNode Text;
        public TooltipBubble Bubble;

        public void Show(string text, WorldAnchor position, Direction offsetDirection, float wrapWidth, float scale = 1f, float padding = 0f) {
            Text.Width = wrapWidth;
            Bubble.LayoutPaddingPixels = new Vector2(padding, padding);
            Bubble.RefreshLayoutDeferred();
            Text.SetText(text, crop: true);
            Bubble.Show(position, offsetDirection, scale);
        }

        public void Show () {
            Bubble.Show();
        }

        public void Hide () {
            Bubble.Hide();
        }

        public void SetSuppressed(bool hidden) {
            Bubble.OverrideHidden(hidden);
        }

    }

}
