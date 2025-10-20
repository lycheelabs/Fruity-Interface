using LycheeLabs.FruityInterface.Elements;
using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public class PromptSequenceLayer : SequenceLayer {

        public bool IsBlockingLayersBelow => IsPrompting;
        public bool IsPrompting => ActivePrompt != null || QueuedPrompt != null;
        public bool IsBlockedByLayersAbove { get; set; }

        public bool CurrentPromptIs<T>() where T : UIPrompt => ActivePrompt != null && ActivePrompt is T;

        /// <summary> Queued prompts trigger before queued events </summary>


        // ---------------------------------------------------

        public readonly EventSequencer Sequencer;
        public readonly UICanvas Canvas;

        private UIPrompt ActivePrompt;
        private UIPrompt.Instantiator QueuedPrompt;

        public PromptSequenceLayer (EventSequencer sequencer, UICanvas canvas) {
            Sequencer = sequencer;
            Canvas = canvas;
        }

        public void Prompt(UIPrompt.Instantiator newPrompt) {
            if (newPrompt == null) {
                Debug.LogWarning("Prompt is null");
                return;
            }

            QueuedPrompt = newPrompt;
            Sequencer.RefreshLayers();

            //FruityUI.DisableInput = ActivePrompt == null;
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

                    //FruityUI.LockUI(ActivePrompt);
                    //FruityUI.DisableInput = false;
                    return;
                }
            }

        }

        public void Clear() {
            if (ActivePrompt != null) {
                ActivePrompt.Close();
            }
            ActivePrompt = null;
            QueuedPrompt = null;
        }

    }

}