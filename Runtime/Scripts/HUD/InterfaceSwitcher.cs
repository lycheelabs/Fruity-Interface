using System;
using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class InterfaceSwitcher<T> where T : Enum {

		private static InterfaceElement[] None = new InterfaceElement[0];

		// -----------------------------------------------

		private readonly List<Set> sets;
		private Dictionary<InterfaceElement, float> enterValues;

		private InterfaceElement[] previousElements;
		private InterfaceElement[] currentElements;
		private T activeMode;

		private bool transitioning;
		private bool exiting;
		private float transitionCurve;

		public T CurrentMode => activeMode;

        public InterfaceSwitcher() {
			sets = new List<Set>();
			enterValues = new Dictionary<InterfaceElement, float>();

			previousElements = None;
			currentElements = None;
		}

		public void AddMode (T mode, params InterfaceElement[] elements) {
			var modeName = mode.ToString();
			var set = new Set(modeName, elements);
			sets.Add(set);

			foreach (var element in elements) {
				if (!Contains(currentElements, element)) {
					element.SetEntered(0);
				}
			}
		}

        public void SetMode (T mode) {
			if (activeMode.Equals(mode)) return; 

			var modeName = mode.ToString();
			for (int i = 0; i < sets.Count; i++) {
				if (sets[i].Name == modeName) {
                    activeMode = mode;
                    StartTransition(sets[i].Elements);
					return;
				}
            }
            activeMode = default;
            StartTransition(None);
		}

		private void StartTransition (InterfaceElement[] newElements) {
			// Incase previous transition didn't finish
			foreach (var element in previousElements) {
				if (!Contains(newElements, element)) {
					element.SetEntered(0);
				}
			}

			previousElements = currentElements;
			currentElements = newElements;
			transitioning = true;
			exiting = false;
			transitionCurve = 0;

			foreach (var element in previousElements) {
				if (!Contains(currentElements, element)) {
					exiting = true;
					transitionCurve = 1;
					break;
				}
            }
		}

		public void Update () {

			if (transitioning) {

				// Transition old element out
				if (exiting) {
					transitionCurve = Mathf.Max(transitionCurve - 6f * Time.deltaTime, 0f);
					var tween = Tweens.	EaseOutQuad(transitionCurve);

					foreach (InterfaceElement element in previousElements) {
						if (Contains(currentElements, element)) {
							// This element is not exiting
							enterValues[element] = 1;
						} else {
							enterValues[element] = Mathf.Min(tween, CachedEntryTween(element));
						}
						element.SetEntered(enterValues[element]);
					}
					if (transitionCurve <= 0) {
						exiting = false;
					}
				}

				// Transition new element in
				if (!exiting) {
					transitionCurve = Mathf.Min(transitionCurve + 6f * Time.deltaTime, 1f);
					var tween = Tweens.EaseOutQuad(transitionCurve);
					
					foreach (InterfaceElement element in previousElements) {
						element.SetEntered(0);
					}
					foreach (InterfaceElement element in currentElements) {
						enterValues[element] = Mathf.Max(tween, CachedEntryTween(element));
						element.SetEntered(enterValues[element]);
					}
					if (transitionCurve >= 1) {
						transitioning = false;
					}
				}
			}

		}

		private float CachedEntryTween (InterfaceElement element) {
			if (!enterValues.ContainsKey(element)) return 0;
			return enterValues[element];
		}

        private bool Contains(InterfaceElement[] currentElements, InterfaceElement element) {
            for (int i = 0; i < currentElements.Length; i++) {
                if (currentElements[i] == element) return true;
            }
            return false;
        }

        private class Set {

			public readonly string Name;
			public readonly InterfaceElement[] Elements;

			public Set (string name, InterfaceElement[] elements) {
				Name = name;
				Elements = elements;
			}

		}

	}

}