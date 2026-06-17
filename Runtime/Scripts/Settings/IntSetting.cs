using Newtonsoft.Json.Linq;

namespace LycheeLabs.FruityInterface.Settings {

    public class IntSetting : Setting {

        private int _value;
        public int Value {
            get => _value;
            set { _value = Clamp(value); Apply(); }
        }

        public int DefaultValue { get; }
        public int Min { get; }
        public int Max { get; }

        public IntSetting (string key, int defaultValue, int min = int.MinValue, int max = int.MaxValue) : base(key) {
            Min = min;
            Max = max;
            DefaultValue = Clamp(defaultValue);
            _value = DefaultValue;
        }

        public void Reset () {
            _value = DefaultValue;
            Apply();
        }

        private int Clamp (int value) {
            if (value < Min) return Min;
            if (value > Max) return Max;
            return value;
        }

        internal override JToken ToToken () => new JValue(_value);
        internal override void FromToken (JToken token) => _value = token.Value<int>();

    }

}
