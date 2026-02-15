using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// An abstract UINode that implements GrabBehaviour. 
    /// Since this is a MonoBehaviour, inheriting classes can provide grabbing behaviour to GameObjects.
    /// </summary>
    public abstract class GrabbableNode : InterfaceNode, ClickTarget, DragTarget, GrabBehaviour {

        public bool IsHighlighted => GrabTarget.IsHovering;
        public bool IsDragging => GrabTarget.IsGrabbed;

        // Create a Grabber pointing back to this instance
        private GrabTarget _grabber;
        protected GrabTarget GrabTarget {
            get {
                if (_grabber == null) {
                    _grabber = new GrabTarget(this);
                }
                return _grabber;
            }
        }

        // Pass mouse events onto the grabber
        public override MouseTarget GetMouseTarget(Vector3 mouseWorldPosition, MouseButton pressedButton) => GrabTarget;

        // Stub my mouse events (delegated to GrabTarget)
        public void MouseHovering (bool isFirstFrame, HighlightParams highlightParams) {}
        public void MouseHoverEnd () {}
        public void MouseClick (ClickParams clickParams) { }
        public void MouseDragging (bool isFirstFrame, DragParams dragParams) { }
        public void CompleteMouseDrag(DragParams dragParams) { }
        public void CancelMouseDrag() { }
        public DragTarget.DragMode GetDragMode(MouseButton dragButton) => DragTarget.DragMode.Hold;

        // The grabber will call these methods back
        public abstract void OnHovering (bool isFirstFrame);
        public abstract void OnHoverEnd ();
        public abstract void OnButtonClick(MouseButton button);

        public abstract void OnGrabbing (bool isFirstFrame, MouseTarget draggingOver);
        public abstract void OnGrabCompleted (MouseTarget draggingOver);
        public abstract void OnGrabCancelled();

        // These configuration properties can be overridden if desired
        public virtual bool GrabbingIsEnabled => InputEnabledInHierarchy;
        public virtual GrabBehaviour.Mode GetMode(MouseButton mouseButton) =>
            (mouseButton == MouseButton.Left) ? GrabBehaviour.Mode.GRAB : GrabBehaviour.Mode.DISABLED;
        public virtual bool CanMultiPlace(MouseTarget target) => false;
        public virtual bool CanPassGrabTo(GrabTarget clickDragger) => false;

    }

}