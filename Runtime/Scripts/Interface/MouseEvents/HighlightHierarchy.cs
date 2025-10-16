using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {
    // Reuses ancestry buffers to diff highlight chains without per-frame allocations.
    public class HighlightHierarchy {

        private readonly List<HighlightLink> current = new List<HighlightLink>(8);
        private readonly List<HighlightLink> previous = new List<HighlightLink>(8);

        public MouseTarget CurrentTarget => current.Count > 0 ? current[0].Target : null;

        // Build the ancestry list for the current highlight chain (leaf first, parents following).
        public void Build(HighlightParams highlightParams) {
            current.Clear();

            AddLink(highlightParams.Target, highlightParams.Node);

            var node = highlightParams.Node?.InputParent;
            while (node != null) {
                var parentTarget = node.GetMouseTarget(highlightParams.MouseWorldPosition, highlightParams.HeldButton);
                AddLink(parentTarget, node);
                node = node.InputParent;
            }
        }

        // Apply changes between the previous frame's chain and the freshly built one.
        public void ApplyDiff(bool logging, HighlightParams highlightParams) {
            var newTarget = CurrentTarget;
            var sameTarget = newTarget == FruityUI.HighlightedTarget;

            FindExclusiveRanges(out var previousExclusive, out var currentExclusive);

            DehighlightRemoved(previousExclusive);

            if (!sameTarget) {
                FruityUI.HighlightedTarget = newTarget;
            }

            ApplyHoverSegment(0, currentExclusive, highlightParams, firstFrame: !sameTarget && newTarget != null);
            ApplyHoverSegment(currentExclusive, current.Count, highlightParams, firstFrame: false);

            if (logging && newTarget != null && !sameTarget) {
                Debug.Log("Highlight: " + newTarget);
            }

            SnapshotCurrent();
        }

        // Append a new link (if target is non-null).
        private void AddLink(MouseTarget target, InterfaceNode node) {
            if (target != null) {
                current.Add(new HighlightLink(target, node));
            }
        }

        // Issue MouseHoverEnd to links that dropped out of the chain.
        private void DehighlightRemoved(int count) {
            for (var i = 0; i < count && i < previous.Count; i++) {
                previous[i].Target?.MouseHoverEnd();
            }
        }

        // Replay MouseHovering for the requested segment of the chain.
        private void ApplyHoverSegment(int startIndexInclusive, int endIndexExclusive, HighlightParams highlightParams, bool firstFrame) {
            if (startIndexInclusive >= endIndexExclusive) {
                return;
            }

            var clampedEnd = Mathf.Min(endIndexExclusive, current.Count);
            for (var i = startIndexInclusive; i < clampedEnd; i++) {
                var target = current[i].Target;
                if (target == null) {
                    continue;
                }

                var parameters = ReferenceEquals(target, highlightParams.Target) ? highlightParams : HighlightParams.blank;
                target.MouseHovering(firstFrame, parameters);
            }
        }

        // Capture this frame's chain so it becomes "previous" on the next update.
        private void SnapshotCurrent() {
            previous.Clear();
            previous.AddRange(current);
        }

        // Determine how many trailing links are identical between current and previous lists.
        private void FindExclusiveRanges(out int previousExclusive, out int currentExclusive) {
            var shared = 0;
            var maxShared = Mathf.Min(current.Count, previous.Count);
            for (; shared < maxShared; shared++) {
                var currentLink = current[current.Count - 1 - shared];
                var previousLink = previous[previous.Count - 1 - shared];

                if (!ReferenceEquals(currentLink.Target, previousLink.Target) ||
                    !ReferenceEquals(currentLink.Node, previousLink.Node)) {
                    break;
                }
            }
            previousExclusive = previous.Count - shared;
            currentExclusive = current.Count - shared;
        }

        // Small data carrier that ties a MouseTarget to the InterfaceNode we derived it from.
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