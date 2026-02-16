using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Manages hover state hierarchy.
    /// Tracks which MouseTargets are currently being hovered and handles state transitions.
    /// </summary>
    public class HoverHierarchy : MouseTargetHierarchy {

        /// <summary>
        /// Build hierarchy for the given target.
        /// </summary>
        public void Build(MouseTarget target, HoverParams hoverParams) {
            base.Build(target, hoverParams.Node, hoverParams.MouseWorldPosition, hoverParams.PressButton);
        }

        /// <summary>
        /// Apply differences between previous and current hover chains.
        /// </summary>
        public void ApplyDiff(bool logging, HoverParams hoverParams) {
            base.ApplyDiff(logging, hoverParams);
        }

        protected override bool ShouldIncludeTarget(MouseTarget target) {
            // All MouseTargets participate in hover
            return target != null;
        }

        protected override void CallUpdate<TParams>(MouseTarget target, bool firstFrame, TParams parameters, bool isLeaf) {
            // Only pass full params to the leaf target
            var hoverParams = isLeaf && parameters is HoverParams hp 
                ? hp 
                : HoverParams.blank;
            
            target.UpdateMouseHover(firstFrame, hoverParams);
        }

        protected override void CallEnd(MouseTarget target) {
            target?.EndMouseHover();
        }

        protected override void UpdateGlobalState(MouseTarget target) {
            FruityUI.HighlightedTarget = target;
        }

        protected override void LogTargetChange(MouseTarget target) {
            Debug.Log("Highlight: " + target);
        }
    }

}