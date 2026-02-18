using LycheeLabs.FruityInterface.Elements;
using System;
using System.Collections.Generic;
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
        public readonly CanvasNode Canvas;

        private PromptNode ActivePrompt;
        private PromptNode.PromptInstantiator QueuedPrompt;
        private PromptNode ReopenPrompt;
        private List<PromptNode> NavigationStack;

        public PromptSequenceLayer (EventSequencer sequencer, CanvasNode canvas) {
            Sequencer = sequencer;
            Canvas = canvas;
            NavigationStack = new List<PromptNode>();
        }

        public void Prompt(PromptNode.PromptInstantiator newPrompt) {
            if (newPrompt == null) {
                Debug.LogWarning("Prompt is null");
                return;
            }

            QueuedPrompt = newPrompt;
            ReopenPrompt = null;
            Sequencer.RefreshLayers();
        }

        public void GoBack () {
            if (ActivePrompt != null) {
                ActivePrompt.Close();
                if (NavigationStack.Count > 0) {
                    ReopenPrompt = NavigationStack[NavigationStack.Count - 1];
                    NavigationStack.RemoveAt(NavigationStack.Count - 1);
                }
            }
        }

        public void Update() {

            // Update active prompt
            if (ActivePrompt != null) {
                ActivePrompt.UpdateFlow(IsBlockedByLayersAbove);

                if (QueuedPrompt != null && !ActivePrompt.IsClosing) {
                    ActivePrompt.Close();
                }

                if (ActivePrompt.HasPaused) {
                    // Pause this prompt
                    NavigationStack.Add(ActivePrompt);
                    ActivePrompt = null;
                }
                else if (ActivePrompt.HasCompleted) {
                    ActivePrompt = null;
                    if (ReopenPrompt != null) {
                        // Reopen paused prompt
                        ActivePrompt = ReopenPrompt;
                        ActivePrompt.Reopen();
                    } else { 
                        NavigationStack.Clear();
                    }
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
            ReopenPrompt = null;
            NavigationStack.Clear();
        }

        public void Clear() {
            if (ActivePrompt != null) {
                ActivePrompt.CloseImmediately();
                ActivePrompt = null;
            }
            QueuedPrompt = null;
            ReopenPrompt = null;
            NavigationStack.Clear();
        }

    }

}