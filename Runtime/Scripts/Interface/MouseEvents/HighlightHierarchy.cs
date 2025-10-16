using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public class HighlightHierarchy {

        private readonly List<HighlightLink> current = new List<HighlightLink>(8);
        private readonly List<HighlightLink> previous = new List<HighlightLink>(8);

        public MouseTarget CurrentTarget { get; private set; }

        public void Build(HighlightParams @params) {
            current.Clear();

            var target = @params.Target;
            var node = @params.Node;
            CurrentTarget = target;

            if (target != null) {
                current.Add(new HighlightLink(target, node));
            }

            var parentNode = node != null ? node.InputParent : null;
            while (parentNode != null) {
                var parentTarget = parentNode.GetMouseTarget(@params.MouseWorldPosition, @params.HeldButton);
                if (parentTarget != null) {
                    current.Add(new HighlightLink(parentTarget, parentNode));
                }
                parentNode = parentNode.InputParent;
            }
        }

        public void ApplyDiff(bool logging, HighlightParams @params) {
            var newTarget = CurrentTarget;
            var sameTarget = newTarget == FruityUI.HighlightedTarget;
            var branchFirstFrame = !sameTarget && newTarget != null;

            var sharedTailCount = FindSharedTailCount(current, previous);
            var previousUniqueCount = previous.Count - sharedTailCount;
            var currentUniqueCount = current.Count - sharedTailCount;

            for (var i = 0; i < previousUniqueCount; i++) {
                previous[i].Target?.MouseHoverEnd();
            }

            if (!sameTarget) {
                FruityUI.HighlightedTarget = newTarget;
            }

            ApplyHoverSegment(current, 0, currentUniqueCount, @params, branchFirstFrame);
            ApplyHoverSegment(current, currentUniqueCount, current.Count, @params, false);

            if (logging && newTarget != null && !sameTarget) {
                Debug.Log("Highlight: " + newTarget);
            }

            previous.Clear();
            previous.AddRange(current);
        }

        private static void ApplyHoverSegment(List<HighlightLink> ancestry, int startIndexInclusive, int endIndexExclusive, HighlightParams @params, bool firstFrame) {
            if (ancestry == null) {
                return;
            }

            var clampedEnd = Mathf.Min(endIndexExclusive, ancestry.Count);
            for (var i = startIndexInclusive; i < clampedEnd; i++) {
                var link = ancestry[i];
                var target = link.Target;
                if (target == null) {
                    continue;
                }

                var parameters = ReferenceEquals(target, @params.Target) ? @params : HighlightParams.blank;
                target.MouseHovering(firstFrame, parameters);
            }
        }

        private static int FindSharedTailCount(List<HighlightLink> current, List<HighlightLink> previous) {
            var shared = 0;
            while (shared < current.Count && shared < previous.Count) {
                var currentLink = current[current.Count - 1 - shared];
                var previousLink = previous[previous.Count - 1 - shared];

                if (!ReferenceEquals(currentLink.Target, previousLink.Target) ||
                    !ReferenceEquals(currentLink.Node, previousLink.Node)) {
                    break;
                }
                shared++;
            }
            return shared;
        }

        private readonly struct HighlightLink {
            public HighlightLink(MouseTarget target, InterfaceNode node) {
                Target = target;
                Node = node;
            }

            public MouseTarget Target { get; }
            public InterfaceNode Node { get; }
        }
    }

}