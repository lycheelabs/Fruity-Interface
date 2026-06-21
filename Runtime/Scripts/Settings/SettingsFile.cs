using LycheeLabs.FruityInterface.SaveLoad;
using Newtonsoft.Json.Linq;

namespace LycheeLabs.FruityInterface.Settings {

    public class SettingsFile : SaveFile {

        private const string RelativePath = "settings.txt";
        private const bool EncryptData = false;

        public static SaveFilePath<SettingsFile> FilePath => new(RelativePath, EncryptData);

        public JObject Data;

        public SettingsFile () {
            Data = new JObject();
        }

        public void CaptureFrom (SettingsRegistry registry) {
            Data = new JObject();
            foreach (var param in registry.All) {
                if (registry.IsExcluded(param.Key))
                    continue;
                SetNested(Data, param.Key, param.ToToken());
            }
        }

        public void ApplyTo (SettingsRegistry registry) {
            foreach (var param in registry.All) {
                if (registry.IsExcluded(param.Key))
                    continue;
                var token = GetNested(Data, param.Key);
                if (token != null) {
                    param.FromToken(token);
                }
            }
            registry.ApplyAll();
        }

        public bool Save () {
            return SaveManager.TrySave(this, RelativePath, EncryptData);
        }

        public static LoadData<SettingsFile> Load () {
            return FilePath.LoadIfExists();
        }

        public override bool Validate () => Data != null;

        private static void SetNested (JObject root, string dottedKey, JToken value) {
            var parts = dottedKey.Split('.');
            var current = root;
            for (int i = 0; i < parts.Length - 1; i++) {
                if (current[parts[i]] is not JObject child) {
                    child = new JObject();
                    current[parts[i]] = child;
                }
                current = child;
            }
            current[parts[^1]] = value;
        }

        private static JToken GetNested (JObject root, string dottedKey) {
            var parts = dottedKey.Split('.');
            JToken current = root;
            foreach (var part in parts) {
                if (current is JObject obj && obj.TryGetValue(part, out var token))
                    current = token;
                else
                    return null;
            }
            return current;
        }

    }

}
