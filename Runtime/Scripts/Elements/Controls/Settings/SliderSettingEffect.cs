using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(SliderNode))]
    public class SliderSettingEffect : SliderEffect {

        public override void OnValueChanged (int value) {
            GetComponentInParent<SliderSettingKey>()?.OnSlid(value);
        }

    }

}
