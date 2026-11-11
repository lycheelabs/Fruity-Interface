using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    /// <summary>
    /// Charge state for a hold-to-confirm button. Fill rises linearly over the configured duration.
    /// Releasing early still fills to MinimumFill, then the charge fades out.
    /// </summary>
    public struct HoldCharge {

        public const float MinimumFill = 0.1f;
        public const float FadeDuration = 0.25f;

        private enum Phase { Idle, Charging, Settling, Fading }

        private float duration;
        private float fill;
        private float fadeTimer;
        private Phase phase;

        public float Fill => fill;

        public float FadeAlpha => phase == Phase.Fading ? 1f - Mathf.Clamp01(fadeTimer / FadeDuration) : 1f;

        public bool IsCharging => phase == Phase.Charging;

        public void Configure (float duration) {
            this.duration = Mathf.Max(duration, 0.01f);
        }

        public void Press () {
            fill = 0f;
            fadeTimer = 0f;
            phase = Phase.Charging;
        }

        public void Release () {
            if (phase != Phase.Charging) return;
            if (fill >= MinimumFill) {
                BeginFade();
            } else {
                phase = Phase.Settling;
            }
        }

        /// <summary>
        /// Advances the charge. Returns true on the frame the hold reaches full charge.
        /// </summary>
        public bool Tick (float deltaTime) {
            switch (phase) {
                case Phase.Charging:
                    fill += deltaTime / duration;
                    if (fill >= 1f) {
                        fill = 1f;
                        BeginFade();
                        return true;
                    }
                    break;

                case Phase.Settling:
                    fill += deltaTime / duration;
                    if (fill >= MinimumFill) {
                        fill = MinimumFill;
                        BeginFade();
                    }
                    break;

                case Phase.Fading:
                    fadeTimer += deltaTime;
                    if (fadeTimer >= FadeDuration) {
                        fill = 0f;
                        phase = Phase.Idle;
                    }
                    break;
            }
            return false;
        }

        private void BeginFade () {
            fadeTimer = 0f;
            phase = Phase.Fading;
        }

    }

}
