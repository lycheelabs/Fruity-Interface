using System.Collections.Generic;
using System;

namespace LycheeLabs.FruityInterface.Flow {

    public class StageTransitionChart {

        private HashSet<StageTransitionLink> stages = new();
        public GameStage CurrentStage { get; private set; }

        public void AddTransition(GameStage from, GameStage to) {
            var transition = new StageTransitionLink {
                From = from,
                To = to
            };
            stages.Add(transition);
        }

        public void TransitionTo(GameStage nextStage) {
            if (CurrentStage != null) {
                var transition = new StageTransitionLink {
                    From = CurrentStage,
                    To = nextStage
                };
                if (!stages.Contains(transition)) {
                    throw new InvalidOperationException(String.Format(
                        "No stage transition exists from {0} to {1}", CurrentStage, nextStage));
                }
                CurrentStage.Close();
            }

            CurrentStage = nextStage;
            nextStage.Open();
        }

        public void Update () {
            if (CurrentStage != null) {
                CurrentStage.Update();
            }
        }

    }

}