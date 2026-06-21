using System.Collections.Generic;

namespace LycheeLabs.FruityInterface.Settings {

    public class SettingsRegistry {

        public static SettingsRegistry Active { get; set; }

        private readonly Dictionary<string, Setting> _params = new();
        private readonly HashSet<string> _excludedPrefixes = new();

        public T Register<T> (T param) where T : Setting {
            _params[param.Key] = param;
            return param;
        }

        public bool TryGet<T> (string key, out T param) where T : Setting {
            if (_params.TryGetValue(key, out var p) && p is T typed) {
                param = typed;
                return true;
            }
            param = null;
            return false;
        }

        public IEnumerable<Setting> All => _params.Values;

        public void ExcludeFromSave (string prefix) {
            _excludedPrefixes.Add(prefix);
        }

        public bool IsExcluded (string key) {
            foreach (var prefix in _excludedPrefixes) {
                if (key.StartsWith(prefix))
                    return true;
            }
            return false;
        }

        public void ApplyAll () {
            foreach (var param in _params.Values) {
                param.Apply();
            }
        }

        public void ResetAll () {
            foreach (var param in _params.Values) {
                if (param is BoolSetting b) b.Reset();
                else if (param is IntSetting i) i.Reset();
                else if (param is StringSetting s) s.Reset();
            }
        }

    }

}
