using System;

namespace LycheeLabs.InputTree.Animation {

    public class Tween {

        private readonly Func<float, float> Function;

        public Tween (Func<float, float> function) {
            Function = function;
        }

        public float Apply (float value) {
            return Function(value);
        }

        public float ApplyInverted (float value) {
            return 1 - Function(1 - value);
        }

    }

}
