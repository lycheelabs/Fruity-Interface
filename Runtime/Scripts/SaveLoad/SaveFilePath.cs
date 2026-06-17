using System;

namespace LycheeLabs.FruityInterface.SaveLoad {

    public class SaveFilePath<T> where T : SaveFile {

        public string RelativePath;
        public bool Encrypt;

        public SaveFilePath (string relativePath, bool encrypt) {
            RelativePath = relativePath;
            Encrypt = encrypt;
        }

        public bool Exists () {
            return SaveManager.Exists(RelativePath);
        }

        public LoadData<T> Load () {
            return SaveManager.TryLoad<T>(RelativePath, Encrypt);
        }

        public LoadData<T> LoadIfExists () {
            if (Exists()) {
                return SaveManager.TryLoad<T>(RelativePath, Encrypt);
            }
            return new LoadData<T>() { IsValid = false };
        }

        public void Delete () {
            SaveManager.DeleteFile(RelativePath);
        }

    }

}
