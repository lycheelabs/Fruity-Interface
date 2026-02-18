using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Base class for tracking mouse target hierarchies (hover, drag-over, etc.).
    /// Handles the complex logic of diffing parent chains and managing state transitions.
    /// Subclasses only need to implement the specific callback methods.
    /// </summary>
    public abstract class MouseTargetHierarchy {
        
        private readonly List<TargetLink> current = new List<TargetLink>(8);
        private readonly List<TargetLink> previous = new List<TargetLink>(8);
        
        protected MouseTarget CurrentTarget => current.Count > 0 ? current[0].Target : null;
        
        /// <summary>
        /// Build the ancestry chain from the raycast hit up through parent hierarchy.
        /// Subclasses override ShouldIncludeTarget to filter which targets participate.
        /// </summary>
        public void Build(MouseTarget raycastHit, InterfaceNode node, Vector3 mouseWorldPos, MouseButton button) {
            current.Clear();
            
            // Add raycast hit if it passes filter
            if (ShouldIncludeTarget(raycastHit)) {
                AddLink(raycastHit, node);
            }
            
            // Walk up parent hierarchy
            var parentNode = node?.InputParent;
            while (parentNode != null) {
                var parentTarget = parentNode.GetMouseTarget(mouseWorldPos, button);
                if (ShouldIncludeTarget(parentTarget)) {
                    AddLink(parentTarget, parentNode);
                }
                parentNode = parentNode.InputParent;
            }
        }
        
        /// <summary>
        /// Apply differences between previous and current chains.
        /// </summary>
        public void ApplyDiff<TParams>(bool logging, TParams parameters) {
            var newTarget = CurrentTarget;
            var previousTarget = previous.Count > 0 ? previous[0].Target : null;
            var sameTarget = ReferenceEquals(newTarget, previousTarget);
            
            FindExclusiveRanges(out var previousExclusive, out var currentExclusive);
            
            // Call end callbacks on removed targets
            EndRemovedTargets(previousExclusive);
            
            // Update global state
            UpdateGlobalState(newTarget);
            
            // Call update callbacks on new segment
            ApplySegment(0, currentExclusive, parameters, firstFrame: !sameTarget && newTarget != null);
            ApplySegment(currentExclusive, current.Count, parameters, firstFrame: false);
            
            if (logging && newTarget != null && !sameTarget) {
                LogTargetChange(newTarget);
            }
            
            SnapshotCurrent();
        }
        
        /// <summary>
        /// Clear all state (called when operation ends).
        /// </summary>
        public void Clear() {
            for (var i = 0; i < current.Count; i++) {
                CallEnd(current[i].Target);
            }
            current.Clear();
            previous.Clear();
            UpdateGlobalState(null);
        }
        
        // ========== Abstract methods subclasses must implement ==========
        
        /// <summary>
        /// Filter which targets should be included in the hierarchy.
        /// </summary>
        protected abstract bool ShouldIncludeTarget(MouseTarget target);
        
        /// <summary>
        /// Call the "update" method on a target (e.g., MouseHovering or MouseDragOver).
        /// </summary>
        protected abstract void CallUpdate<TParams>(MouseTarget target, bool firstFrame, TParams parameters, bool isLeaf);
        
        /// <summary>
        /// Call the "end" method on a target (e.g., MouseHoverEnd or MouseDragOverEnd).
        /// </summary>
        protected abstract void CallEnd(MouseTarget target);
        
        /// <summary>
        /// Update global state with the current target.
        /// </summary>
        protected abstract void UpdateGlobalState(MouseTarget target);
        
        /// <summary>
        /// Log when target changes (if logging enabled).
        /// </summary>
        protected abstract void LogTargetChange(MouseTarget target);
        
        // ========== Private implementation (shared by all subclasses) ==========
        
        private void AddLink(MouseTarget target, InterfaceNode node) {
            if (target != null) {
                current.Add(new TargetLink(target, node));
            }
        }
        
        private void EndRemovedTargets(int count) {
            for (var i = 0; i < count && i < previous.Count; i++) {
                CallEnd(previous[i].Target);
            }
        }
        
        private void ApplySegment<TParams>(int startInclusive, int endExclusive, TParams parameters, bool firstFrame) {
            if (startInclusive >= endExclusive) return;
            
            var clampedEnd = Mathf.Min(endExclusive, current.Count);
            for (var i = startInclusive; i < clampedEnd; i++) {
                var target = current[i].Target;
                var isLeaf = (i == 0);
                CallUpdate(target, firstFrame && isLeaf, parameters, isLeaf);
            }
        }
        
        private void SnapshotCurrent() {
            previous.Clear();
            previous.AddRange(current);
        }
        
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
        
        private readonly struct TargetLink {
            public TargetLink(MouseTarget target, InterfaceNode node) {
                Target = target;
                Node = node;
            }
            
            public MouseTarget Target { get; }
            public InterfaceNode Node { get; }
        }
    }

}
