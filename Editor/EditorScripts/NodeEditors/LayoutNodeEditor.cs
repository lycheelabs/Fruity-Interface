using UnityEditor;
using LycheeLabs.FruityInterface;

namespace LycheeLabs.FruityInterface.Editor {

    [CustomEditor(typeof(LayoutNode))]
    public class LayoutNodeEditor : UnityEditor.Editor {

        public override void OnInspectorGUI () {
            FruityEditorDrawer.DrawLayoutProperties(serializedObject);
            FruityEditorDrawer.DrawNodeTreeProperties(serializedObject);
        }

    }

}
