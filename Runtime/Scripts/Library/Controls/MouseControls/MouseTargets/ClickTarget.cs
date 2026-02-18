namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Interface for objects that can be clicked with the mouse.
    /// Extends MouseTarget to also receive hover events.
    /// </summary>
    public interface ClickTarget : MouseTarget {

        /// <summary>
        /// Called when this target is clicked.
        /// By default, this is called on mouse button up.
        /// Override ClickOnMouseDown to change to mouse button down.
        /// </summary>
        void ApplyMouseClick(ClickParams clickParams);

        /// <summary>
        /// Called when a different target is being clicked while this target was the last selected.
        /// Return false to block the new click and keep this target selected.
        /// </summary>
        bool TryMouseUnclick(ClickParams clickParams) { return true; }

        /// <summary>
        /// When true, MouseClick is called on mouse button down instead of up.
        /// </summary>
        bool ClickOnMouseDown { get => false; }

    }

}