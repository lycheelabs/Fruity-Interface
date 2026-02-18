using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace LycheeLabs.FruityInterface.Elements {

    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public abstract class ContainerNode : LayoutNode {
        
        public readonly List<LayoutNode> ChildNodes = new List<LayoutNode>();
        public int ChildCount => ChildNodes.Count;
        [SerializeField] private int prevChildCount = -1;

        public Vector2 minimumSize = new Vector2(100, 100);

        private void LateUpdate () {
            RefreshChildren();
        }

        private void RefreshChildren () {
            if (transform.childCount != prevChildCount || true) {
                prevChildCount = transform.childCount;
                RebuildChildNodes();
            }
        }

        protected override void OnNewChildAttached () {
            RebuildChildNodes();
        }

        public void RebuildChildNodes () {
            ChildNodes.Clear();
            for (int i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                var childNode = child.GetComponent<LayoutNode>();
                if (childNode != null) {
                    ChildNodes.Add(childNode);
                }
            }
            RefreshLayoutDeferred();
        }

        protected abstract override void RefreshLayout ();

    }

}