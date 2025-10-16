using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class HighlightEvent : InterfaceEvent {

        public HighlightParams Params;

        private readonly struct HighlightLink {
            public HighlightLink(MouseTarget target, InterfaceNode node) {
                Target = target;
                Node = node;
            }

            public MouseTarget Target { get; }
            public InterfaceNode Node { get; }
        }

        private static readonly List<HighlightLink> currentAncestry = new List<HighlightLink>(8);
        private static readonly List<HighlightLink> previousAncestry = new List<HighlightLink>(8);

        public void Activate(bool logging) {
            BuildAncestry(currentAncestry);

            var newTarget = Params.Target;
            var sameTarget = newTarget == FruityUI.HighlightedTarget;
            var branchFirstFrame = !sameTarget && newTarget != null;

            DiffAndApply(newTarget, logging, sameTarget, branchFirstFrame);

            previousAncestry.Clear();
            previousAncestry.AddRange(currentAncestry);
        }

        private void DiffAndApply(MouseTarget newTarget, bool logging, bool isSameTarget, bool branchFirstFrame) {
            var sharedTailCount = FindSharedTailCount(currentAncestry, previousAncestry);
            var previousUniqueCount = previousAncestry.Count - sharedTailCount;
            var currentUniqueCount = currentAncestry.Count - sharedTailCount;

            for (var i = 0; i < previousUniqueCount; i++) {
                previousAncestry[i].Target?.MouseHoverEnd();
            }

            if (!isSameTarget) {
                FruityUI.HighlightedTarget = newTarget;
            }

            ApplyHoverSegment(currentAncestry, 0, currentUniqueCount, branchFirstFrame);
            ApplyHoverSegment(currentAncestry, currentUniqueCount, currentAncestry.Count, firstFrame: false);

            if (logging && newTarget != null && !isSameTarget) {
                Debug.Log("Highlight: " + newTarget);
            }
        }

        private void BuildAncestry(List<HighlightLink> ancestry) {
            ancestry.Clear();

            var target = Params.Target;
            var node = Params.Node;
            if (target != null) {
                ancestry.Add(new HighlightLink(target, node));
            }

            var parentNode = node != null ? node.InputParent : null;
            while (parentNode != null) {
                var parentTarget = parentNode.GetMouseTarget(Params.MouseWorldPosition, Params.HeldButton);
                if (parentTarget != null) {
                    ancestry.Add(new HighlightLink(parentTarget, parentNode));
                }
                parentNode = parentNode.InputParent;
            }
        }

        private void ApplyHoverSegment(List<HighlightLink> ancestry, int startIndexInclusive, int endIndexExclusive, bool firstFrame) {
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

                var parameters = ReferenceEquals(target, Params.Target) ? Params : HighlightParams.blank;
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
    }

}