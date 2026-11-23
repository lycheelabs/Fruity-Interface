using UnityEngine;

namespace LycheeLabs.InputTree {

    public abstract class NodeSFX : MonoBehaviour {

        public virtual void OnFirstHover () { }
        public virtual void OnClick () { }
        public virtual void OnTurnOn () { }
        public virtual void OnTurnOff () { }
        public virtual void OnSliderModified (float value) { }

    }

}
