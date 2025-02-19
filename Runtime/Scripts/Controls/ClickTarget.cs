namespace LycheeLabs.FruityInterface {

    public interface ClickTarget : MouseTarget {

        /// <summary>
        /// Called when this target is clicked (default = mouse button up)
        /// </summary>
        void MouseClick (ClickParams clickParams);

        /// <summary>
        /// Called when this target was clicked last, and now a different target has been clicked (default =mouse button up).
        /// If false is returned, the click target wont change and the new target wont be clicked.
        /// </summary>
        bool TryMouseUnclick (ClickParams clickParams) { return true; }

        /// <summary>
        /// Override this method to make the click events happen on mouse down
        /// </summary>
        bool ClickOnMouseDown { get => false; }

    }

}