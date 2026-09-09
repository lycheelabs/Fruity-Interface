using UnityEditor;
using LycheeLabs.FruityInterface;

namespace LycheeLabs.FruityInterface.Editors {

    [CustomEditor(typeof(InterfaceNode))]
    public class InterfaceNodeEditor : UnityEditor.Editor {

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
        }

    }

}
