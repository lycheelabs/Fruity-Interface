using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired every frame to update which target is being highlighted.
    /// Manages MouseHovering and MouseHoverEnd calls on targets.
    /// </summary>
    public class HighlightEvent : InterfaceEvent {

        private static readonly HighlightHierarchy hierarchy = new HighlightHierarchy();

        public HighlightParams Params;

        public void Activate(bool logging) {
            hierarchy.Build(Params);
            hierarchy.ApplyDiff(logging, Params);
        }

    }

}