using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {
    public class CameraShaker {

        private float shake;
        private float shakeTime;

        public Vector3 Offset { get; private set; }

        public void Shake(float strength) {
            shake = Math.Max(shake, strength);
        }

        public void Update() {

            //if (Input.GetKeyUp(KeyCode.Backspace)) {
            //    Shake(2);
            //}

            // Shake
            this.shake = this.shake.MoveTowards(0, 7);
            shakeTime += Time.deltaTime * 40;
            if (this.shake == 0) shakeTime = 0;
            var shake = Mathf.Sin(shakeTime) * this.shake;
            var shakeVector = new Vector3(0, shake, 0);
            Offset = shakeVector * 0.66f;
        }

    }

}