using LycheeLabs.FruityInterface.Settings;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class ToggleSettingKey : MonoBehaviour {

        public string SettingKey;
        private BoolSetting setting;

        public void Start () {
            if (SettingsRegistry.Active != null &&
                SettingsRegistry.Active.TryGet<BoolSetting>(SettingKey, out setting)) {
                var toggle = GetComponentInChildren<ToggleButton>();
                if (toggle != null) {
                    toggle.JumpTo(setting.Value);
                } else {
                    Debug.LogWarning("No ToggleButton found in children.");
                }
            } else {
                Debug.LogWarning("No bool setting found with key: " + SettingKey);
            }
        }

        public void Apply (bool value) {
            if (setting != null) {
                setting.Value = value;
            }
        }

    }

}
