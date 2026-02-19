using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public abstract class TabbingSelectorEffect : MonoBehaviour {

        public int SelectedIndex { get; private set; }
        public abstract List<TabbingSelectorOption> ListAllOptions ();

        protected abstract TabbingSelectorOption InitialOption { get; }
        protected abstract bool CanWrap { get; }

        public void Initialise () {
            SelectedIndex = 0;

            var options = ListAllOptions();
            if (options.Count == 0) {
                return;
            }

            for (int i = 0; i < options.Count; i++) {
                if (options[i] == InitialOption) {
                    SelectedIndex = i;
                    break;
                }
            }
        }

        public TabbingSelectorOption SelectedOption () {
            var options = ListAllOptions();
            if (options.Count == 0) {
                return null;
            }
            SelectedIndex = Mathf.Clamp(SelectedIndex, 0, options.Count - 1);
            return options[SelectedIndex];
        }

        public bool CanTabLeft () {
            var options = ListAllOptions();
            return options.Count > 0 && (SelectedIndex > 0 || CanWrap);
        }

        public bool CanTabRight () {
            var options = ListAllOptions();
            return options.Count > 0 && (SelectedIndex < options.Count - 1 || CanWrap);
        }

        public TabbingSelectorOption TabLeft () {
            var options = ListAllOptions();
            if (options.Count == 0) {
                SelectedIndex = 0;
                return null;
            }

            if (SelectedIndex > 0) {
                SelectedIndex--;
            } else if (CanWrap) {
                SelectedIndex = options.Count - 1;
            }

            return options[SelectedIndex];
        }

        public TabbingSelectorOption TabRight () {
            var options = ListAllOptions();
            if (options.Count == 0) {
                SelectedIndex = 0;
                return null;
            }

            if (SelectedIndex < options.Count - 1) {
                SelectedIndex++;
            } else if (CanWrap) {
                SelectedIndex = 0;
            }

            return options[SelectedIndex];
        }

    }

    public interface TabbingSelectorOption {
        string Name { get; }
        Color Color { get; }
    }

}