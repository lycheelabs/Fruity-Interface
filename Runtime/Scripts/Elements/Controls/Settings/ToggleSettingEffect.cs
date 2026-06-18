using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class ToggleSettingEffect : ToggleEffect {

        protected override void ApplyToggle (bool value) {
            var key = GetComponentInParent<ToggleSettingKey>();
            if (key == null) {
                Debug.LogWarning("No ToggleSettingKey found in parent.");
                return;
            }
            key.Apply(value);
        }

    }

}
