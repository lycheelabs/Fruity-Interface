using LycheeLabs.FruityInterface.Elements;
using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class EventSequencer {

        public bool IsIdle => !IsAnimating && !IsPrompting;
        public bool IsAnimating => ActiveEvent != null || WaitingEvents.Count > 0;
        public bool IsPrompting => ActivePrompt != null || QueuedPrompts.Count > 0;
        public bool IsPromptingNow => ActivePrompt != null;

        /// <summary> Played events trigger first </summary>
        public void Play(BlockingEvent newEvent) {
            if (newEvent == null) {
                Debug.LogWarning("Event is null");
                return;
            }
            PlayingEvents.Add(newEvent);
            if (IsIdle) {
                ActivateNextEvent(PlayingEvents);
            }
        }

        /// <summary> Queued events trigger last (after prompts) </summary>
        public void Queue(BlockingEvent newEvent) {
            if (newEvent == null) {
                Debug.LogWarning("Event is null");
                return;
            }
            WaitingEvents.Add(newEvent);
        }

        /// <summary> Queued prompts trigger before queued events </summary>
        public void Queue(UIPrompt newPrompt) {
            if (newPrompt == null) {
                Debug.LogWarning("Event is null");
                return;
            }
            QueuedPrompts.Add(newPrompt);
            InterfaceConfig.DisableInput = ActivePrompt == null;
        }

        // ---------------------------------------------------

        private List<BlockingEvent> PlayingEvents;
        private List<BlockingEvent> WaitingEvents;
        private BlockingEvent ActiveEvent;

        private List<UIPrompt> QueuedPrompts;
        private UIPrompt ActivePrompt;
        private bool PromptIsActive;

        public EventSequencer() {
            PlayingEvents = new List<BlockingEvent>();
            WaitingEvents = new List<BlockingEvent>();
            QueuedPrompts = new List<UIPrompt>();
        }

        public void Update() {

            // Update active prompt
            if (ActivePrompt != null && ActivePrompt.ReadyToContinue) {
                ActivePrompt = null;
            }
            if (PromptIsActive && ActivePrompt == null) {
                PromptIsActive = false;
                InterfaceConfig.UnlockUI();
            }

            // Update active event
            if (ActiveEvent != null) {
                ActiveEvent.Update();
                if (ActiveEvent.IsComplete) {
                    ActiveEvent.Finish();
                    ActiveEvent = null;
                }
            }

            if (ActiveEvent != null || ActivePrompt != null) {
                // Wait for complection...
                return;
            }

            // Activate next playing event
            if (PlayingEvents.Count > 0) {
                ActivateNextEvent(PlayingEvents);
                return;
            }

            // Activate next prompt
            if (QueuedPrompts.Count > 0) {
                ActivateNextPrompt();
                return;
            }

            // Activate next waiting event
            if (WaitingEvents.Count > 0) {
                ActivateNextEvent(WaitingEvents);
                return;
            }
        }

        private void ActivateNextEvent(List<BlockingEvent> queue) {
            ActiveEvent = queue[0];
            queue.RemoveAt(0);
            ActiveEvent.Activate();
        }

        private void ActivateNextPrompt() {
            ActivePrompt = QueuedPrompts[0];
            QueuedPrompts.RemoveAt(0);
            ActivePrompt.Activate();
            InterfaceConfig.LockUI(ActivePrompt);
            InterfaceConfig.DisableInput = false;
            PromptIsActive = true;
        }

    }

}