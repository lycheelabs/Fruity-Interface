using LycheeLabs.FruityInterface.Elements;
using System;
using System.Collections.Generic;

namespace LycheeLabs.FruityInterface {

    public abstract class EventSequencer {

        private List<SequenceLayer> sequenceLayers;
        private InterfaceNode currentRestrictedNode;

        protected EventSequencer() {
            sequenceLayers = new List<SequenceLayer>();
        }

        protected GameplaySequenceLayer AddGameplayLayer() {
            var layer = new GameplaySequenceLayer(this);
            sequenceLayers.Add(layer);
            return layer;
        }

        protected EventSequenceLayer AddEventLayer() {
            var layer = new EventSequenceLayer(this);
            sequenceLayers.Add(layer);
            return layer;
        }

        protected PromptSequenceLayer AddPromptLayer (CanvasNode canvas) {
            var layer = new PromptSequenceLayer(this, canvas); 
            sequenceLayers.Add(layer); 
            return layer;
        }

        protected TransitionSequenceLayer AddTransitionLayer() {
            var layer = new TransitionSequenceLayer(this);
            sequenceLayers.Add(layer);
            return layer;
        }

        public void Update () {
            // Update layers top to bottom
            RefreshLayers();
            for (int i = sequenceLayers.Count - 1; i >= 0; i--) {
                var layer = sequenceLayers[i];
                layer.Update();
            }
            RefreshControlsLock();
        }

        public void RefreshLayers () {
            // Update layers top to bottom
            bool isBlocked = false;
            for (int i = sequenceLayers.Count - 1; i >= 0; i--) {
                var layer = sequenceLayers[i];
                layer.IsBlockedByLayersAbove = isBlocked;
                isBlocked |= layer.IsBlockingLayersBelow;
            }

        }

        public void RefreshControlsLock() {
            InterfaceNode newRestrictedNode = null;
            for (int i = sequenceLayers.Count - 1; i >= 0; i--) {
                var promptLayer = sequenceLayers[i] as PromptSequenceLayer;
                if (promptLayer != null && promptLayer.IsRestrictingMouseInput) {
                    newRestrictedNode = promptLayer.ActiveNode;
                    break;
                }
            }
            if (newRestrictedNode != currentRestrictedNode) {
                currentRestrictedNode = newRestrictedNode;
                FruityUI.LockUI(newRestrictedNode);
            }
        }

        public void ClearLayersBelow(SequenceLayer targetLayer) {
            var clear = false;
            for (int i = sequenceLayers.Count - 1; i >= 0; i--) {
                var layer = sequenceLayers[i];
                if (layer == targetLayer) {
                    clear = true;
                    continue;
                }
                if (clear) {
                    layer.Clear();
                }
            }
        }

    }

}