
namespace LycheeLabs.FruityInterface {

    public interface MouseTarget {

        /// <summary>
        /// Called on every frame where the mouse is hovering over this target
        /// </summary>
        void MouseHovering (bool isFirstFrame, HighlightParams highlightParams);

        /// <summary>
        /// Called when this target stops being hovered
        /// </summary>
        void MouseHoverEnd ();

    }

}