using Newtonsoft.Json.Linq;

namespace LycheeLabs.InputTree.Settings {

    public abstract class Setting {

        public string Key { get; }

        protected Setting (string key) {
            Key = key;
        }

        public virtual void Apply () { }

        internal abstract JToken ToToken ();
        internal abstract void FromToken (JToken token);

    }

}
