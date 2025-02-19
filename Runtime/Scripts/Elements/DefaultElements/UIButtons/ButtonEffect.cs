using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {
    public abstract class ButtonEffect : MonoBehaviour {

        public abstract void MouseOver();
        public virtual bool MouseButtonIsPermitted(MouseButton clickButton) => clickButton == MouseButton.Left;
        public virtual Vector3 EntryOffset { get; }

        // -----------------------------------------------------

        public void OnEnable() {
            UpdateLayout();
        }

        protected abstract void UpdateLayout();

    }

    public abstract class ClickButtonEffect : ButtonEffect {

        public abstract void Activate(MouseButton clickButton);
        public virtual bool TryUnclick(MouseButton clickButton) => true;

    }

}