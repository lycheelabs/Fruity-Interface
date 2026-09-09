using UnityEditor;
using LycheeLabs.FruityInterface;

namespace LycheeLabs.FruityInterface.Editor {

    [CustomEditor(typeof(InterfaceNode))]
    public class InterfaceNodeEditor : UnityEditor.Editor {

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
        }

    }

}
