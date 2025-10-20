using UnityEngine;

namespace LycheeLabs.FruityInterface.Flow {

    public class ScreenTransitionController : MonoBehaviour {

        private ScreenTransition entering;
        private ScreenTransition exiting;
        private ScreenTransition bufferedExit;

        private Color activeColor;

        public bool HasFullyEntered => entering != null && entering.EntryIsComplete;
        public bool HasFullyExited => entering == null && exiting == null;

        private void Update() {

			// If an exit is buffered and the active transition has fully entered, perform the exit
            if (HasFullyEntered && bufferedExit != null && exiting == null) {
                ApplyExit();
            }

			// Clear exiting transition
            if (exiting != null) {
                if (exiting.ExitIsComplete) {
                    exiting = null;
                }
            }

        }

        public void Enter(ScreenTransition transition, Color color, bool jump = false) {
            if (entering == null && exiting == null) {
                ApplyEnter(transition, color, jump);
                return;
            }
        }

        public void Exit(ScreenTransition transition = null) {
            if (transition == null && entering == null) {
                return; // nothing to exit
            }
            bufferedExit = transition ?? entering;
        }

        // ------------------------------------------------------

        private void ApplyEnter(ScreenTransition transition, Color color, bool jump) {
            if (transition == null) {
                return;
            }

            entering = transition;
            activeColor = color;
            transition.Enter(jump);
        }

		private void ApplyExit() {
			if (bufferedExit == null || exiting != null) return;

            exiting = bufferedExit;

            if (entering != exiting) {
                entering?.Exit(jump: true);
                exiting.Enter(jump: true);
            }
            exiting.Exit();

            bufferedExit = null;
            entering = null;
		}

    }

}