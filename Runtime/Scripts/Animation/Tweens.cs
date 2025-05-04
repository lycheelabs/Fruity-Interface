using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public static class Tweens {

        public static Tween EaseInQuadTween = new Tween(EaseInQuad);
        public static Tween EaseOutQuadTween = new Tween(EaseOutQuad);
        public static Tween EaseInOutQuadTween = new Tween(EaseInOutQuad);

        public static Tween EaseInCubeTween = new Tween(EaseInCube);
        public static Tween EaseOutCubeTween = new Tween(EaseOutCube);
        public static Tween EaseInOutCubeTween = new Tween(EaseInOutCube);

        public static Tween EaseInQuartTween = new Tween(EaseInQuart);
        public static Tween EaseOutQuartTween = new Tween(EaseOutQuart);
        public static Tween EaseInOutQuartTween = new Tween(EaseInOutQuart);

        public static Tween SpringTween = new Tween(Spring);
        public static Tween BounceTween = new Tween(Bounce);
        public static Tween ParabolaTween = new Tween(Parabola);

        // ----------------------------------------------------

        public static float EaseIn (float completion, float power) {
            completion = Mathf.Clamp01(completion);
            return Mathf.Pow(completion, power);
        }

        public static float EaseOut (float completion, float power) {
            completion = Mathf.Clamp01(completion);
            completion = 1 - completion;
            return 1 - Mathf.Pow(completion, power);
        }

        public static float EaseInOut (float completion, float power) {
            completion = Mathf.Clamp01(completion);
            float doubleCompletion = (completion * 2);
            if (doubleCompletion < 1) return EaseIn(doubleCompletion, power) * 0.5f;
            else return EaseOut(doubleCompletion - 1, power) * 0.5f + 0.5f;
        }
        // ----------------------------------------------------

        public static float EaseInQuad (float completion) {
            completion = Mathf.Clamp01(completion);
            return completion * completion;
        }

        public static float EaseOutQuad (float completion) {
            completion = Mathf.Clamp01(completion);
            completion = 1 - completion;
            return 1 - (completion * completion);
        }

        public static float EaseInOutQuad (float completion) {
            completion = Mathf.Clamp01(completion);
            float doubleCompletion = (completion * 2);
            if (doubleCompletion < 1) return EaseInQuad(doubleCompletion) * 0.5f;
            else return EaseOutQuad(doubleCompletion - 1) * 0.5f + 0.5f;
        }

        // ----------------------------------------------------

        public static float EaseInCube (float completion) {
            completion = Mathf.Clamp01(completion);
            return completion * completion * completion;
        }

        public static float EaseOutCube (float completion) {
            completion = Mathf.Clamp01(completion);
            completion = 1 - completion;
            return 1 - (completion * completion * completion);
        }

        public static float EaseInOutCube (float completion) {
            completion = Mathf.Clamp01(completion);
            float doubleCompletion = (completion * 2);
            if (doubleCompletion < 1) return EaseInCube(doubleCompletion) * 0.5f;
            else return EaseOutCube(doubleCompletion - 1) * 0.5f + 0.5f;
        }

        // ----------------------------------------------------

        public static float EaseInQuart (float completion) {
            completion = Mathf.Clamp01(completion);
            return completion * completion * completion * completion;
        }

        public static float EaseOutQuart (float completion) {
            completion = Mathf.Clamp01(completion);
            completion = 1 - completion;
            return 1 - (completion * completion * completion * completion);
        }

        public static float EaseInOutQuart (float completion) {
            completion = Mathf.Clamp01(completion);
            float doubleCompletion = (completion * 2);
            if (doubleCompletion < 1) return EaseInQuart(doubleCompletion) * 0.5f;
            else return EaseOutQuart(doubleCompletion - 1) * 0.5f + 0.5f;
        }

        // ----------------------------------------------------

        public static float Spring (float completion) {
            completion = Mathf.Clamp01(completion);
            float easeIn = completion * completion;
            return easeIn + Mathf.Sin(Mathf.PI * completion) * 0.7f;
        }

        public static float Bounce (float completion) {
            completion = Mathf.Clamp01(completion);
            float linear = 0;
            if (completion < 0.5f) {
                linear = (completion / 0.5f);
            } else {
                linear = (completion - 0.5f) / 0.5f;
                linear = 1 - linear * 0.25f;
            }
            return 1 - linear * linear;
        }

        public static float Parabola (float completion) {
            completion = Mathf.Clamp01(completion);
            return 4 * (completion - completion * completion);
        }

        public static float Delay (float completion, float delay) {
            completion = Mathf.Clamp01(completion);
            return Mathf.Clamp(completion * (1 + delay) - delay, 0, 1);
        }

        // ----------------------------------------------------

        public static float SmoothLerp(this float a, float b, float halfLife) {
            return b + (a - b) * Exp2(-Time.deltaTime / halfLife);
        }

        public static float SmoothLerp(this float a, bool b, float halfLife) {
            var B = b ? 1 : 0;
            return B + (a - B) * Exp2(-Time.deltaTime / halfLife);
        }

        public static Vector2 SmoothLerp(this Vector2 a, Vector2 b, float halfLife) {
            return b + (a - b) * Exp2(-Time.deltaTime / halfLife);
        }

        public static Vector3 SmoothLerp(this Vector3 a, Vector3 b, float halfLife) {
            return b + (a - b) * Exp2(-Time.deltaTime / halfLife);
        }

        public static float SmoothLerp(this float a, float b, float halfLife, float deltaTime) {
            return b + (a - b) * Exp2(-deltaTime / halfLife);
        }

        public static float SmoothLerp(this float a, bool b, float halfLife, float deltaTime) {
            var B = b ? 1 : 0;
            return B + (a - B) * Exp2(-deltaTime / halfLife);
        }

        public static Vector2 SmoothLerp(this Vector2 a, Vector2 b, float halfLife, float deltaTime) {
            return b + (a - b) * Exp2(-deltaTime / halfLife);
        }

        public static Vector3 SmoothLerp(this Vector3 a, Vector3 b, float halfLife, float deltaTime) {
            return b + (a - b) * Exp2(-deltaTime / halfLife);
        }

        private static float Exp2(float x) {
            return Mathf.Exp(x * Mathf.Log(2));
        }

        public static float MoveTowards(this float value, float target, float speedScaling = 1f) {
            if (value < target) {
                return Mathf.Min(value + Time.deltaTime * speedScaling, target);
            }
            else {
                return Mathf.Max(value - Time.deltaTime * speedScaling, target);
            }
        }

        public static float MoveTowards(this float value, bool target, float speedScaling = 1f) {
            return MoveTowards(value, target ? 1 : 0, speedScaling);
        }

        public static float MoveTowardsUnscaled(this float value, float target, float speedScaling = 1f) {
            if (value < target) {
                return Mathf.Min(value + Time.unscaledDeltaTime * speedScaling, target);
            }
            else {
                return Mathf.Max(value - Time.unscaledDeltaTime * speedScaling, target);
            }
        }
        public static float MoveTowardsUnscaled(this float value, bool target, float speedScaling = 1f) {
            return MoveTowardsUnscaled(value, target ? 1 : 0, speedScaling);
        }

    }

}