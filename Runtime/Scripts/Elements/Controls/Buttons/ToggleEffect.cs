using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(ToggleButton))]
    public abstract class ToggleEffect : MonoBehaviour {

        public bool IsToggledOn { get; private set; }

        private ToggleButton button = null;
        public ToggleButton Button {
            get => button ?? GetComponent<ToggleButton>();
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