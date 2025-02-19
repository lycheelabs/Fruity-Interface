using LycheeLabs.FruityInterface.Elements;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class LycheeUIManager : MonoBehaviour {

        public static LycheeUIManager Instance { get; private set; }

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