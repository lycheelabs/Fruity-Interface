using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public class CameraShaker {

        private float fastShake;
        private float fastShakeTime;

        public Vector3 Offset { get; private set; }

        public void Shake(float strength) {
            fastShake = Math.Max(fastShake, strength);
        }

        public void Update() {

            // Shake
            fastShake = fastShake.MoveTowards(0, 7);
            fastShakeTime += Time.deltaTime * 75;
            if (fastShake == 0) fastShakeTime = 0;
            var shake = Mathf.Sin(fastShakeTime) * fastShake;
            var shakeVector = new Vector3(0, shake, 0);
            Offset = shakeVector * 0.66f;
        }

    }

}