using System;
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    /// <summary>
    /// InputNodes can arrange your various input-handling elements into a tree (or multiple trees). 
    /// This allows different parts of the trees to be enabled/disabled at various times. 
    /// </summary>
    public abstract class InterfaceNode : MonoBehaviour {

        private InterfaceNode automaticInputParent;
        [SerializeField] private InterfaceNode inputParentOverride;
        [SerializeField] private bool ignoreInterfaceLock;

        private const int MAX_PARENT_DEPTH = 100;

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

        public virtual bool InputIsDisabled { get; }

        public InterfaceNode InputParent => inputParentOverride ?? automaticInputParent;

        public InterfaceNode InputParentOverride {
            get => inputParentOverride;
            set {
                if (value != null && ParentCausesHierarchyLoop(value)) {
                    Debug.LogError(string.Format("This parent would cause a loop in the InputNode hierarchy: {0} <-- {1}", value, this));
                    return;
                }
                inputParentOverride = value;
            }
        }

        public bool InputEnabledInHierarchy {
            get {
                if (FruityUI.DisableInput) {
                    return false;
                }
                var node = this;
                var foundLockRoot = false;
                int depth = 0;
                while (node != null) {
                    if (depth++ > MAX_PARENT_DEPTH) {
                        Debug.LogWarning(string.Format("Potential infinite loop detected in InputNode hierarchy at {0}. Breaking iteration.", gameObject.name));
                        return false;
                    }
                    if (node.InputIsDisabled) return false;
                    foundLockRoot |= (node == FruityUI.LockedNode || node.ignoreInterfaceLock);
                    node = node.InputParent;
                }
                if (FruityUI.LockedNode != null && !foundLockRoot) {
                    return false;
                }
                return true;
            }
        }

        public bool NodeIsAParent (InterfaceNode target) {
            if (target == null) return false;

            var node = this;
            int depth = 0;
            while (node != null) {
                if (depth++ > MAX_PARENT_DEPTH) {
                    Debug.LogWarning(string.Format("Potential infinite loop detected in InputNode hierarchy at {0}. Breaking iteration.", gameObject.name));
                    return false;
                }
                node = node.InputParent;
                if (node == target) return true;
            }
            return false;
        }

        public bool NodeIsAChild(InterfaceNode target) {
            if (target == null) return false;
            return target.NodeIsAParent(this);
        }

        protected void AttachTo (InterfaceNode target, Transform transform) {
            if (target != null && !ParentCausesHierarchyLoop(target)) {
                target.transform.SetParent(transform, false);
                target.InputParentOverride = this;
                target.OnNewChildAttached();
            }
        }

        public void Attach (InterfaceNode target) {
            if (target != null && !ParentCausesHierarchyLoop(target)) {
                target.transform.SetParent(AttachTarget.transform, false);
                target.InputParentOverride = this;
                OnNewChildAttached();
            }
        }

        public void Attach(GameObject target) {
            if (target != null) {
                target.transform.SetParent(AttachTarget.transform, false);
                OnNewChildAttached();
            }
        }

        private bool ParentCausesHierarchyLoop(InterfaceNode newParent) {
            if (newParent == this) return true;
            if (newParent == null) return false;
            return newParent.NodeIsAParent(this);
        }

        protected virtual Transform AttachTarget => transform;
        protected virtual void OnNewChildAttached () { }

    }

}