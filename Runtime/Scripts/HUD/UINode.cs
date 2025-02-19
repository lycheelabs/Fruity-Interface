using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// InputNodes can arrange your various input-handling elements into a tree (or multiple trees). 
    /// This allows different parts of the trees to be enabled/disabled at various times. 
    /// </summary>
    public class UINode : MonoBehaviour {

        [SerializeField] private UINode inputParent;
        private bool validatedParent;

        public UINode InputParent {
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

        private void ValidateParent (UINode newParent) {
            if (ParentCausesHierarchyLoop(newParent)) {
                Debug.LogError(string.Format("This parent would cause a loop in the InputNode hierarchy: {0} <-- {1}", newParent, this));
                return;
            }
            validatedParent = true;
        }

        private bool ParentCausesHierarchyLoop (UINode node) {
            if (node == this) return true;
            if (node == null || node == inputParent) return false;
            return NodeIsAChild(node);
        }

        public bool NodeIsAParent (UINode target) {
            if (target == null) return false;

            var node = this;
            while (node != null) {
                node = node.inputParent;
                if (node == target) return true;
            }
            return false;
        }

        public bool NodeIsAChild(UINode target) {
            if (target == null) return false;
            return target.NodeIsAParent(this);
        }

        // ------------------------------------------------------------------------

        public virtual bool InputIsDisabled { get; }

        public bool InputEnabledInHierarchy {
            get {
                if (UIConfig.DisableInput) {
                    return false;
                }
                var node = this;
                var foundLockRoot = false;
                while (node != null) {
                    if (node.InputIsDisabled) return false;
                    foundLockRoot |= (node == UIConfig.LockedNode);
                    node = node.inputParent;
                }
                if (UIConfig.LockedNode != null && !foundLockRoot) {
                    return false;
                }
                return true;
            }
        }

        public void Attach (UINode target) {
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

        public void AttachTo(UINode target, Transform transform) {
            if (target != null && !target.ParentCausesHierarchyLoop(this)) {
                target.transform.SetParent(transform, false);
                target.InputParent = this;
            }
        }

        public void AttachTo(GameObject target, Transform transform) {
            if (target != null) {
                target.transform.SetParent(transform, false);
            }
        }

        protected virtual Transform AttachTarget => transform;

    }

}