using UnityEngine;
using System.Collections;

namespace LycheeLabs.FruityInterface {

    public interface MouseTarget {

        /// <summary>
        /// Implement this method to pass the mouse events onto a different target (like a grabber)
        /// </summary>
        MouseTarget ResolveTarget (Vector3 mouseWorldPosition) => this;

        /// <summary>
        /// Called on every frame where the mouse is highlighting this target
        /// </summary>
        void MouseHighlight (bool firstFrame, HighlightParams highlightParams);

        /// <summary>
        /// Called when this target stops being highlighted
        /// </summary>
        void MouseDehighlight ();

    }

}