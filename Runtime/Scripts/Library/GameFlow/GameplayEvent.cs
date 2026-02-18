using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public abstract class GameplayEvent {

        public bool IsActivated { get; private set; }
        public bool IsComplete { get; private set; }
        public GameplayEvent Next { get; set; }

        private GameplaySequenceLayer Layer;

        public void Start(GameplaySequenceLayer layer) {
            Layer = layer;
            Activate();
            IsActivated = true;
        }

        protected abstract void Activate();
        public abstract void Update();

        public void Complete () {
            if (!IsComplete) {
                IsComplete = true;
                Deactivate();

                if (Next != null) {
                    Layer.Execute(Next);
                }
            }
        }

        protected abstract void Deactivate();

    }

}