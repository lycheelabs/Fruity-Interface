using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class FruityUIManager : MonoBehaviour {

        public static FruityUIManager Instance { get; private set; }

        // ------------------------------------------------------------------------

        public ScreenAspect MinAspectRatio = ScreenAspect.STANDARD;
        public ScreenAspect MaxAspectRatio = ScreenAspect.ULTRAWIDE;

        private void Start () {
            Instance = this;
        }

        void Update () {

            // Update the interface
            UIConfig.Update(MinAspectRatio, MaxAspectRatio);

            // Update the scene
            GrabTarget.UpdateCurrentGrab();

        }

    }

}