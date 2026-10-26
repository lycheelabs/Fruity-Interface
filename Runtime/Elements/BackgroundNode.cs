using LycheeLabs.FruityInterface;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public abstract class BackgroundNode : InterfaceNode {

        [UnityEngine.SerializeField, UnityEngine.Range(0f, 1f)]
        protected float maxEntry = 1f;

        [SerializeField]
        protected float transitionSpeed = 3f;

        private bool show;
        private bool activeShow;
        private float tween;

        public float MaxEntry {
            get => maxEntry;
            set => maxEntry = value;
        }

        public float TransitionSpeed {
            get => transitionSpeed;
            set => transitionSpeed = value;
        }

        protected float Tween => tween;
        public bool IsActive => tween > 0;
        public bool IsEntering => activeShow;

        protected virtual bool WaitForFullTransition => true;

        protected virtual void Awake() {
            OnInitialise();
            ApplyTween(tween);
        }

        protected virtual void OnInitialise() { }

        protected abstract void ApplyTween(float tween);

        private void Update() {
            if (WaitForFullTransition) {
                if (show && !activeShow && tween <= 0)
                    activeShow = true;

                if (!show && activeShow && tween >= 1)
                    activeShow = false;
            }
            else {
                activeShow = show;
            }

            var target = activeShow ? 1f : 0f;
            tween = Mathf.MoveTowards(tween, target, transitionSpeed * Time.unscaledDeltaTime);
            ApplyTween(tween);
        }

        public virtual void Show(bool jump = false) {
            show = true;
            if (!jump) return;

            activeShow = true;
            tween = 1f;
            ApplyTween(tween);
        }

        public virtual void Hide(bool jump = false) {
            show = false;
            if (!jump) return;

            activeShow = false;
            tween = 0;
            ApplyTween(tween);
        }

        protected static float EaseInOutQuad(float value) {
            return value < 0.5f
                ? 2f * value * value
                : 1f - Mathf.Pow(-2f * value + 2f, 2f) / 2f;
        }

    }

}
