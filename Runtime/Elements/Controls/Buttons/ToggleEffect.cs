using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(ToggleSwitch))]
    public abstract class ToggleEffect : MonoBehaviour {

        public bool IsToggledOn { get; private set; }

        private ToggleSwitch button = null;
        public ToggleSwitch Button {
            get => button ?? GetComponent<ToggleSwitch>();
        }

        public void ToggleTo(bool value) {
            ApplyToggle(value);
            IsToggledOn = value;
        }

        // -------------------------------------------------

        public virtual void OnHover () {}
        public virtual void OnClick () {}
        protected abstract void ApplyToggle(bool value);

    }

}