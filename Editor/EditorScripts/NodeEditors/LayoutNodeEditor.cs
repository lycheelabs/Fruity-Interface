using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(LayoutNode))]
    public class LayoutNodeEditor : Editor {

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
            FruityEditorDrawer.DrawLayoutProperties(serializedObject); 
        }

    }

}