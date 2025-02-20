using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// An abstract UINode that implements GrabBehaviour. 
    /// Since this is a MonoBehaviour, inheriting classes can provide grabbing behaviour to GameObjects.
    /// </summary>
    public abstract class GrabbableNode : InterfaceNode, ClickTarget, DragTarget, GrabBehaviour {

        public bool IsHighlighted => GrabTarget.IsHighlighted;
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

        public MouseTarget MouseTarget => GrabTarget;

        // Pass mouse events onto the grabber
        MouseTarget MouseTarget.ResolveTarget (Vector3 mouseWorldPosition) => GrabTarget;

        // Stub my mouse events
        public void MouseHighlight (bool firstFrame, HighlightParams highlightParams) {}
        public void MouseDehighlight () {}
        public void MouseClick(ClickParams clickParams) { }
        public void StartMouseDrag(DragParams dragParams) { }
        public void UpdateMouseDrag(DragParams dragParams) { }
        public void CompleteMouseDrag(DragParams dragParams) { }
        public void CancelMouseDrag() { }
        public bool DraggingIsEnabled => true;

        // The grabber will call these methods back
        public abstract void OnHighlight (bool firstFrame);
        public abstract void OnDehighlight ();
        public abstract void OnButtonClick(MouseButton button);

        public abstract void OnGrabStart ();
        public abstract void OnGrabUpdate (MouseTarget draggingOver);
        public abstract void OnGrabCompleted (MouseTarget draggingOver);
        public abstract void OnGrabCancelled();
        public abstract void OnGrabEnd ();

        // These configuration properties can be overridden if desired
        public virtual bool GrabbingIsEnabled => InputEnabledInHierarchy;
        public virtual GrabBehaviour.Mode GetMode(MouseButton mouseButton) =>
            (mouseButton == MouseButton.Left) ? GrabBehaviour.Mode.GRAB : GrabBehaviour.Mode.DISABLED;
        public virtual bool CanMultiPlace(MouseTarget target) => false;
        public virtual bool CanPassGrabTo(GrabTarget clickDragger) => false;

    }

}