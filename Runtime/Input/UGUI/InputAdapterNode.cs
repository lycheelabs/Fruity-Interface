using UnityEngine;
using UnityEngine.EventSystems;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// Adapts Fruity pointer interactions into UGUI event callbacks.
    /// Place this on the same GameObject as the UGUI control it should drive.
    /// </summary>
    public sealed class InputAdapterNode : InterfaceNode, ClickTarget, DragTarget, DraggedOverTarget {

        [SerializeField] private MouseDragMode dragMode = MouseDragMode.Disabled;
        [SerializeField] private bool clickOnMouseDown;
        [SerializeField] private bool receivesDragOver;

        private PointerEventData pointerData;
        private EventSystem pointerEventSystem;
        private GameObject pointerPress;
        private GameObject pointerDrag;
        private bool pointerIsDown;
        private bool dragStarted;
        private Vector2 previousPosition;

        public MouseDragMode DragMode {
            get => dragMode;
            set => dragMode = value;
        }

        public bool ClickOnMouseDown => clickOnMouseDown;
        public bool ReceivesDragOver => receivesDragOver;

        public override MouseTarget GetMouseTarget(Vector3 mouseWorldPosition, MouseButton pressedButton) {
            return this;
        }

        public void UpdateMouseHover(bool firstFrame, HoverParams hoverParams) {
            SetPointerPosition(FruityUI.RawMouseScreenPosition);
            if (firstFrame) {
                Execute(ExecuteEvents.pointerEnterHandler);
            }

            if (hoverParams.PressButton != MouseButton.None && !pointerIsDown) {
                BeginPointer(hoverParams.PressButton);
            }
        }

        public void EndMouseHover() {
            Execute(ExecuteEvents.pointerExitHandler);
        }

        public void ApplyMouseClick(ClickParams clickParams) {
            SetPointerPosition(FruityUI.RawMouseScreenPosition);
            if (!pointerIsDown) {
                BeginPointer(clickParams.ClickButton);
            }

            Execute(ExecuteEvents.pointerUpHandler);
            Execute(ExecuteEvents.pointerClickHandler);
            ClearPointerPress();
        }

        public bool TryMouseUnclick(ClickParams clickParams) {
            if (!pointerIsDown) return true;

            SetPointerPosition(FruityUI.RawMouseScreenPosition);
            Execute(ExecuteEvents.pointerUpHandler);
            ClearPointerPress();
            return true;
        }

        public MouseDragMode GetDragMode(MouseButton dragButton) {
            return dragMode;
        }

        public void UpdateMouseDragging(bool isFirstFrame, DragParams dragParams) {
            SetPointerPosition(dragParams.MouseUIPosition);
            if (!pointerIsDown) BeginPointer(dragParams.DragButton);

            if (isFirstFrame) {
                pointerDrag = gameObject;
                pointerData.pointerDrag = pointerDrag;
                dragStarted = true;
                Execute(ExecuteEvents.beginDragHandler);
            }

            Execute(ExecuteEvents.dragHandler);
        }

        public void ApplyMouseDrag(DragParams dragParams) {
            SetPointerPosition(dragParams.MouseUIPosition);
            if (!dragStarted) return;

            if (dragParams.DraggingOver is Component dropTarget) {
                ExecuteEvents.ExecuteHierarchy(
                    dropTarget.gameObject,
                    pointerData,
                    ExecuteEvents.dropHandler);
            }

            Execute(ExecuteEvents.endDragHandler);
            dragStarted = false;
            pointerDrag = null;
        }

        public void CancelMouseDrag() {
            if (!dragStarted) return;

            Execute(ExecuteEvents.endDragHandler);
            dragStarted = false;
            pointerDrag = null;
        }

        public void UpdateMouseDraggedOver(bool isFirstFrame, DragParams dragParams) {
            SetPointerPosition(dragParams.MouseUIPosition);
        }

        public void EndMouseDraggedOver() { }

        private void BeginPointer(MouseButton button) {
            EnsurePointerData();
            pointerData.button = ToPointerButton(button);
            pointerData.pressPosition = pointerData.position;
            pointerData.eligibleForClick = true;
            pointerData.clickCount = 1;
            pointerData.rawPointerPress = gameObject;
            pointerPress = ExecuteEvents.ExecuteHierarchy(
                gameObject,
                pointerData,
                ExecuteEvents.pointerDownHandler) ?? gameObject;
            pointerData.pointerPress = pointerPress;
            pointerIsDown = true;
        }

        private void SetPointerPosition(Vector2 position) {
            EnsurePointerData();
            pointerData.delta = position - pointerData.position;
            pointerData.position = position;
            previousPosition = position;
        }

        private void ClearPointerPress() {
            pointerData.pointerPress = null;
            pointerData.rawPointerPress = null;
            pointerData.pointerDrag = null;
            pointerData.eligibleForClick = false;
            pointerPress = null;
            pointerIsDown = false;
        }

        private void Execute<T>(ExecuteEvents.EventFunction<T> handler) where T : IEventSystemHandler {
            EnsurePointerData();
            ExecuteEvents.ExecuteHierarchy(gameObject, pointerData, handler);
        }

        private void EnsurePointerData() {
            var eventSystem = EventSystem.current;
            if (eventSystem == null) return;
            if (pointerData == null || pointerEventSystem != eventSystem) {
                pointerData = new PointerEventData(eventSystem) {
                    pointerId = 0,
                    position = previousPosition
                };
                pointerEventSystem = eventSystem;
            }
        }

        private static PointerEventData.InputButton ToPointerButton(MouseButton button) {
            switch (button) {
                case MouseButton.Right:
                    return PointerEventData.InputButton.Right;
                case MouseButton.Middle:
                    return PointerEventData.InputButton.Middle;
                default:
                    return PointerEventData.InputButton.Left;
            }
        }

    }

}
