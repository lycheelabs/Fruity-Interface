using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class HighlightEvent : InterfaceEvent {

        public HighlightParams Params;
        public void Activate(bool logging) {
            var newTarget = Params.Target;
            var firstFrame = (newTarget != InterfaceTargets.Highlighted);

            if (firstFrame) {
                InterfaceTargets.Highlighted?.MouseDehighlight();
                InterfaceTargets.Highlighted = newTarget;
                if (logging) {
                    Debug.Log("Highlight: " + newTarget);
                }
            }

            InterfaceTargets.Highlighted?.MouseHighlight(firstFrame, Params);
        }
    }
}