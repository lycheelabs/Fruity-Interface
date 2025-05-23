﻿
namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// A Grabbable can be dragged and also 'picked up' (by clicking it, then clicking again to place it).
    /// </summary>
    public interface GrabBehaviour {

        /// <summary>
        /// Toggles the behaviour of a click dragger
        /// </summary>
        public enum Mode {
            GRAB, // Full functionality
            ONLY_DRAG,  // Behave like a draggable (no clicking)
            ONLY_CLICK, // Behave like a button (no dragging)
            DISABLED, // No functionality
        }

        /// <summary> If false, all input will be discarded. </summary>
        bool GrabbingIsEnabled { get; }

        /// <summary> Determines how this object behaves for each mouse button. </summary>
        Mode GetMode (MouseButton mouseButton);

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

        /// <summary> When in ONLY_CLICK mode, this event is called instead of the Grab events. </summary>
        void OnButtonClick (MouseButton mouseButton);

    }

}