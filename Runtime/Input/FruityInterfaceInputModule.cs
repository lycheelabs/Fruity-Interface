using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LycheeLabs.FruityInterface {

    [RequireComponent(typeof(EventSystem))]
    public sealed class FruityInterfaceInputModule : BaseInputModule {

        [Header("Pointer Actions")]
        public InputActionAsset actionsAsset;
        public InputActionReference point;
        public InputActionReference leftClick;
        public InputActionReference rightClick;
        public InputActionReference middleClick;
        public InputActionReference scrollWheel;

        [Header("Diagnostics")]
        public bool logEvents;
        public bool logRawInput;
        public bool logRaycasts;

        private DefaultInputActions defaultActions;
        private FruityInputRuntime inputRuntime;
        [System.NonSerialized] private InputActionAsset validatedActionsAsset;
        private bool actionsSubscribed;
        private bool leftPressed;
        private bool rightPressed;
        private bool middlePressed;

        private InputAction PointAction => point?.action;
        private InputAction LeftClickAction => leftClick?.action;
        private InputAction RightClickAction => rightClick?.action;
        private InputAction MiddleClickAction => middleClick?.action;
        private InputAction ScrollWheelAction => scrollWheel?.action;

        protected override void Awake() {
            base.Awake();
            FruityUI.SetUICamera(Camera.main);
            FruityUI.SetWorldPlane(new Plane(Vector3.up, Vector3.zero));
            inputRuntime = new FruityInputRuntime(logEvents, logRawInput, logRaycasts);
            AssignActionsIfNeeded();
        }

        protected override void OnValidate() {
            base.OnValidate();
            if (actionsAsset == validatedActionsAsset) return;
            validatedActionsAsset = actionsAsset;
            AssignActionsFromAsset();
        }

        public void AssignDefaultActions() {
            if (defaultActions == null || defaultActions.asset == null) {
                defaultActions?.Dispose();
                defaultActions = new DefaultInputActions();
            }

            actionsAsset = defaultActions.asset;
            point = InputActionReference.Create(defaultActions.UI.Point);
            leftClick = InputActionReference.Create(defaultActions.UI.Click);
            rightClick = InputActionReference.Create(defaultActions.UI.RightClick);
            middleClick = InputActionReference.Create(defaultActions.UI.MiddleClick);
            scrollWheel = InputActionReference.Create(defaultActions.UI.ScrollWheel);
        }

        public void AssignActionsFromAsset() {
            if (actionsAsset == null) return;

            point = FindActionReference("UI/Point", "Point");
            leftClick = FindActionReference("UI/Click", "UI/LeftClick", "Click", "LeftClick");
            rightClick = FindActionReference("UI/RightClick", "RightClick");
            middleClick = FindActionReference("UI/MiddleClick", "MiddleClick");
            scrollWheel = FindActionReference("UI/ScrollWheel", "ScrollWheel", "Scroll");
        }

        private void AssignActionsIfNeeded() {
            if (actionsAsset != null) {
                AssignActionsFromAsset();
                return;
            }

            if (point == null && leftClick == null && rightClick == null &&
                middleClick == null && scrollWheel == null) {
                AssignDefaultActions();
            }
        }

        private InputActionReference FindActionReference(params string[] actionNames) {
            for (var i = 0; i < actionNames.Length; i++) {
                var action = actionsAsset.FindAction(actionNames[i], false);
                if (action != null) return InputActionReference.Create(action);
            }
            return null;
        }

        public override bool IsModuleSupported() {
            return true;
        }

        public override bool ShouldActivateModule() {
            return isActiveAndEnabled && inputRuntime != null;
        }

        public override void ActivateModule() {
            base.ActivateModule();
            SubscribeActions();
            EnableActions();
        }

        public override void DeactivateModule() {
            FruityInputRuntime.SubmitRawInput(RawInputEvent.Cancel(0, Vector2.zero, Time.unscaledTime));
            inputRuntime?.Process();
            UnsubscribeActions();
            DisableActions();
            leftPressed = false;
            rightPressed = false;
            middlePressed = false;
            base.DeactivateModule();
        }

        public override void Process() {
            inputRuntime?.Process();
        }

        private void SubscribeActions() {
            if (actionsSubscribed) return;
            actionsSubscribed = true;
            if (PointAction != null) PointAction.performed += OnPoint;
            if (LeftClickAction != null) {
                LeftClickAction.started += OnLeftStarted;
                LeftClickAction.performed += OnLeftPerformed;
                LeftClickAction.canceled += OnLeftCanceled;
            }
            if (RightClickAction != null) {
                RightClickAction.started += OnRightStarted;
                RightClickAction.performed += OnRightPerformed;
                RightClickAction.canceled += OnRightCanceled;
            }
            if (MiddleClickAction != null) {
                MiddleClickAction.started += OnMiddleStarted;
                MiddleClickAction.performed += OnMiddlePerformed;
                MiddleClickAction.canceled += OnMiddleCanceled;
            }
            if (ScrollWheelAction != null) ScrollWheelAction.performed += OnScrollWheel;
        }

        private void UnsubscribeActions() {
            if (!actionsSubscribed) return;
            actionsSubscribed = false;
            if (PointAction != null) PointAction.performed -= OnPoint;
            if (LeftClickAction != null) {
                LeftClickAction.started -= OnLeftStarted;
                LeftClickAction.performed -= OnLeftPerformed;
                LeftClickAction.canceled -= OnLeftCanceled;
            }
            if (RightClickAction != null) {
                RightClickAction.started -= OnRightStarted;
                RightClickAction.performed -= OnRightPerformed;
                RightClickAction.canceled -= OnRightCanceled;
            }
            if (MiddleClickAction != null) {
                MiddleClickAction.started -= OnMiddleStarted;
                MiddleClickAction.performed -= OnMiddlePerformed;
                MiddleClickAction.canceled -= OnMiddleCanceled;
            }
            if (ScrollWheelAction != null) ScrollWheelAction.performed -= OnScrollWheel;
        }

        private void EnableActions() {
            PointAction?.Enable();
            LeftClickAction?.Enable();
            RightClickAction?.Enable();
            MiddleClickAction?.Enable();
            ScrollWheelAction?.Enable();
        }

        private void DisableActions() {
            PointAction?.Disable();
            LeftClickAction?.Disable();
            RightClickAction?.Disable();
            MiddleClickAction?.Disable();
            ScrollWheelAction?.Disable();
        }

        private void OnPoint(InputAction.CallbackContext context) {
            FruityInputRuntime.SubmitRawInput(RawInputEvent.Move(0, context.ReadValue<Vector2>(), context.time));
        }

        private void OnScrollWheel(InputAction.CallbackContext context) {
            FruityInputRuntime.SubmitRawInput(RawInputEvent.Scroll(
                0,
                PointAction != null ? PointAction.ReadValue<Vector2>() : Vector2.zero,
                context.ReadValue<Vector2>(),
                context.time));
        }

        private void OnLeftStarted(InputAction.CallbackContext context) => UpdateButton(context, MouseButton.Left, true, ref leftPressed);
        private void OnLeftPerformed(InputAction.CallbackContext context) => UpdateButton(context, MouseButton.Left, context.ReadValue<float>() > 0f, ref leftPressed);
        private void OnLeftCanceled(InputAction.CallbackContext context) => UpdateButton(context, MouseButton.Left, false, ref leftPressed);
        private void OnRightStarted(InputAction.CallbackContext context) => UpdateButton(context, MouseButton.Right, true, ref rightPressed);
        private void OnRightPerformed(InputAction.CallbackContext context) => UpdateButton(context, MouseButton.Right, context.ReadValue<float>() > 0f, ref rightPressed);
        private void OnRightCanceled(InputAction.CallbackContext context) => UpdateButton(context, MouseButton.Right, false, ref rightPressed);
        private void OnMiddleStarted(InputAction.CallbackContext context) => UpdateButton(context, MouseButton.Middle, true, ref middlePressed);
        private void OnMiddlePerformed(InputAction.CallbackContext context) => UpdateButton(context, MouseButton.Middle, context.ReadValue<float>() > 0f, ref middlePressed);
        private void OnMiddleCanceled(InputAction.CallbackContext context) => UpdateButton(context, MouseButton.Middle, false, ref middlePressed);

        private void UpdateButton(
            InputAction.CallbackContext context,
            MouseButton button,
            bool pressed,
            ref bool previousPressed) {
            if (pressed == previousPressed) return;
            previousPressed = pressed;

            var position = PointAction != null ? PointAction.ReadValue<Vector2>() : Vector2.zero;
            var inputEvent = pressed
                ? RawInputEvent.ButtonDown(0, button, position, context.time)
                : RawInputEvent.ButtonUp(0, button, position, context.time);
            FruityInputRuntime.SubmitRawInput(inputEvent);
        }

        protected override void OnDestroy() {
            DeactivateModule();
            inputRuntime?.Dispose();
            inputRuntime = null;
            defaultActions?.Dispose();
            base.OnDestroy();
        }

    }

}
