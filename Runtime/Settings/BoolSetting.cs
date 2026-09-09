using Newtonsoft.Json.Linq;

namespace LycheeLabs.FruityInterface.Settings {

    public class BoolSetting : Setting {

        private bool _value;
        public bool Value {
            get => _value;
            set { _value = value; Apply(); }
        }

        public bool DefaultValue { get; }

        public BoolSetting (string key, bool defaultValue) : base(key) {
            DefaultValue = defaultValue;
            _value = defaultValue;
        }

        public void Reset () {
            _value = DefaultValue;
            Apply();
        }

        internal override JToken ToToken () => new JValue(_value);
        internal override void FromToken (JToken token) => _value = token.Value<bool>();

    }

}
