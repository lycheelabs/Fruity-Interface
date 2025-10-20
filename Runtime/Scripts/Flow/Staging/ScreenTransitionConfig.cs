using System.Collections.Generic;

namespace LycheeLabs.FruityInterface.Flow {
    public class ScreenTransitionConfig {

        public readonly Dictionary<string, object> Config = new();

        public void SetConfig(string key, object value) {
            Config[key] = value;
        }

        public bool TryGetConfig(string key, out object value) {
            if (Config.ContainsKey(key)) {
                value = Config[key];
                return true;
            }
            value = null;
            return false;
        }

    }

}