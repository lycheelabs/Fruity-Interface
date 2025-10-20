
using UnityEngine;

namespace LycheeLabs.FruityInterface.Flow {

    public class TransitionEvent {

        private StageTransition newStage;
        private ScreenTransitionController screenController;
        private ScreenTransition screenIn;
        private ScreenTransition screenOut;

        private bool hasStarted;
        private bool hasTransitioned;

        public TransitionEvent(StageTransition newStage) {
            this.newStage = newStage;
        }

        public TransitionEvent (StageTransition newStage, ScreenTransitionController screenController, ScreenTransition screenIn, ScreenTransition screenOut) {
            this.screenIn = screenIn;

            this.screenController = screenController;
            this.newStage = newStage;
            this.screenOut = screenOut;
        }

        public void Update () {
            if (!hasStarted) {
                hasStarted = true;
                screenController.Enter(screenIn, Color.black);
            }
            if (hasStarted && !hasTransitioned && screenController.HasFullyEntered) {
                hasTransitioned = true;
                newStage.Apply();
                screenController.Exit(screenOut);
            }
        }

        public bool IsComplete => hasTransitioned;

    }

}