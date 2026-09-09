using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class SliderSettingKey : MonoBehaviour {

        public string SettingKey;

        private Settings.IntSetting setting;

        void Start () {
            if (Settings.SettingsRegistry.Active == null) return;
            if (!Settings.SettingsRegistry.Active.TryGet<Settings.IntSetting>(SettingKey, out setting)) return;

            var slider = GetComponentInChildren<SliderNode>();
            if (slider == null) return;

            slider.Configure(setting.Min, setting.Max, setting.Value);
        }

        public void OnSlid (int value) {
            if (setting != null) {
                setting.Value = value;
            }
        }

    }

}
