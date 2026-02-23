using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {
    public class SimpleContainerNode : ContainerNode {

        [SerializeField] LayoutNode LayoutContents;

        protected override void RefreshLayout () {
            if (LayoutContents != null) {
                LayoutSizePixels = LayoutContents.TotalSizePixels;
                rectTransform.sizeDelta = TotalSizePixels;
            }
        }

    }

}