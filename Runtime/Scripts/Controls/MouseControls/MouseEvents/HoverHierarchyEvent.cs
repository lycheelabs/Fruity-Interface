using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired every frame to update which target is being highlighted.
    /// Manages MouseHovering and MouseHoverEnd calls on targets.
    /// </summary>
    public class HoverHierarchyEvent : ControlEvent {

        private static readonly HoverHierarchy hierarchy = new HoverHierarchy();

        public HoverParams Params;

        public void Activate(bool logging) {
            hierarchy.Build(Params);
            hierarchy.ApplyDiff(logging, Params);
        }

    }

}