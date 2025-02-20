using UnityEngine;
using System.Collections;

namespace LycheeLabs.FruityInterface {

	public struct DragParams {

		public static readonly DragParams Null = new DragParams(null, null, Vector2.zero, Vector2.zero, MouseButton.None);

		private readonly DragTarget target;
		private readonly MouseTarget draggingOver;
		private readonly Vector2 startPos;
		private readonly Vector2 endPos;
		private readonly MouseButton dragButton;

		public DragParams (DragTarget target, MouseTarget draggingOver, Vector2 startPos, Vector2 endPos, MouseButton dragButton) {
			this.target = target;
			this.draggingOver = draggingOver;
			this.startPos = startPos;
			this.endPos = endPos;
			this.dragButton = dragButton;
		}
		
		public Vector3 OriginalWorldPosition => InterfaceConfig.ScreenPointToWorldPoint(startPos);
		public Vector3 MouseWorldPosition => InterfaceConfig.ScreenPointToWorldPoint(endPos);
        public Vector3 WorldDragDisplacement => (MouseWorldPosition - OriginalWorldPosition);

		public Vector2 OriginalUIPosition => startPos;
		public Vector2 MouseUIPosition => endPos;
		public Vector2 UIDragDisplacement => MouseUIPosition - OriginalUIPosition;
		
		public MouseButton DragButton => dragButton;
		public DragTarget Target => target; 
		public MouseTarget DraggingOver => draggingOver;
		public bool IsOutsideCollider => target != draggingOver;

	}

}