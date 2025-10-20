using UnityEngine;

namespace LycheeLabs.FruityInterface.Flow {
    public abstract class ScreenTransition : InterfaceNode {

        public enum Mode { ENTER, EXIT }

        public float Tween => entryTween;
        public bool IsEntering => currentMode == Mode.ENTER;
        public bool EntryIsComplete => IsEntering && entryTween >= 1;
        public bool ExitIsComplete => !IsEntering && entryTween <= 0;

        [SerializeField] private Mode currentMode;
        [SerializeField] [Range(0, 1)] private float entryTween;
        [SerializeField] private Color color;

        private void OnValidate() {
            Refresh(IsEntering, entryTween);
            SetColor(color);
        }

        private void Awake() {
            currentMode = Mode.EXIT;
            entryTween = 0;
            Refresh(IsEntering, entryTween);
            SetColor(color);
        }

        internal void Enter(bool jump = false) {
            currentMode = Mode.ENTER;
            if (jump) {
                entryTween = 1;
            }
            Refresh(IsEntering, entryTween);
        }

        internal void Exit(bool jump = false) {
            currentMode = Mode.EXIT;
            if (jump) {
                entryTween = 0;
            }
            Refresh(IsEntering, entryTween);
        }

        public void LateUpdate() {
            entryTween = entryTween.MoveTowards(IsEntering, TransitionSpeed);
            Refresh(IsEntering, entryTween);
        }

        protected abstract float TransitionSpeed { get; }
        protected abstract void Refresh(bool isEntering, float tween);
        public abstract void SetColor(Color color);

    }


}