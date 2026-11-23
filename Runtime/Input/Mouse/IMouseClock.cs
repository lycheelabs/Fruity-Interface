using UnityEngine;

namespace LycheeLabs.InputTree {

    public interface IMouseClock {
        float UnscaledTime { get; }
        int FrameCount { get; }
    }

    internal sealed class UnityMouseClock : IMouseClock {
        public float UnscaledTime => Time.unscaledTime;
        public int FrameCount => Time.frameCount;
    }

}
