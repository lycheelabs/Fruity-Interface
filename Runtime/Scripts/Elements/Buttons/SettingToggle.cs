using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(ToggleButton))]
    public class SettingToggle : ToggleEffect {

        public string SettingKey;

        public void Start () {
            var registry = Settings.SettingsRegistry.Active;
            if (registry != null && registry.TryGet<Settings.BoolSetting>(SettingKey, out var param)) {
                SetUpAs(param.Value);
            }
        }

        protected override void ApplyToggle (bool value) {
            var registry = Settings.SettingsRegistry.Active;
            if (registry != null && registry.TryGet<Settings.BoolSetting>(SettingKey, out var param)) {
                param.Value = value;
            }
        }

        public override void OnHover () { }

    }

}
