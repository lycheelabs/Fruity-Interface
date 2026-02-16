using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Manages hover state hierarchy.
    /// Tracks which MouseTargets are currently being hovered and handles state transitions.
    /// </summary>
    public class HoverHierarchy : MouseTargetHierarchy {

        // Build method specific to highlight params
        public void Build(HoverParams hoverParams) {
            base.Build(hoverParams.Target, hoverParams.Node, hoverParams.MouseWorldPosition, hoverParams.HeldButton);
        }

        // ApplyDiff method specific to highlight params - use non-generic overload
        public void ApplyDiff(bool logging, HoverParams highlightParams) {
            base.ApplyDiff(logging, highlightParams);
        }

        protected override bool ShouldIncludeTarget(MouseTarget target) {
            // All MouseTargets participate in hover
            return target != null;
        }

        protected override void CallUpdate<TParams>(MouseTarget target, bool firstFrame, TParams parameters, bool isLeaf) {
            // Only pass full params to the leaf target
            var highlightParams = isLeaf && parameters is HoverParams hp 
                ? hp 
                : HoverParams.blank;
            
            target.MouseHovering(firstFrame, highlightParams);
        }

        protected override void CallEnd(MouseTarget target) {
            target?.MouseHoverEnd();
        }

        protected override void UpdateGlobalState(MouseTarget target) {
            FruityUI.HighlightedTarget = target;
        }

        protected override void LogTargetChange(MouseTarget target) {
            Debug.Log("Highlight: " + target);
        }
    }

}