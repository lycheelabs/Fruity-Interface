using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {
    public abstract class TabbingSelectorEffect : MonoBehaviour {

        public virtual bool MouseButtonIsPermitted (MouseButton clickButton) => clickButton == MouseButton.Left;
        public virtual bool TryUnclick (MouseButton clickButton) => true;

        public abstract void MouseOver (TabbingSelectorComponent type);
        public abstract void Activate (TabbingSelectorComponent type, MouseButton clickButton);

    }

}