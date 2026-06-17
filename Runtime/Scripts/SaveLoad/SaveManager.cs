using UnityEngine;

namespace LycheeLabs.FruityInterface.SaveLoad {
    public static class SaveManager {

        /// <summary>
        /// Returns true if a savedata file exists at the specified path
        /// </summary>
        public static bool Exists (string relativePath) {
            return AtomicSaveManager.SaveFileExists(relativePath);
        }

        /// <summary>
        /// Saves the data to the specified relative path.
        /// Creates directories automatically if needed.
        /// </summary>
        public static bool TrySave<T>(T data, string relativePath, bool encrypt) where T : SaveFile {
            if (!AtomicSaveManager.TryCreateDirectory(relativePath)) {
                return false;
            }
            return AtomicSaveManager.TrySaveData(data, relativePath, encrypt);
        }

        /// <summary>
        /// Loads the data from the specified relative path.
        /// Returns a default instance if the file doesn't exist or is invalid.
        /// </summary>
        public static LoadData<T> TryLoad<T>(string relativePath, bool decrypt) where T : SaveFile {
            var result = AtomicSaveManager.TryLoadData<T>(relativePath, decrypt);
            if (result.IsValid)
                return result;

            Debug.LogWarning($"No valid savedata found at {relativePath}.");
            return LoadData<T>.Invalid;          
        }

        /// <summary>
        /// Deletes a save file and its backup.
        /// </summary>
        public static void DeleteFile (string relativePath) {
            AtomicSaveManager.TryDeleteFile(relativePath); 
        }

    }

}
