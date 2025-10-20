
using UnityEngine;

namespace LycheeLabs.FruityInterface.Flow {

    public class TransitionEvent {

        public TransitionSequenceLayer Callbacks;

        private StageTransition newStage;
        private ScreenTransition screenIn;
        private ScreenTransition screenOut;

        private bool hasStarted;
        private bool hasTransitioned;
        private Color color;

        public TransitionEvent(StageTransition newStage) {
            this.newStage = newStage;
            color = Color.black;
        }

        public TransitionEvent(StageTransition newStage, ScreenTransition screenIn) {
            this.newStage = newStage;
            this.screenIn = screenIn;
            this.screenOut = screenIn;
            color = Color.black;
        }

        public TransitionEvent(StageTransition newStage, ScreenTransition screenIn, Color color) {
            this.newStage = newStage;
            this.screenIn = screenIn;
            this.screenOut = screenIn;
            this.color = color;
        }

        public TransitionEvent (StageTransition newStage, ScreenTransition screenIn, ScreenTransition screenOut) {
            this.newStage = newStage;
            this.screenIn = screenIn;
            this.screenOut = screenOut;
            color = Color.black;
        }

        public TransitionEvent(StageTransition newStage, ScreenTransition screenIn, ScreenTransition screenOut, Color color) {
            this.newStage = newStage;
            this.screenIn = screenIn;
            this.screenOut = screenOut;
            this.color = color;
        }

        public void Update () {
            if (!hasStarted) {
                hasStarted = true;
                screenIn.Controller.Enter(screenIn.Transition, screenIn.Config, color);
            }
            if (hasStarted && !hasTransitioned && screenIn.Controller.HasFullyEntered) {
                hasTransitioned = true;
                Callbacks?.OnTransitionApplied();
                newStage.Apply();
                screenIn.Controller.Exit(screenOut.Transition, screenOut.Config);
            }
        }

        public bool IsComplete => hasTransitioned;

    }

}