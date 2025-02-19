using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(ToggleButton))]
    public abstract class ToggleButtonEffect : ButtonEffect {

        public bool IsToggledOn { get; private set; }

        private ToggleButton button = null;
        public ToggleButton Button {
            get => button ?? GetComponent<ToggleButton>();
        }

        [SerializeField]
        private float width = 300;
        public float Width {
            get => width;
            set { width = value; UpdateLayout(); }
        }

        [SerializeField]
        private float height = 40;
        public float Height {
            get => height;
            set { height = value; UpdateLayout(); }
        }

        [SerializeField]
        private string text = "Button Text";
        public string Text {
            get => text;
            set { text = value; UpdateLayout(); }
        }

        public void OnValidate() {
            Button.Initialise();
            UpdateLayout();
        }
        protected override void UpdateLayout() {
            Button.Configure(Text, Height, Width);
        }

        public void SetUpAs(bool value) {
            IsToggledOn = value;
        }

        public void Toggle () {
            ToggleTo(!IsToggledOn);
        }

        public void ToggleTo(bool value) {
            ApplyToggle(value);
            IsToggledOn = value;
        }

        protected abstract void ApplyToggle(bool value);

    }

}