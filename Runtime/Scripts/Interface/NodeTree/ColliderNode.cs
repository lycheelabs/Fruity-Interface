
using UnityEngine;

namespace LycheeLabs.FruityInterface {

    public class ColliderNode : InterfaceNode {

        public InterfaceNode Behaviour;

        public override MouseTarget GetMouseTarget(Vector3 mouseWorldPosition, MouseButton pressedButton) {
            return Behaviour as MouseTarget;
        }

    }

}