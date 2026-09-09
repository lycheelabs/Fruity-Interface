using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class InterfaceLayer : MonoBehaviour {

        public const int DefaultLayer = 0;

        public int Layer;

        public static int GetLayer(InterfaceNode node) {
            const int MAX_DEPTH = 100;
            int depth = 0;
            while (node != null) {
                if (depth++ > MAX_DEPTH) {
                    Debug.LogWarning("Potential infinite loop detected resolving InterfaceLayer. Breaking.");
                    return DefaultLayer;
                }
                var layer = node.GetComponent<InterfaceLayer>();
                if (layer != null) {
                    return layer.Layer;
                }
                node = node.InputParent;
            }
            return DefaultLayer;
        }

    }

}
