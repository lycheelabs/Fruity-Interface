namespace LycheeLabs.FruityInterface.Elements.Buttons {

    public class ArrowSelectorEffect : ButtonEffect {

        public ArrowSelectorComponent Type;
        public ArrowSelector Parent;

        public override void Activate (MouseButton clickButton) {
            Parent.Activate(Type, clickButton);
        }

        public override void MouseOver () {
            Parent.MouseOver(Type);
        }

    }

}