using UnityEngine;

namespace LycheeLabs.FruityInterface.Animation {
    public class JuicyGroup : JuicyAnimator {

        public JuicyChild[] Children;

        protected override void Apply(ref TransformData transformData) {
            for (int i =0; i < Children.Length; i++) {
                var child = Children[i];
                var childTransformData = transformData;

                childTransformData.position += child.basePosition;
                childTransformData.scale = Vector3.Scale(transformData.scale, child.baseScale);
                childTransformData.rotation += child.baseRotation;

                if (child.RectTransform == null) {
                    child.transform.localPosition = childTransformData.position;
                }
                else {
                    child.RectTransform.anchoredPosition = childTransformData.position;
                }
                child.transform.localEulerAngles = childTransformData.rotation;
                child.transform.localScale = childTransformData.scale;
            }
        }

    }

}