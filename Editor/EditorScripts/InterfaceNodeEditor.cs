using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(InterfaceNode))]
    public class InterfaceNodeEditor : Editor {

        public override void OnInspectorGUI () {
            InterfaceNodeDrawer.DrawNodeTreeProperties(serializedObject);
        }

    }

}