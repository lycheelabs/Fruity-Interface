using System;
using System.Collections.Generic;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class InterfaceModeSwitcher<T> where T : Enum {

		private static readonly EnteringElement[] None = Array.Empty<EnteringElement>();

		// -----------------------------------------------

		private readonly List<Set> sets;
		private Dictionary<EnteringElement, float> enterValues;

		private EnteringElement[] previousElements;
		private EnteringElement[] currentElements;
		private T activeMode;

		private bool transitioning;
		private bool exiting;
		private float transitionCurve;

		public T CurrentMode => activeMode;

        public InterfaceModeSwitcher() {
			sets = new List<Set>();
			enterValues = new Dictionary<EnteringElement, float>();

			previousElements = None;
			currentElements = None;
		}

		public void AddMode (T mode, params EnteringElement[] elements) {
			var modeName = mode.ToString();
			var set = new Set(modeName, elements);
			sets.Add(set);

			foreach (var element in elements) {
                element.SetEnterTween(0);
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

		private void StartTransition (EnteringElement[] newElements) {
			// Incase previous transition didn't finish
			foreach (var element in previousElements) {
				if (!Contains(newElements, element)) {
					element.SetEnterTween(0);
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
					var tween = Tweens.EaseOutQuad(transitionCurve);

					foreach (EnteringElement element in previousElements) {
						if (Contains(currentElements, element)) {
							// This element is not exiting
							enterValues[element] = 1;
						} else {
							enterValues[element] = Mathf.Min(tween, CachedEntryTween(element));
						}
						element.SetEnterTween(enterValues[element]);
					}
					if (transitionCurve <= 0) {
						exiting = false;
					}
				}

				// Transition new element in
				if (!exiting) {
					transitionCurve = Mathf.Min(transitionCurve + 6f * Time.deltaTime, 1f);
					var tween = Tweens.EaseOutQuad(transitionCurve);
					
					foreach (EnteringElement element in previousElements) {
						element.SetEnterTween(0);
					}
					foreach (EnteringElement element in currentElements) {
						enterValues[element] = Mathf.Max(tween, CachedEntryTween(element));
						element.SetEnterTween(enterValues[element]);
					}
					if (transitionCurve >= 1) {
						transitioning = false;
					}
				}
			}

		}

		private float CachedEntryTween (EnteringElement element) {
			if (!enterValues.ContainsKey(element)) return 0;
			return enterValues[element];
		}

        private bool Contains(EnteringElement[] currentElements, EnteringElement element) {
            for (int i = 0; i < currentElements.Length; i++) {
                if (currentElements[i] == element) return true;
            }
            return false;
        }

        private class Set {

			public readonly string Name;
			public readonly EnteringElement[] Elements;

			public Set (string name, EnteringElement[] elements) {
				Name = name;
				Elements = elements;
			}

		}

	}

}