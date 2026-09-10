using UnityEngine;
using Unity.Scripting.LifecycleManagement;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Event fired every frame to update which target is being highlighted.
    /// Manages MouseHovering and MouseHoverEnd calls on targets.
    /// </summary>
    public partial class HoverHierarchyEvent : ControlEvent {

        [AutoStaticsCleanup]
        private static HoverHierarchy hierarchy = new HoverHierarchy();

        public MouseTarget Target;
        public HoverParams Params;

        public void Activate(bool logging) {
            hierarchy.Build(Target, Params);
            hierarchy.ApplyDiff(logging, Params);
        }

    }

}
