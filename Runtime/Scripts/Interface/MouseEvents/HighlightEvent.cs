using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class HighlightEvent : InterfaceEvent {

        public HighlightParams Params;

        public void Activate(bool logging) {
            var newTarget = Params.Target;
            if (newTarget != FruityUI.HighlightedTarget) {
                ApplyHierarchyChange(newTarget, logging);
            }
            else {
                ApplyHoverChain(firstFrame: false);
            }
        }

        private void ApplyHierarchyChange(MouseTarget newTarget, bool logging) {
            FruityUI.SwapHighlightAncestryBuffers();

            var currentBuffer = FruityUI.HighlightedTargetsBuffer;
            BuildAncestry(currentBuffer);

            var previousBuffer = FruityUI.PreviousHighlightedAncestry;
            var sharedDepth = FindSharedDepth(currentBuffer, previousBuffer);

            // End highlights no longer part of the ancestry
            for (var i = previousBuffer.Count - 1; i >= sharedDepth; i--) {
                previousBuffer[i]?.MouseHoverEnd();
            }

            FruityUI.HighlightedTarget = newTarget;

            // Re-apply shared chain with firstFrame=false
            ApplyHoverSegment(currentBuffer, sharedDepthExclusiveEnd: sharedDepth, firstFrame: false);

            // Apply newly highlighted branch; includes root
            ApplyHoverSegment(currentBuffer, startIndexInclusive: sharedDepth, firstFrame: true);

            FruityUI.SetHighlightAncestry(currentBuffer);

            if (logging && newTarget != null) {
                Debug.Log("Highlight: " + newTarget);
            }
        }

        private void ApplyHoverChain(bool firstFrame) {
            var currentBuffer = FruityUI.HighlightedTargetsBuffer;
            currentBuffer.Clear();

            BuildAncestry(currentBuffer);

            if (!firstFrame) {
                var existingBuffer = FruityUI.HighlightedAncestry;
                var sharedDepth = FindSharedDepth(currentBuffer, existingBuffer);

                for (var i = existingBuffer.Count - 1; i >= sharedDepth; i--) {
                    if (i >= currentBuffer.Count || currentBuffer[i] != existingBuffer[i]) {
                        existingBuffer[i]?.MouseHoverEnd();
                    }
                }
            }

            ApplyHoverSegment(currentBuffer, firstFrame: firstFrame);

            FruityUI.SetHighlightAncestry(currentBuffer);
        }

        private void ApplyHoverSegment(List<MouseTarget> ancestry, 
                int startIndexInclusive = 0, int sharedDepthExclusiveEnd = -1, bool firstFrame = false) {
            if (ancestry == null) {
                return;
            }

            var endIndex = (sharedDepthExclusiveEnd >= 0) ? Mathf.Min(sharedDepthExclusiveEnd, ancestry.Count) : ancestry.Count;
            for (var i = startIndexInclusive; i < endIndex; i++) {
                var target = ancestry[i];
                if (target == null) continue;

                var isRoot = (i == 0);
                var parameters = isRoot ? Params : HighlightParams.blank;
                target.MouseHovering(firstFrame, parameters);
            }
        }

        private int FindSharedDepth(List<MouseTarget> current, IReadOnlyList<MouseTarget> previous) {
            var depth = 0;
            while (depth < current.Count && depth < previous.Count && current[depth] == previous[depth]) {
                depth++;
            }
            return depth;
        }

        private void BuildAncestry(List<MouseTarget> buffer) {
            buffer.Clear();

            if (Params.Target != null) {
                buffer.Add(Params.Target);

                var parentNode = Params.Node != null ? Params.Node.InputParent : null;
                while (parentNode != null) {
                    var target = parentNode.GetMouseTarget(Params.MouseWorldPosition, Params.HeldButton);
                    if (target != null) {
                        buffer.Add(target);
                    }
                    parentNode = parentNode.InputParent;
                }
            }
        }

    }

}