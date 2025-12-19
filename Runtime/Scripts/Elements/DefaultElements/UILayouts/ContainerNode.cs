using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(RectTransform))]
    public abstract class ContainerNode : LayoutNode {
        
        public readonly List<LayoutNode> ChildNodes = new List<LayoutNode>();
        public int ChildCount => ChildNodes.Count;

        public Vector2 minimumSize = new Vector2(100, 100);

        public void RebuildChildNodes () {
            ChildNodes.Clear();
            for (int i = 0; i < transform.childCount; i++) {
                var child = transform.GetChild(i);
                var childNode = child.GetComponent<LayoutNode>();
                if (childNode != null) {
                    ChildNodes.Add(childNode);
                }
            }
        }

        private void OnEnable () {
            RebuildChildNodes();
        }

        private void Update () {
            if (!Application.isPlaying) {
                RebuildChildNodes();
            }
            Layout();
        }

        protected abstract void Layout ();

    }

}