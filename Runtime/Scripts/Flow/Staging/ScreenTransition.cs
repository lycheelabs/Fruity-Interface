namespace LycheeLabs.FruityInterface.Flow {

    public struct ScreenTransition {

        public readonly ScreenTransitionNode Transition;
        public readonly ScreenTransitionController Controller;
        public readonly ScreenTransitionConfig Config;

        public ScreenTransition(ScreenTransitionNode transition, ScreenTransitionController controller) {
            Transition = transition;
            Controller = controller;
            Config = new ScreenTransitionConfig();
        }

        public ScreenTransition SetConfig (string key, object value) {
            Config.SetConfig(key, value);
            return this;
        }

    }

}