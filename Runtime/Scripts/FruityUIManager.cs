using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class FruityUIManager : MonoBehaviour {

        public static FruityUIManager Instance { get; private set; }

        // ------------------------------------------------------------------------

        public ScreenAspect MinAspectRatio = ScreenAspect.STANDARD;
        public ScreenAspect MaxAspectRatio = ScreenAspect.ULTRAWIDE;
        public bool LogEvents;
        
        private void Start () {
            Instance = this;
        }

        void Update () {

            // Update the interface
            InterfaceConfig.Update(MinAspectRatio, MaxAspectRatio);
            Mouse.Update(LogEvents);

            // Update the scene
            GrabTarget.UpdateCurrentGrab();

            
        }

    }

}