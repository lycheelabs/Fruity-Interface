using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class FruityUIManager : MonoBehaviour {

        public static FruityUIManager Instance { get; private set; }

        public static void Queue(InterfaceEvent newEvent) {
            Instance.tree.QueueEvent(newEvent);
        }
        
        // ------------------------------------------------------------------------

        public ScreenAspect MinAspectRatio = ScreenAspect.STANDARD;
        public ScreenAspect MaxAspectRatio = ScreenAspect.ULTRAWIDE;
        public bool LogEvents;

        private InterfaceTree tree;
        
        private void Start () {
            Instance = this;
            tree = new InterfaceTree();
        }

        void Update () {

            // Update the interface
            InterfaceConfig.Update(MinAspectRatio, MaxAspectRatio);
            Mouse.Update(LogEvents);
            tree.Update(LogEvents);
            
            // Update the scene
            GrabTarget.UpdateCurrentGrab();
            
        }

    }

}