using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public abstract class ContainerNode : LayoutNode {

        public readonly List<LayoutNode> ChildNodes = new List<LayoutNode>();
        private readonly List<LayoutNode> previousChildNodes = new List<LayoutNode>();
        public int ChildCount => ChildNodes.Count;
        [SerializeField] private int prevChildCount = -1;

        public Vector2 minimumSize = new Vector2(100, 100);

        new private void OnValidate () {
            RebuildChildren();
        }

        private void OnTransformChildrenChanged() {
            RebuildChildren();
        }

        private void LateUpdate () {
            if (Application.isPlaying) {
                RefreshLayout();
            } else {
                RefreshLayoutDeferred();
            }
        }

        public void Insert (LayoutNode child, int siblingIndex) {
            Attach(child);
            child.transform.SetSiblingIndex(siblingIndex);
            RebuildChildren();
        }

        // Extract and cache LayoutNode components from valid children.
        // Build list of valid children.
        private void RebuildChildren () {
            previousChildNodes.Clear();
            previousChildNodes.AddRange(ChildNodes);
            ChildNodes.Clear();
            for (int i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                var childNode = child.GetComponent<LayoutNode>();
                if (childNode != null) {
                    ChildNodes.Add(childNode);
                }
            }
            for (int i = 0; i < previousChildNodes.Count; i++) {
                var oldChild = previousChildNodes[i];
                if (!ChildNodes.Contains(oldChild)) {
                    OnChildRemoved(oldChild);
                }
            }
            for (int i = 0; i < ChildNodes.Count; i++) {
                var newChild = ChildNodes[i];
                if (!previousChildNodes.Contains(newChild)) {
                    OnChildAdded(newChild);
                }
            }
            RefreshLayoutDeferred();
        }

        protected virtual void OnChildAdded(LayoutNode newChild) {}
        protected virtual void OnChildRemoved(LayoutNode newChild) {}
        protected abstract override void RefreshLayout ();

        // -------------------------------------------------
        protected int CountPrunedChildren() {
            var numItems = 0;
            for (int i = 0; i < ChildNodes.Count; i++) {
                var node = ChildNodes[i];
                if (!ShouldPrune(node)) { 
                    numItems++;
                }
            }
            return numItems;
        }

        protected bool ShouldPrune (LayoutNode node) {
            return !node || !node.gameObject.activeSelf;
        }

    }

}