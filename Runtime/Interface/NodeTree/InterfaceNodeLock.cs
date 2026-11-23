using UnityEngine;

namespace LycheeLabs.InputTree {

    public class InterfaceNodeLock : MonoBehaviour {

        private int _lockedLayer;
        private bool _isLocked;

        private void OnEnable() {
            var node = GetComponent<InterfaceNode>();
            if (node == null) {
                Debug.LogError($"InterfaceNodeLock on '{name}' requires an InterfaceNode on the same GameObject.", this);
                return;
            }
            _lockedLayer = InterfaceLayer.GetLayer(node);
            _isLocked = true;
            Interface.LockLayer(_lockedLayer);
        }

        private void OnDisable() {
            if (_isLocked) {
                Interface.UnlockLayer(_lockedLayer);
                _isLocked = false;
            }
        }

    }

}
