using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    public class ButtonSelectorNode : ContainerNode {

        [SerializeField] private int selectedIndex;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color unselectedColor = Color.gray;

        public int SelectedIndex => selectedIndex;

        public void Select (int index) {
            selectedIndex = index;
            ClampIndex();
            RefreshSelection();

            var effect = GetComponent<ButtonSelectorEffect>();
            if (effect == null) {
                Debug.LogWarning("ButtonSelectorNode is missing a ButtonSelectorEffect.");
                return;
            }
            effect.ApplySelection(selectedIndex);
        }

        public void OnChildClicked (ButtonSelectorSubEffect child) {
            var index = ChildNodes.IndexOf(child.GetComponent<LayoutNode>());
            if (index >= 0) {
                Select(index);
            }
        }

        protected override void OnChildAdded (LayoutNode newChild) {
            var sub = newChild.GetComponent<ButtonSelectorSubEffect>();
            if (sub == null) {
                Debug.LogWarning($"{newChild.name} is missing a ButtonSelectorSubEffect and will not be selectable.", newChild);
                return;
            }
            if (newChild.GetComponent<SelectableButton>() == null) {
                Debug.LogWarning($"{newChild.name} does not implement SelectableButton and will not be selectable.", newChild);
                return;
            }
            sub.Target = this;
        }

        protected override void RefreshLayout () {
            RemoveDestroyedChildren();
            ClampIndex();
            RefreshSelection();

            float containedWidth = 0;
            float containedHeight = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node.gameObject.activeSelf) { continue; }
                containedWidth += node.TotalWidthPixels;
                containedHeight = Mathf.Max(containedHeight, node.TotalHeightPixels);
            }

            var size = new Vector2(Mathf.Max(containedWidth, minimumSize.x), Mathf.Max(containedHeight, minimumSize.y));
            LayoutSizePixels = size;
            rectTransform.sizeDelta = LayoutSizePixels;
            var pivotOffset = (new Vector2(0.5f, 0.5f) - rectTransform.pivot) * LayoutSizePixels;

            float x = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!node.gameObject.activeSelf) { continue; }

                var newWidth = node.TotalWidthPixels;
                var shift = x - containedWidth / 2f + newWidth / 2f;
                node.rectTransform.localPosition = new Vector3(shift + pivotOffset.x, pivotOffset.y);
                x += newWidth;
            }
        }

        private void RefreshSelection () {
            for (int i = 0; i < ChildNodes.Count; i++) {
                var selectable = ChildNodes[i].GetComponent<SelectableButton>();
                selectable?.SetBackingColor(i == selectedIndex ? selectedColor : unselectedColor);
            }
        }

        private void ClampIndex () {
            selectedIndex = Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, ChildNodes.Count - 1));
        }

        private void RemoveDestroyedChildren () {
            for (int i = ChildNodes.Count - 1; i >= 0; i--) {
                if (ChildNodes[i] == null) {
                    ChildNodes.RemoveAt(i);
                }
            }
        }

    }

}
