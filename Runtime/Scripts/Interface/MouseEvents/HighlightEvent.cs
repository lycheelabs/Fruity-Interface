namespace LycheeLabs.FruityInterface {

    public class HighlightEvent : InterfaceEvent {

        private static readonly HighlightHierarchy hierarchy = new HighlightHierarchy();

        public HighlightParams Params;

        public void Activate(bool logging) {
            hierarchy.Build(Params);
            hierarchy.ApplyDiff(logging, Params);
        }

    }

}