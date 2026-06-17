using System;

namespace LycheeLabs.FruityInterface.SaveLoad {

    [Serializable]
    public abstract class SaveFile {

        public abstract bool Validate ();

    }

}
