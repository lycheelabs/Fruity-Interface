using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class GameplaySequenceLayer : SequenceLayer {

        public bool IsBlockingLayersBelow => false;
        public bool IsBlockedByLayersAbove { get; set; }

        private readonly EventSequencer Sequencer;
        private List<GameplayEvent> ActiveEvents;

        public GameplaySequenceLayer(EventSequencer sequencer) {
            Sequencer = sequencer;
            ActiveEvents = new List<GameplayEvent>();
        }

        public void Execute(GameplayEvent newEvent) {
            if (newEvent == null) {
                Debug.LogWarning("Event is null");
                return;
            }
            newEvent.Start(this);
            ActiveEvents.Add(newEvent);
        }

        public void Update() {
            for (int i = ActiveEvents.Count - 1; i >= 0; i--) {
                ActiveEvents[i].Update();
                if (ActiveEvents[i].IsComplete) {
                    ActiveEvents.RemoveAt(i);
                }
            }

        }

        public void Clear() {
            for (int i = ActiveEvents.Count - 1; i >= 0; i--) {
                ActiveEvents[i].Complete();
            }
            ActiveEvents.Clear();
        }

    }

}