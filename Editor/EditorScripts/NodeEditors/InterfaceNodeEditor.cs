using UnityEditor;
using LycheeLabs.InputTree;

namespace LycheeLabs.InputTree.Editors {

    [CustomEditor(typeof(InterfaceNode))]
    public class InterfaceNodeEditor : UnityEditor.Editor {

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
        }

    }

}
