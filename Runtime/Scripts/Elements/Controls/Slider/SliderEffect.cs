using UnityEngine;

namespace LycheeLabs.FruityInterface {

    [RequireComponent(typeof(SliderNode))]
    public abstract class SliderEffect : MonoBehaviour {

        public abstract void OnValueChanged (int value);

    }

}
