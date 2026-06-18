using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public abstract class ButtonEffect : MonoBehaviour {

        public abstract void MouseOver (); 
        public virtual bool MouseButtonIsPermitted (MouseButton clickButton) => clickButton == MouseButton.Left;
        public abstract void Activate(MouseButton clickButton);
        public virtual bool TryUnclick(MouseButton clickButton) => true;

    }

}