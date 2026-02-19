namespace LycheeLabs.FruityInterface.Elements {
    public abstract class TabbingButtonEffect : ButtonEffect {

        public abstract void Activate (MouseButton clickButton);
        public virtual bool TryUnclick (MouseButton clickButton) => true;

    }

}