using UnityEngine;

namespace LycheeLabs.FruityInterface  {
    public interface InterfaceEvent {

        void Activate(bool logging);

    }

    public class HighlightEvent : InterfaceEvent {

        public HighlightParams HighlightParams;
        public void Activate(bool logging) {
            var newTarget = HighlightParams.Target;
            var firstFrame = (newTarget != InterfaceTargets.Highlighted);

            if (firstFrame) {
                InterfaceTargets.Highlighted?.MouseDehighlight();
                InterfaceTargets.Highlighted = newTarget;
                if (logging) {
                    Debug.Log("Highlight: " + newTarget);
                }
            }

            InterfaceTargets.Highlighted?.MouseHighlight(firstFrame, HighlightParams);
        }
    }

    public static class InterfaceTargets {

        public static MouseTarget Highlighted;

    }
    
}