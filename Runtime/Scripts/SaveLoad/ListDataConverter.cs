using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface.SaveLoad {

    public abstract class ListDataConverter<T, D> {

        public List<D> ConvertToData (List<T> items) {
            var keys = new List<D>();
            if (items != null) {
                foreach (var item in items) {
                    keys.Add(ToData(item));
                }
            }
            return keys;
        }

        public List<T> ConvertToItems (List<D> data) {
            var items = new List<T>();
            if (data != null) {
                foreach (var d in data) {
                    var item = TryGetItem(d);
                    if (item != null) {
                        items.Add(item);
                    } else {
                        Debug.LogWarning("Load warning: " + ToErrorMessage(d));
                    }
                }
            }
            return items;
        }

        protected abstract D ToData (T item);
        protected abstract T TryGetItem (D data);
        protected abstract string ToErrorMessage (D data);

    }

}
