using UnityEngine;

namespace LycheeLabs.FruityInterface
{
    public class HighlightEvent : InterfaceEvent {

        public HighlightParams Params;
        public void Activate(bool logging) {
            var newTarget = Params.Target;
            var firstFrame = (newTarget != FruityUI.HighlightedTarget);

            if (firstFrame) {
                FruityUI.HighlightedTarget?.MouseDehighlight();
                FruityUI.HighlightedTarget = newTarget;
                if (logging) {
                    Debug.Log("Highlight: " + newTarget);
                }
            }

            FruityUI.HighlightedTarget?.MouseHighlight(firstFrame, Params);
        }
    }
}