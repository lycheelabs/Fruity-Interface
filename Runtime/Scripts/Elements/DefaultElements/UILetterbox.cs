using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    public class UILetterbox : MonoBehaviour {

		public static UILetterbox Instantiate (Transform parent) {
			var instance = FruityUIPrefabs.NewUILetterbox().GetComponent<UILetterbox>();
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
			float width = InterfaceConfig.LetterboxWidth / InterfaceConfig.UIScaling;
			float height = InterfaceConfig.LetterboxHeight / InterfaceConfig.UIScaling;

			borderL.sizeDelta = new Vector2 (width / 2, 0);
			borderR.sizeDelta = new Vector2 (width / 2, 0);
			borderU.sizeDelta = new Vector2 (0, height / 2);
			borderD.sizeDelta = new Vector2 (0, height / 2);
        }

	}

}