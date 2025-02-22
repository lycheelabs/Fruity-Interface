using UnityEngine;
using System.Collections;

namespace LycheeLabs.FruityInterface {

    public interface MouseTarget {

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