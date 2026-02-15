namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// A Grabbable can be dragged and also 'picked up' (by clicking it, then clicking again to place it).
    /// </summary>
    public interface GrabBehaviour {

        /// <summary> If false, all input will be discarded. </summary>
        bool GrabbingIsEnabled { get; }

        /// <summary> Determines how this object behaves for each mouse button. </summary>
        DragTarget.DragMode GetDragMode (MouseButton mouseButton);

        /// <summary> If true, placing the Grabbable will not end the grab. </summary>
        bool CanMultiPlace(MouseTarget placeTarget) => false;

        /// <summary> When true, clicking this target will immediately drop the old target and grab this instead. </summary>
        bool CanPassGrabTo(GrabTarget newDragger) => false;

        // Highlight events
        void OnHovering (bool firstFrame);
        void OnHoverEnd ();

        // Grab events
        void OnGrabbing (bool isFirstFrame, MouseTarget placeTarget);
        void OnGrabCompleted (MouseTarget placeTarget);
        void OnGrabCancelled();

        /// <summary> When in Disabled mode but still a ClickTarget, this event is called. </summary>
        void OnButtonClick (MouseButton mouseButton);

    }

}