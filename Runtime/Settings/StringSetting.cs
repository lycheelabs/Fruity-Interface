using Newtonsoft.Json.Linq;

namespace LycheeLabs.FruityInterface.Settings {

    public class StringSetting : Setting {

        private string _value;
        public string Value {
            get => _value;
            set { _value = value; Apply(); }
        }

        public string DefaultValue { get; }

        public StringSetting (string key, string defaultValue) : base(key) {
            DefaultValue = defaultValue;
            _value = defaultValue;
        }

        public void Reset () {
            _value = DefaultValue;
            Apply();
        }

        internal override JToken ToToken () => new JValue(_value);
        internal override void FromToken (JToken token) => _value = token.Value<string>();

    }

}
