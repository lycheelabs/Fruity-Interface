using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface.SaveLoad {

    public abstract class ListKeyConverter<T> {

        public List<string> ConvertToKeys (List<T> items) {
            var keys = new List<string>();
            if (items != null) {
                foreach (var item in items) {
                    keys.Add(ToKey(item));
                }
            }
            return keys;
        }

        public List<T> ConvertToItems (List<string> keys) {
            var items = new List<T>();
            if (keys != null) {
                foreach (var key in keys) {
                    var item = TryGetItem(key);
                    if (item != null) {
                        items.Add(item);
                    } else {
                        Debug.LogWarning("Load warning: " + ToErrorMessage(key));
                    }
                }
            }
            return items;
        }

        protected abstract string ToKey (T item);
        protected abstract T TryGetItem (string key);
        protected abstract string ToErrorMessage (string key);

    }

}
