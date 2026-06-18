using UnityEngine;
using UnityEngine.Audio;

namespace LycheeLabs.FruityInterface.Settings {

    public class VolumeSetting : IntSetting {

        private readonly AudioMixer mixer;
        private readonly string parameter;

        public VolumeSetting (string key, AudioMixer mixer, string parameter)
            : base(key, 100, 0, 100) {
            this.mixer = mixer;
            this.parameter = parameter;
        }

        public override void Apply () {
            var normalized = Mathf.Clamp(ValueAsPercent, 0.0001f, 1f);
            mixer.SetFloat(parameter, Mathf.Log10(normalized) * 20f);
        }

    }

}
