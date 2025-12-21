using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(LayoutNode))]
    public class LayoutNodeEditor : Editor {

        public override void OnInspectorGUI () {
            InterfaceNodeDrawer.DrawNodeTreeProperties(serializedObject);
            InterfaceNodeDrawer.DrawLayoutProperties(serializedObject); 
        }

    }

}