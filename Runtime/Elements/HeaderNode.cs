using TMPro;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    [ExecuteAlways]
    public class HeaderNode : LayoutNode {

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private float textScale = 0.5f;

        new private void OnValidate() {
            RefreshLayoutDeferred();
        }

        protected override void RefreshLayout() {
            rectTransform.sizeDelta = LayoutSizePixels;

            var height = LayoutSizePixels.y;
            text.fontSizeMax = height * textScale;
            text.fontSizeMin = height * textScale * 0.25f;
        }

    }

}
