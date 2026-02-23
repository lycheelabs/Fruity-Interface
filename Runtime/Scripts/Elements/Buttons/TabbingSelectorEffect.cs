using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public abstract class TabbingSelectorEffect : MonoBehaviour {

        public int SelectedIndex { get; private set; }
        public abstract List<TabbingSelectorOption> ListAllOptions ();

        public abstract bool MainButtonIsSelectable { get; }
        protected abstract bool OptionsCanWrap { get; }

        private TabbingSelectorOption currentOption;

        public void Initialise () {         
            var options = ListAllOptions();
            if (options.Count == 0) {
                return;
            }
            if (currentOption == null) {
                SelectedIndex = 0;
                currentOption = options[0];
            }
        }


        public void OnSelectionSet (TabbingSelectorOption option) {
            if (option != currentOption) {
                var options = ListAllOptions();
                for (int i = 0; i < options.Count; i++) {
                    if (options[i] == option) {
                        currentOption = option;
                        SelectedIndex = i;
                        OnSelectionChanged(option);
                        return;
                    }
                }
            }
        }

        public abstract void MouseOverComponent (TabbingSelectorComponent type);
        public abstract void ActivateMainButton ();
        protected abstract void OnSelectionChanged (TabbingSelectorOption newOption);

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
            return options.Count > 0 && (SelectedIndex > 0 || OptionsCanWrap);
        }

        public bool CanTabRight () {
            var options = ListAllOptions();
            return options.Count > 0 && (SelectedIndex < options.Count - 1 || OptionsCanWrap);
        }

        public TabbingSelectorOption TabLeft () {
            var options = ListAllOptions();
            if (options.Count == 0) {
                SelectedIndex = 0;
                return null;
            }

            if (SelectedIndex > 0) {
                SelectedIndex--;
            } else if (OptionsCanWrap) {
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
            } else if (OptionsCanWrap) {
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