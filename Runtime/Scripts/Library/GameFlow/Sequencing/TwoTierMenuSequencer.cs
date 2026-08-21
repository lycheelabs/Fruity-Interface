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

        public TwoTierMenuSequencer() {
            Gameplay = AddGameplayLayer();
            BlockingGameplay = AddEventLayer();
            GamePrompts = AddPromptLayer(null);
            OverlayPrompts = AddPromptLayer(null);
            Transitions = AddTransitionLayer();
        }

        public void InjectPromptCanvases(CanvasNode gamePromptCanvas, CanvasNode overlayPromptCanvas) {
            GamePrompts.InjectCanvas(gamePromptCanvas);
            OverlayPrompts.InjectCanvas(overlayPromptCanvas);
        }

        public bool PauseAllGameplay => OverlayPrompts.IsPrompting;

        public void Transition(TransitionEvent newEvent) {
            Transitions.Transition(newEvent);
        }

        public void Prompt(PromptNode.PromptInstantiator newPrompt, TwoTierMenuLayer layer) {
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