using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// InputNodes can arrange your various input-handling elements into a tree (or multiple trees). 
    /// This allows different parts of the trees to be enabled/disabled at various times. 
    /// </summary>
    public abstract class InterfaceNode : MonoBehaviour, TreeNode {

        private InterfaceNode automaticInputParent;
        [SerializeField] private InterfaceNode inputParentOverride;
        [SerializeField] private bool inputDisabled;
        [Obsolete, SerializeField, HideInInspector] private bool ignoreInterfaceLock;

        public virtual MouseTarget GetMouseTarget(Vector3 mouseWorldPosition, MouseButton pressedButton) {
            return this as MouseTarget;
        }

        private void Awake() {
            RefreshAutomaticInputParent();
        }

        private void OnTransformParentChanged() {
            RefreshAutomaticInputParent();
        }

        private void RefreshAutomaticInputParent() {
            automaticInputParent = null;
            if (transform.parent != null) {
                automaticInputParent = transform.parent.GetComponentInParent<InterfaceNode>();
            }
        }

        public bool InputIsDisabled => inputDisabled || ForceInputDisabled;

        protected virtual bool ForceInputDisabled => false;

        public void SetInputDisabled (bool disabled) {
            inputDisabled = disabled;
        }

        /// <summary>The layer index for this node, resolved from the first InterfaceLayer found walking up the hierarchy.</summary>
        public int LayerIndex => InterfaceLayer.GetLayer(this);

        public InterfaceNode InputParent => (inputParentOverride != null) 
            ? inputParentOverride : automaticInputParent;

        public InterfaceNode InputParentOverride {
            get => inputParentOverride;
            set {
                if (value != null && TreeNodeHierarchy.WouldCreateLoop(this, value)) {
                    Debug.LogError(string.Format("This parent would cause a loop in the InputNode hierarchy: {0} <-- {1}", value, this));
                    return;
                }
                inputParentOverride = value;
            }
        }

        public bool InputEnabledInHierarchy => TreeNodeHierarchy.IsInputEnabled(this);

        public bool NodeIsAParent (InterfaceNode target) {
            return TreeNodeHierarchy.HasAncestor(this, target);
        }

        public bool NodeIsAChild(InterfaceNode target) {
            if (target == null) return false;
            return target.NodeIsAParent(this);
        }

        protected void AttachTo (InterfaceNode target, Transform transform) {
            if (target != null && !TreeNodeHierarchy.WouldCreateLoop(this, target)) {
                target.transform.SetParent(transform, false);
            }
        }

        public void Attach (InterfaceNode target) {
            if (target != null && !TreeNodeHierarchy.WouldCreateLoop(this, target)) {
                target.transform.SetParent(AttachTarget.transform, false);
            }
        }

        public void Attach(GameObject target) {
            if (target != null) {
                target.transform.SetParent(AttachTarget.transform, false);
            }
        }

        protected virtual Transform AttachTarget => transform;

        TreeNode TreeNode.Parent => InputParent != null ? InputParent : null;

        bool TreeNode.IsInputDisabled => InputIsDisabled;

        int TreeNode.Layer => LayerIndex;

    }

}
