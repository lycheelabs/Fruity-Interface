using UnityEngine;

namespace LycheeLabs.InputTree.Elements {

    public class FullscreenLetterboxNode : InterfaceNode {

		public static FullscreenLetterboxNode Instantiate (Transform parent) {
			var instance = InputTreePrefabs.InstantiateFullscreenLetterbox();
			instance.transform.SetParent(parent, false);
			return instance;
        }

		public RectTransform borderL;
		public RectTransform borderR;
		public RectTransform borderU;
		public RectTransform borderD;

        private void Awake () {
			Update();
        }

		private void Update () {
			float width = Interface.ScreenBounds.LetterboxWidth / Interface.ScreenBounds.UIScaling;
			float height = Interface.ScreenBounds.LetterboxHeight / Interface.ScreenBounds.UIScaling;

			borderL.sizeDelta = new Vector2 (width / 2, 0);
			borderR.sizeDelta = new Vector2 (width / 2, 0);
			borderU.sizeDelta = new Vector2 (0, height / 2);
			borderD.sizeDelta = new Vector2 (0, height / 2);
        }

	}

}
