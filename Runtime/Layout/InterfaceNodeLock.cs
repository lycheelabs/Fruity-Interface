using UnityEngine;

namespace LycheeLabs.FruityInterface {

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
            FruityUI.LockLayer(_lockedLayer);
        }

        private void OnDisable() {
            if (_isLocked) {
                FruityUI.UnlockLayer(_lockedLayer);
                _isLocked = false;
            }
        }

    }

}
