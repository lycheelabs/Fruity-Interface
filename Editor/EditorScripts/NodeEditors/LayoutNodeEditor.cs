using UnityEditor;

namespace LycheeLabs.FruityInterface {

    [CustomEditor(typeof(LayoutNode))]
    public class LayoutNodeEditor : Editor {

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawLayoutProperties(serializedObject);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
        }

    }

}