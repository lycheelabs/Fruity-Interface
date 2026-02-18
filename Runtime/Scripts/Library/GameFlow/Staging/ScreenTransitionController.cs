using UnityEngine;

namespace LycheeLabs.FruityInterface.Flow {

    public class ScreenTransitionController : MonoBehaviour {

        private ScreenTransitionNode entering;
        private ScreenTransitionNode exiting;
        private ScreenTransitionNode bufferedExit;
        private ScreenTransitionConfig bufferedConfig;

        private Color activeColor;

        public bool HasFullyEntered => entering != null && entering.EntryIsComplete && !PreventCompleteExit;
        public bool HasFullyExited => entering == null && exiting == null;

        public bool PreventStartEntry;
        public bool PreventCompleteExit;

        private void Update() {

            // Update entering transition
            if (entering != null) {
                entering.SetColor(activeColor);
            }

            // If an exit is buffered and the active transition has fully entered, perform the exit
            if (HasFullyEntered && bufferedExit != null && exiting == null) {
                ApplyExit();
            }

            // Update exiting transition
            if (exiting != null) {
                exiting.SetColor(activeColor);

                if (exiting.ExitIsComplete) {
                    exiting = null;
                }
            }

        }

        public void Enter(ScreenTransitionNode transition, ScreenTransitionConfig config, Color color, bool jump = false) {
            if (entering == null && exiting == null) {
                ApplyEnter(transition, config, color, jump);
                return;
            }
        }

        public void Exit(ScreenTransitionNode transition, ScreenTransitionConfig config) {
            if (transition == null && entering == null) {
                return; // nothing to exit
            }
            bufferedExit = transition ?? entering;
            bufferedConfig = config;
        }

        // ------------------------------------------------------

        private void ApplyEnter(ScreenTransitionNode transition, ScreenTransitionConfig config, Color color, bool jump) {
            if (transition == null) {
                return;
            }

            entering = transition;
            activeColor = color;
            transition.Enter(config, jump);
            transition.SetColor(activeColor);
        }

        private void ApplyExit() {
            if (bufferedExit == null || exiting != null) return;

            exiting = bufferedExit;

            if (entering != exiting) {
                entering?.Exit(bufferedConfig, jump: true);
                exiting.Enter(bufferedConfig, jump: true);
            }
            exiting.Exit(bufferedConfig);
            exiting.SetColor(activeColor);

            bufferedExit = null;
            entering = null;
        }

    }

}