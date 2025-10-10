using LycheeLabs.FruityInterface.Elements;

namespace LycheeLabs.FruityInterface {

    public enum TwoTierMenuLayer {
        GAMEPLAY, OVERLAY
    }

    /// <summary>
    /// This sequencer allows layering of a main menu prompt on top of in-game menu prompts.
    /// </summary>
    public class TwoTierMenuSequencer : EventSequencer {

        public bool IsIdle => !IsAnimating && !IsPrompting;
        public bool IsAnimating => GameEvents.IsAnimating;
        public bool IsPrompting => GamePrompts.IsPrompting || OverlayPrompts.IsPrompting;

        private PromptSequenceLayer OverlayPrompts;
        private PromptSequenceLayer GamePrompts;
        private EventSequenceLayer GameEvents;

        public TwoTierMenuSequencer(UICanvas gamePromptCanvas, UICanvas overlayPromptCanvas) {
            // Instantiate layers from bottom to top
            GameEvents = AddEventLayer();
            GamePrompts = AddPromptLayer(gamePromptCanvas);
            OverlayPrompts = AddPromptLayer(overlayPromptCanvas);
        }

        public void Queue (BlockingEvent newEvent) {
            GameEvents.Queue(newEvent);
        }

        public void Prompt(UIPrompt.Instantiator newPrompt, TwoTierMenuLayer layer) {
            if (layer == TwoTierMenuLayer.OVERLAY) {
                OverlayPrompts.Prompt(newPrompt);
            } else {
                GamePrompts.Prompt(newPrompt);
            }
        }

    }

}