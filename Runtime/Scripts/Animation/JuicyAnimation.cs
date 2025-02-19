using UnityEngine;

namespace LycheeLabs.FruityInterface.Animation {

    // Juicy Animators can have 1 base animation, which is replaceable.
    // They can have any number of layered animations, which overlay.
    public enum JuicyAnimationMode { BASE, LAYER }

    public interface JuicyAnimation {
        public JuicyAnimationMode Mode { get; }
        void Update (ref TransformData transform, float timeScaling);
        bool ReadyToFinish { get; }
    }

    public abstract class SimpleJuicyAnimation : JuicyAnimation {
        public virtual JuicyAnimationMode Mode => JuicyAnimationMode.LAYER;
        public abstract void Update (ref TransformData transform, float timeScaling);
        public abstract bool ReadyToFinish { get; }
    }

}