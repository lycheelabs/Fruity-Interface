using UnityEngine;
using TMPro;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextLayout : LayoutNode {

        private TextMeshProUGUI _text;
        public TextMeshProUGUI Text {
            get {
                _text = _text ?? GetComponent<TextMeshProUGUI>();
                return _text;
            }
        }

        private void LateUpdate () {
            var rect = Text.rectTransform;
            var width = Mathf.Max(50, LayoutSizePixels.x);
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
            var height = Text.preferredHeight;
            rect.sizeDelta = new Vector2(width, height);
            LayoutSizePixels = new Vector2(width, height);
        }

    }

}