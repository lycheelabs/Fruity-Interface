using UnityEditor;
using LycheeLabs.InputTree;

namespace LycheeLabs.InputTree.Editors {

    [CustomEditor(typeof(LayoutNode))]
    public class LayoutNodeEditor : UnityEditor.Editor {

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawLayoutProperties(serializedObject);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
        }

    }

}
