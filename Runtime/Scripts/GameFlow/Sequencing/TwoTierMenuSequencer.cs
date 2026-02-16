using LycheeLabs.FruityInterface.Elements;
using LycheeLabs.FruityInterface.Flow;

namespace LycheeLabs.FruityInterface {

    public enum TwoTierMenuLayer {
        GAMEPLAY, OVERLAY
    }

    /// <summary>
    /// This sequencer allows layering of a main menu prompt on top of in-game menu prompts.
    /// </summary>
    public class TwoTierMenuSequencer : EventSequencer {

        public bool IsIdle => !IsAnimating && !IsPrompting && !IsTransitioning;
        public bool IsTransitioning => Transitions.IsTransitioning;
        public bool IsAnimating => BlockingGameplay.IsAnimating;
        public bool IsPrompting => GamePrompts.IsPrompting || OverlayPrompts.IsPrompting;
        public bool IsOverlayPrompting => OverlayPrompts.IsPrompting;

        private TransitionSequenceLayer Transitions;
        private PromptSequenceLayer OverlayPrompts;
        private PromptSequenceLayer GamePrompts;
        private EventSequenceLayer BlockingGameplay;
        private GameplaySequenceLayer Gameplay;

        public TwoTierMenuSequencer(UICanvas gamePromptCanvas, UICanvas overlayPromptCanvas) {
            // Instantiate layers from bottom to top
            Gameplay = AddGameplayLayer();
            BlockingGameplay = AddEventLayer();
            GamePrompts = AddPromptLayer(gamePromptCanvas);
            OverlayPrompts = AddPromptLayer(overlayPromptCanvas);
            Transitions = AddTransitionLayer();
        }

        public bool PauseAllGameplay => OverlayPrompts.IsPrompting;

        public void Transition(TransitionEvent newEvent) {
            Transitions.Transition(newEvent);
        }

        public void Prompt(UIPrompt.PromptInstantiator newPrompt, TwoTierMenuLayer layer) {
            if (layer == TwoTierMenuLayer.OVERLAY) {
                OverlayPrompts.Prompt(newPrompt);
            } else {
                GamePrompts.Prompt(newPrompt);
            }
        }

        public void CloseOverlayPrompt () {
            OverlayPrompts.Close();
        }

        public void Queue(BlockingEvent newEvent) {
            BlockingGameplay.Queue(newEvent);
        }

        public void Execute(GameplayEvent newEvent) { 
            Gameplay.Execute(newEvent);
        }

    }

}