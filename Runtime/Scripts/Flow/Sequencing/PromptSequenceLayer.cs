using LycheeLabs.FruityInterface.Elements;
using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public class PromptSequenceLayer : SequenceLayer {

        public bool IsBlockingLayersBelow => IsPrompting;
        public bool IsPrompting => ActivePrompt != null || QueuedPrompt != null;
        public bool IsBlockedByLayersAbove { get; set; }

        public bool IsRestrictingMouseInput => ActivePrompt != null && ActivePrompt.RestrictMouseInput;
        public InterfaceNode ActiveNode => ActivePrompt;

        // ---------------------------------------------------

        public readonly EventSequencer Sequencer;
        public readonly UICanvas Canvas;

        private UIPrompt ActivePrompt;
        private UIPrompt.PromptInstantiator QueuedPrompt;

        public PromptSequenceLayer (EventSequencer sequencer, UICanvas canvas) {
            Sequencer = sequencer;
            Canvas = canvas;
        }

        public void Prompt(UIPrompt.PromptInstantiator newPrompt) {
            if (newPrompt == null) {
                Debug.LogWarning("Prompt is null");
                return;
            }

            QueuedPrompt = newPrompt;
            Sequencer.RefreshLayers();
        }

        public void Update() {

            // Update active prompt
            if (ActivePrompt != null) {
                ActivePrompt.UpdateFlow(IsBlockedByLayersAbove);

                if (QueuedPrompt != null) {
                    ActivePrompt.Close();
                }
                if (ActivePrompt.HasCompleted) {
                    ActivePrompt = null;
                }
            }

            // Activate next prompt
            if (!IsBlockedByLayersAbove) {
                if (ActivePrompt == null && QueuedPrompt != null) {
                    ActivePrompt = QueuedPrompt.Instantiate(this);
                    ActivePrompt.StartOpening();
                    ActivePrompt.UpdateFlow(IsBlockedByLayersAbove);
                    QueuedPrompt = null;
                    return;
                }
            }

        }

        public void Close () {
            if (ActivePrompt != null) {
                ActivePrompt.Close();
            }
            QueuedPrompt = null;
        }

        public void Clear() {
            if (ActivePrompt != null) {
                ActivePrompt.CloseImmediately();
                ActivePrompt = null;
            }
            QueuedPrompt = null;
        }

    }

}