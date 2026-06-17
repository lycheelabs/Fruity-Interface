namespace LycheeLabs.FruityInterface.SaveLoad {

    public struct LoadData<T> where T : SaveFile {

        public static LoadData<T> Invalid => new LoadData<T>();

        public bool IsValid;
        public T Data;

        public LoadData(T data) {
            IsValid = data != null && data.Validate();
            Data = data;
        }

    }

}
