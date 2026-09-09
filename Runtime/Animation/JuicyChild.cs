using UnityEngine;

namespace LycheeLabs.FruityInterface.Animation {

    [ExecuteAlways]
    public class JuicyChild : MonoBehaviour {

        public Vector3 basePosition;
        public Vector3 baseRotation;
        public Vector3 baseScale = Vector3.one;

        private RectTransform rectTransform;
        public RectTransform RectTransform => rectTransform;

        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            basePosition = (rectTransform == null) ? transform.localPosition : rectTransform.anchoredPosition;
            baseRotation = transform.localEulerAngles;
            baseScale = transform.localScale;
        }

        private void OnValidate() {
            rectTransform = GetComponent<RectTransform>();
            basePosition = (rectTransform == null) ? transform.localPosition : rectTransform.anchoredPosition;
            baseRotation = transform.localEulerAngles;
            baseScale = transform.localScale;
        }

        private void Update() {}

    }

}