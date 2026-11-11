using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class TabbingContainerNode : ContainerNode {

        [SerializeField] private int index;

        public int Index => index;

        public void SetIndex (int newIndex) {
            index = newIndex;
            ClampIndex();
            RefreshLayoutDeferred();
        }

        private void LateUpdate () {
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node) { continue; }
                var shouldBeActive = i == index;
                if (node.gameObject.activeSelf != shouldBeActive) {
                    node.gameObject.SetActive(shouldBeActive);
                }
            }
        }

        protected override void RefreshLayout () {
            RemoveDestroyedChildren();
            ClampIndex();

            float containedWidth = 0;
            float containedHeight = 0;

            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                containedWidth = Mathf.Max(containedWidth, node.TotalWidthPixels);
                containedHeight = Mathf.Max(containedHeight, node.TotalHeightPixels);
            }

            containedWidth = Mathf.Max(containedWidth, minimumSize.x);
            containedHeight = Mathf.Max(containedHeight, minimumSize.y);

            LayoutSizePixels = new Vector2(containedWidth, containedHeight);
            rectTransform.sizeDelta = TotalSizePixels;
        }

        private void ClampIndex () {
            index = Mathf.Clamp(index, 0, Mathf.Max(0, ChildNodes.Count - 1));
        }

        private void RemoveDestroyedChildren () {
            for (int i = ChildNodes.Count - 1; i >= 0; i--) {
                var node = ChildNodes[i];
                if (node == null) {
                    ChildNodes.RemoveAt(i);
                }
            }
        }

    }

}
