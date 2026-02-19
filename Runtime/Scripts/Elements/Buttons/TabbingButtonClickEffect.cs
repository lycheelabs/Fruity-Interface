namespace LycheeLabs.FruityInterface.Elements.Buttons {
    public class TabbingButtonClickEffect : ClickButtonEffect {

        public TabbingSelector.Component Type;
        public TabbingSelector Parent;

        public TabbingButtonClickEffect () {
        }

        public override void Activate (MouseButton clickButton) {
            Parent.Activate(Type, clickButton);
        }

        public override void MouseOver () {
            Parent.MouseOver(Type);
        }

        protected override void UpdateLayout () {
            Parent.UpdateLayout(Type);
        }

    }

}