using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// InputNodes can arrange your various input-handling elements into a tree (or multiple trees). 
    /// This allows different parts of the trees to be enabled/disabled at various times. 
    /// </summary>
    public abstract class InterfaceNode : MonoBehaviour {

        [SerializeField] private InterfaceNode inputParent;
        private bool validatedParent;

        public abstract MouseTarget GetMouseTarget(Vector3 mouseWorldPosition);
        
        public virtual bool InputIsDisabled { get; }

        public InterfaceNode InputParent {
            get {
                if (!validatedParent) {
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
                if (InterfaceConfig.DisableInput) {
                    return false;
                }
                var node = this;
                var foundLockRoot = false;
                while (node != null) {
                    if (node.InputIsDisabled) return false;
                    foundLockRoot |= (node == InterfaceConfig.LockedNode);
                    node = node.inputParent;
                }
                if (InterfaceConfig.LockedNode != null && !foundLockRoot) {
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
            validatedParent = true;
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

        public void Attach (InterfaceNode target) {
            if (target != null && !target.ParentCausesHierarchyLoop(this)) {
                target.transform.SetParent(AttachTarget.transform, false);
                target.InputParent = this;
            }
        }

        public void Attach(GameObject target) {
            if (target != null) {
                target.transform.SetParent(AttachTarget.transform, false);
            }
        }

        protected void AttachTo(InterfaceNode target, Transform transform) {
            if (target != null && !target.ParentCausesHierarchyLoop(this)) {
                target.transform.SetParent(transform, false);
                target.InputParent = this;
            }
        }

        protected void AttachTo(GameObject target, Transform transform) {
            if (target != null) {
                target.transform.SetParent(transform, false);
            }
        }

        protected virtual Transform AttachTarget => transform;

    }

}