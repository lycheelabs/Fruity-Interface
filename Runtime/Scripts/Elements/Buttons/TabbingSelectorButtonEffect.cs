namespace LycheeLabs.FruityInterface.Elements.Buttons {

    public class TabbingSelectorButtonEffect : ButtonEffect {

        public TabbingSelectorComponent Type;
        public TabbingSelector Parent;

        public override void Activate (MouseButton clickButton) {
            Parent.Activate(Type, clickButton);
        }

        public override void MouseOver () {
            Parent.MouseOver(Type);
        }

    }

}