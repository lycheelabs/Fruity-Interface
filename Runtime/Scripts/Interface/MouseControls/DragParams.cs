using UnityEngine;
using System.Collections;

namespace LycheeLabs.FruityInterface {

	public struct DragParams {

		public static readonly DragParams Null = new DragParams(null, null, Vector2.zero, Vector2.zero, MouseButton.None);

		private readonly DragTarget target;
		private readonly MouseTarget draggingOver;
		private readonly Vector2 startScreenPos;
		private readonly Vector2 endScreenPos;
		private readonly MouseButton dragButton;

		public DragParams (DragTarget target, MouseTarget draggingOver, Vector2 startScreenPos, Vector2 endScreenPos, MouseButton dragButton) {
			this.target = target;
			this.draggingOver = draggingOver;
			this.startScreenPos = startScreenPos;
			this.endScreenPos = endScreenPos;
			this.dragButton = dragButton;
		}
		
		public Vector3 OriginalWorldPosition => FruityUI.ScreenPointToWorldPoint(startScreenPos);
		public Vector3 MouseWorldPosition => FruityUI.ScreenPointToWorldPoint(endScreenPos);
        public Vector3 WorldDragDisplacement => (MouseWorldPosition - OriginalWorldPosition);

		public Vector2 OriginalUIPosition => startScreenPos;
		public Vector2 MouseUIPosition => endScreenPos;
		public Vector2 UIDragDisplacement => MouseUIPosition - OriginalUIPosition;
		
		public MouseButton DragButton => dragButton;
		public DragTarget Target => target; 
		public MouseTarget DraggingOver => draggingOver;
		public bool IsOutsideCollider => target != draggingOver;

	}

}