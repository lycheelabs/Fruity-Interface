using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// An abstract class that implements GrabBehaviour and provides a MouseTarget.
    /// This is not a MonoBehaviour, meaning it allows grabbing behaviour without requiring a GameObject.
    /// </summary>
    public abstract class Grabbable : GrabBehaviour {

        public bool IsHighlighted => GrabTarget.IsHovering;
        public bool IsGrabbed => GrabTarget.IsGrabbed;

        // Create a Grabber pointing back to this instance
        private GrabTarget _grabber;
        public GrabTarget GrabTarget {
            get {
                if (_grabber == null) {
                    _grabber = new GrabTarget(this);
                }
                return _grabber;
            }
        }

        public MouseTarget MouseTarget => GrabTarget;

        // The GrabTarget will call these methods for its behaviour
        public abstract void OnHovering(bool isFirstFrame);
        public abstract void OnHoverEnd();
        public abstract void OnButtonClick(MouseButton button);

        public abstract void OnGrabbing(bool isFirstFrame, MouseTarget draggingOver);
        public abstract void OnGrabCompleted(MouseTarget draggingOver);
        public abstract void OnGrabCancelled();

        // These configuration properties can be overridden if desired
        public virtual bool GrabbingIsEnabled => true;
        public virtual GrabBehaviour.Mode GetMode(MouseButton mouseButton) =>
            (mouseButton == MouseButton.Left) ? GrabBehaviour.Mode.GRAB : GrabBehaviour.Mode.DISABLED;
        public virtual bool CanMultiPlace(MouseTarget target) => false;
        public virtual bool CanPassGrabTo(GrabTarget clickDragger) => false;

    }

}