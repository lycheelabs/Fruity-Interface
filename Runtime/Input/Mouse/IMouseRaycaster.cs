using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public interface IMouseRaycaster {
        void CollideAndResolve(MouseButton button, out MouseTarget target, out InterfaceNode targetNode, out Vector3 targetPoint);
    }

}
