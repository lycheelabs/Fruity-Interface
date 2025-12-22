using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// InputNodes can arrange your various input-handling elements into a tree (or multiple trees). 
    /// This allows different parts of the trees to be enabled/disabled at various times. 
    /// </summary>
    public abstract class InterfaceNode : MonoBehaviour {

        [SerializeField] private InterfaceNode inputParent;
        [SerializeField] private bool ignoresInterfaceLock;
        private bool hasValidatedParent;

        public virtual MouseTarget GetMouseTarget(Vector3 mouseWorldPosition, MouseButton pressedButton) {
            return this as MouseTarget;
        }

        private void OnValidate() {
            // Default input parent
            if (inputParent == null && transform.parent != null) {
                InputParent = transform.parent.GetComponent<InterfaceNode>();
            }
        }

        public virtual bool InputIsDisabled { get; }

        public InterfaceNode InputParent {
            get {
                if (!hasValidatedParent) {
                    ValidateParent(inputParent);
                } 
                return inputParent;
            }
            set {
                if (inputParent == value) return;
                ValidateParent(value);
                inputParent = value;
            }
        }

        public bool InputEnabledInHierarchy {
            get {
                if (FruityUI.DisableInput) {
                    return false;
                }
                var node = this;
                var foundLockRoot = false;
                while (node != null) {
                    if (node.InputIsDisabled) return false;
                    foundLockRoot |= (node == FruityUI.LockedNode || node.ignoresInterfaceLock);
                    node = node.inputParent;
                }
                if (FruityUI.LockedNode != null && !foundLockRoot) {
                    return false;
                }
                return true;
            }
        }

        private void ValidateParent (InterfaceNode newParent) {
            if (ParentCausesHierarchyLoop(newParent)) {
                Debug.LogError(string.Format("This parent would cause a loop in the InputNode hierarchy: {0} <-- {1}", newParent, this));
                return;
            }
            hasValidatedParent = true;
        }

        private bool ParentCausesHierarchyLoop (InterfaceNode node) {
            if (node == this) return true;
            if (node == null || node == inputParent) return false;
            return NodeIsAChild(node);
        }

        public bool NodeIsAParent (InterfaceNode target) {
            if (target == null) return false;

            var node = this;
            while (node != null) {
                node = node.inputParent;
                if (node == target) return true;
            }
            return false;
        }

        public bool NodeIsAChild(InterfaceNode target) {
            if (target == null) return false;
            return target.NodeIsAParent(this);
        }

        protected void AttachTo (InterfaceNode target, Transform transform) {
            if (target != null && !target.ParentCausesHierarchyLoop(this)) {
                target.transform.SetParent(transform, false);
                target.InputParent = this;
                target.OnNewChildAttached();
            }
        }

        public void Attach (InterfaceNode target) {
            if (target != null && !target.ParentCausesHierarchyLoop(this)) {
                target.transform.SetParent(AttachTarget.transform, false);
                target.InputParent = this;
                OnNewChildAttached();
            }
        }

        public void Attach(GameObject target) {
            if (target != null) {
                target.transform.SetParent(AttachTarget.transform, false);
                OnNewChildAttached();
            }
        }

        protected virtual Transform AttachTarget => transform;
        protected virtual void OnNewChildAttached () { }

    }

}