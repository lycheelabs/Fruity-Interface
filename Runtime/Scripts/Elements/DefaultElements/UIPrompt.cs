using UnityEngine;

namespace LycheeLabs.FruityInterface.Elements {

    [RequireComponent(typeof(RectTransform))]

    /// <summary>
    /// A UINode that attaches to a canvas. Can be activated and dismissed.
    /// </summary>
    public abstract class UIPrompt : UINode {

        public static T SpawnGeneric<T> (string name, UICanvas canvas) where T : UIPrompt {
            var rootObject = LycheeUIPrefabs.NewUIRect();
            rootObject.name = name;

            var prompt = rootObject.AddComponent<T>();
            canvas?.Attach(prompt);

            return prompt;
        } 

        public abstract void Activate ();

        public void Complete () {
            if (!HasCompleted) {
                Destroy(gameObject);
                HasCompleted = true;
                ReadyToContinue = true;
            }
        }

        public void ContinueNow() {
            ReadyToContinue = true;
        }

        public bool HasCompleted { get; private set; }
        public bool ReadyToContinue { get; private set; }

    }

}