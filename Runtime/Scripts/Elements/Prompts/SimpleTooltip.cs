using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class SimpleTooltip : InterfaceNode {

        public TextBoxNode Text;
        public TooltipBubble Bubble;

        public void Show(string text, WorldAnchor position, Direction offsetDirection, float scale = 1f) {
            Text.SetText(text);
            Bubble.Show(position, offsetDirection, scale);
        }

        public void Show () {
            Bubble.Show();
        }

        public void Hide () {
            Bubble.Hide();
        }

    }

}