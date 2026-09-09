using UnityEngine;
using System.Collections;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Parameters passed to DragTarget methods.
    /// Contains information about the drag state including positions and the drop target.
    /// </summary>
    public struct DragParams {

        public static readonly DragParams Null = new DragParams(null, null, Vector2.zero, Vector2.zero, MouseButton.None);

        private readonly DragTarget target;
        private readonly DraggedOverTarget draggingOver;
        private readonly Vector2 startScreenPos;
        private readonly Vector2 currentScreenPos;
        private readonly MouseButton dragButton;

        /// <param name="target">The target being dragged.</param>
        /// <param name="draggingOver">The DragOverTarget under the mouse cursor (for drop detection).</param>
        /// <param name="startScreenPos">Screen position where the drag started.</param>
        /// <param name="currentScreenPos">Current mouse screen position.</param>
        /// <param name="dragButton">Which mouse button is being used for the drag.</param>
        public DragParams(DragTarget target, DraggedOverTarget draggingOver, Vector2 startScreenPos, Vector2 currentScreenPos, MouseButton dragButton) {
            this.target = target;
            this.draggingOver = draggingOver;
            this.startScreenPos = startScreenPos;
            this.currentScreenPos = currentScreenPos;
            this.dragButton = dragButton;
        }

        /// <summary>The target being dragged.</summary>
        public DragTarget Target => target;
        
        /// <summary>
        /// The DragOverTarget currently under the mouse cursor.
        /// Use this to detect what the dragged item would be dropped onto.
        /// Null if the mouse is not over a valid drop target.
        /// </summary>
        public DraggedOverTarget DraggingOver => draggingOver;
        
        /// <summary>Which mouse button is being used for the drag.</summary>
        public MouseButton DragButton => dragButton;
        
        /// <summary>True if the mouse is not over a valid drop target.</summary>
        public bool IsOutsideDropTarget => draggingOver == null;

        // Position at drag start
        /// <summary>Screen position where the drag started.</summary>
        public Vector2 OriginalUIPosition => startScreenPos;
        
        /// <summary>World position (on world plane) where the drag started.</summary>
        public Vector3 OriginalWorldPosition => FruityUI.ScreenPointToWorldPoint(startScreenPos);

        // Current position
        /// <summary>Current mouse screen position.</summary>
        public Vector2 MouseUIPosition => currentScreenPos;
        
        /// <summary>Current mouse world position (on world plane).</summary>
        public Vector3 MouseWorldPosition => FruityUI.ScreenPointToWorldPoint(currentScreenPos);

        // Displacement
        /// <summary>Screen-space displacement from drag start to current position.</summary>
        public Vector2 UIDragDisplacement => currentScreenPos - startScreenPos;
        
        /// <summary>World-space displacement from drag start to current position.</summary>
        public Vector3 WorldDragDisplacement => MouseWorldPosition - OriginalWorldPosition;

    }

}