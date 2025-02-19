using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class ParticleWrapper : TransformWrapper {

        protected readonly ParticleSystem particleSystem;
        protected readonly ParticleSystemRenderer renderer;
        private bool playing;

        public ParticleWrapper (GameObject gameObject) : base(gameObject) {
            particleSystem = gameObject.GetComponent<ParticleSystem>();
            renderer = gameObject.GetComponent<ParticleSystemRenderer>();
            playing = particleSystem.isPlaying;

            if (particleSystem == null) {
                throw new NullReferenceException("No ParticleSystem component found");
            }
            if (renderer == null) {
                throw new NullReferenceException("No ParticleSystemRenderer component found");
            }
        }

        public ParticleWrapper (Transform transform) : this(transform.gameObject) {}

        public void Play () {
            if (!playing) {
                playing = true;
                particleSystem.Play(true);
            }
        }

        public void Stop () {
            if (playing) {
                playing = false;
                particleSystem.Stop(true);
            }
        }

        public void Restart () {
            Stop();
            Play();
        }

        public void SetRunning (bool running) {
            if (running) {
                Play();
            } else {
                Stop();
            }
        }

        public void SetSize (Vector3 newSize) {
            var shape = particleSystem.shape;
            shape.scale = newSize;
        }

        public void SetParticleSize(float newSize) {
            var emission = particleSystem.main;
            emission.startSize = newSize;
        }

        public void SetRate (float rateOverTime, float rateOverDistance = 0) {
            var emission = particleSystem.emission;
            emission.rateOverTime = rateOverTime;
            emission.rateOverDistance = rateOverDistance;
        }

        public void SetRateMultiplier(float multiplier) {
            var emission = particleSystem.emission;
            emission.rateOverTimeMultiplier = multiplier;
            emission.rateOverDistanceMultiplier = multiplier;
        }

        public void SetSimulationSpeed(float newSpeed) {
            var main = particleSystem.main;
            main.simulationSpeed = newSpeed;
        }

        public void SetColor (Color newColor) {
            var main = particleSystem.main;
            main.startColor = newColor;
        }

        public void Burst (int amount) {
            particleSystem.Emit(amount);
        }

        public Material Material {
            get => renderer.sharedMaterial; 
            set => renderer.sharedMaterial = value;
        }

    }

}