using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(LayoutNode))]
    public class InterfaceNodeEditor : Editor {

        public override void OnInspectorGUI () {
            InterfaceNodeDrawer.DrawNodeTreeProperties(serializedObject);
        }

    }

}