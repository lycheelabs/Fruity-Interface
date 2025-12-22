using UnityEngine;
using TMPro;

namespace LycheeLabs.FruityInterface.Elements {

    public class TextBoxNode : LayoutNode {

        public float Width = 500;

        [SerializeField] private TextMeshProUGUI _text;
        public TextMeshProUGUI Text => _text ??= GetComponent<TextMeshProUGUI>();

        private void OnValidate () {
            RefreshLayoutDeferred();
        }

        private void OnEnable () {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(OnTextChanged);
            RefreshLayoutDeferred();
        }

        private void OnDisable () {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(OnTextChanged);
        }

        public void SetText (string newText) {
            Text.text = newText;
            RefreshLayoutDeferred();
        }

        private void OnTextChanged (Object obj) {
            if (obj == Text) {
                RefreshLayoutDeferred();
            }
        }

        protected override void RefreshLayout () {
            // Find preferred height based on width
            var rect = Text.rectTransform;
            rect.sizeDelta = new Vector2(Width, rect.sizeDelta.y);
            var height = Text.preferredHeight;

            // Apply size
            rect.sizeDelta = new Vector2(Width, height);
            LayoutSizePixels = new Vector2(Width, height);
            RefreshLayoutDeferred();
        }

    }

}