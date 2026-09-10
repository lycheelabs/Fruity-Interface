using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired every frame to update which target is being highlighted.
    /// Manages MouseHovering and MouseHoverEnd calls on targets.
    /// </summary>
    public partial class HoverHierarchyEvent : ControlEvent {

        private static HoverHierarchy hierarchy = new HoverHierarchy();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad() {
            hierarchy.Clear();
        }

        public MouseTarget Target;
        public HoverParams Params;

        public void Activate(bool logging) {
            hierarchy.Build(Target, Params);
            hierarchy.ApplyDiff(logging, Params);
        }

    }

}
