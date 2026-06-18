using UnityEngine;

namespace LycheeLabs.FruityInterface.Settings {

    public class VSyncSetting : BoolSetting {

        public VSyncSetting (string key, bool defaultValue) : base(key, defaultValue) { }

        public override void Apply () {
            QualitySettings.vSyncCount = Value ? 1 : 0;
        }

    }

}
