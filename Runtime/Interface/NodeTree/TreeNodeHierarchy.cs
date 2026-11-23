using UnityEngine;

namespace LycheeLabs.InputTree {

    internal static class TreeNodeHierarchy {

        private const int MAX_PARENT_DEPTH = 100;

        public static bool WouldCreateLoop(TreeNode node, TreeNode newParent) {
            if (newParent == node) return true;
            if (newParent == null) return false;
            return HasAncestor(newParent, node);
        }

        public static bool HasAncestor(TreeNode node, TreeNode ancestor) {
            if (ancestor == null) return false;

            var current = node;
            int depth = 0;
            while (current != null) {
                if (depth++ > MAX_PARENT_DEPTH) {
                    Debug.LogWarning("Potential infinite loop detected in InputNode hierarchy. Breaking iteration.");
                    return false;
                }
                current = current.Parent;
                if (current == ancestor) return true;
            }
            return false;
        }

        public static bool IsInputEnabled(TreeNode node) {
            var current = node;
            int depth = 0;
            while (current != null) {
                if (depth++ > MAX_PARENT_DEPTH) {
                    Debug.LogWarning("Potential infinite loop detected in InputNode hierarchy. Breaking iteration.");
                    return false;
                }
                if (current.IsInputDisabled) return false;
                current = current.Parent;
            }
            if (Interface.ActiveLayerThreshold > node.Layer) return false;
            if (!Interface.InterfaceIsLocked && Interface.DisableInput) return false;
            return true;
        }
    }

}
