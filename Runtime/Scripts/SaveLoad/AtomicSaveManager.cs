using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace LycheeLabs.FruityInterface.SaveLoad {

    internal static class AtomicSaveManager {

        private static string BaseDirectory => Application.persistentDataPath;
        private static string GetSavePath(string relativePath) => Path.Combine(BaseDirectory, relativePath);
        private static string GetBackupPath(string relativePath) => Path.Combine(BaseDirectory, relativePath + ".bak");
        private static string GetTempPath(string relativePath) => Path.Combine(BaseDirectory, relativePath + ".tmp");
        
        private static bool ShouldPreserveBackups => Application.isEditor;


        // --- Public API ---
        public static bool SaveFileExists (string relativePath) {
            try {
                var savePath = GetSavePath(relativePath);
                if (File.Exists(savePath))
                    return true;

                var backupPath = GetBackupPath(relativePath);
                if (File.Exists(backupPath))
                    return true;

                return false;
            }
            catch (Exception e) {
                Debug.LogError($"SaveFileExists failed: {e}");
                return false;
            }
        }

        public static bool TryCreateDirectory(string relativePath) {
            string fullPath = GetSavePath(relativePath);
            string dir = Path.GetDirectoryName(fullPath) ?? Application.persistentDataPath;

            // Ensure directory exists
            try {
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                return true;
            }
            catch (Exception e) {
                Debug.LogError($"TryCreateDirectory could not create directory for {relativePath}: {e}");
                return false;
            }
        }

        public static bool TrySaveData<T>(T data, string relativePath, bool encrypt) where T : SaveFile {
            try {
                byte[] serialized = SerializeData(data, encrypt);
                var finalPath = GetSavePath(relativePath);
                var tempPath = GetTempPath(relativePath);
                var backupPath = GetBackupPath(relativePath);
                return WriteFileAtomic<T>(serialized, finalPath, tempPath, backupPath, encrypt);
            }
            catch (Exception e) {
                Debug.LogError($"SaveGame failed: {e}");
                return false;
            }
        }

        public static LoadData<T> TryLoadData<T>(string relativePath, bool decrypt) where T : SaveFile {
            try {
                var savePath = GetSavePath(relativePath);
                if (File.Exists(savePath))
                    return DeserializeData<T>(File.ReadAllBytes(savePath), decrypt);

                var backupPath = GetBackupPath(relativePath);
                if (File.Exists(backupPath)) {
                    Debug.LogWarning("Main save missing, loading backup...");
                    return DeserializeData<T>(File.ReadAllBytes(backupPath), decrypt);
                }
            }
            catch (Exception e) {
                Debug.LogError($"LoadGame failed: {e}");
            }

            return LoadData<T>.Invalid; 
        }

        public static void TryDeleteFile(string relativePath) {
            DeleteFile(GetSavePath(relativePath));
            DeleteFile(GetBackupPath(relativePath));
            DeleteFile(GetTempPath(relativePath));
        }

        // --- Core Serialization ---
        private static byte[] SerializeData<T>(T data, bool encrypt) {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            if (!encrypt) {
                return bytes; // just plain UTF8 bytes
            }

            // compress
            byte[] compressed = Compress(bytes);
            byte[] key, iv;
            GetKeyAndIV(out key, out iv);
            EnsureKeyIvLengths(key, iv);

            byte[] encrypted = Encrypt(compressed, key, iv);
            return encrypted;
        }

        private static LoadData<T> DeserializeData<T>(byte[] data, bool decrypt) where T : SaveFile {
            byte[] bytes = data;

            if (decrypt) {
                // decrypt
                byte[] key, iv;
                GetKeyAndIV(out key, out iv);
                EnsureKeyIvLengths(key, iv);

                bytes = Decrypt(bytes, key, iv);

                // decompress
                bytes = Decompress(bytes);
            }

            string json = Encoding.UTF8.GetString(bytes);
            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidDataException("JSON was empty.");

            T result = JsonConvert.DeserializeObject<T>(json);
            if (result == null)
                throw new InvalidDataException("JSON failed to deserialize into " + typeof(T).Name);

            return new LoadData<T>(result);
        }

        // --- Atomic Write & Validation ---
        private static bool WriteFileAtomic<T>(byte[] data, string finalPath, string tempPath, string backupPath, bool encrypt) where T : SaveFile {
            try {

                // 1. Ensure backup recovery before saving
                if (File.Exists(finalPath) && !ValidateFile<T>(finalPath, encrypt) && File.Exists(backupPath)) {
                    Debug.LogWarning($"Detected a save backup at {backupPath}");
                    try {
                        // Move backup to main save if main save is missing or corrupted
                        File.Copy(backupPath, finalPath, overwrite: true);
                        Debug.LogWarning($"Recovered main save from backup at {backupPath}");
                    }
                    catch (Exception e) {
                        Debug.LogError($"Failed to recover save from backup: {e}");
                        return false;
                    }
                }

                // 2. Write to temp with write-through to avoid cached-only writes
                const int bufferSize = 4096;
                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.WriteThrough)) {
                    fs.Write(data, 0, data.Length);
                    // explicit flush not required when using WriteThrough, but safe to call:
                    fs.Flush();
                }

                // 3. Validate temp file
                if (!ValidateFile<T>(tempPath, encrypt)) {
                    Debug.LogError("Temp save validation failed — keeping old save");
                    DeleteFile(tempPath);
                    return false;
                }

                // 4. Backup & 5. Replace atomically if possible
                try {
                    if (File.Exists(finalPath)) {
                        // Try atomic replace which also creates a backup (Windows supports this)
                        File.Replace(tempPath, finalPath, backupPath, ignoreMetadataErrors: true);
                    }
                    else {
                        // No existing file -> just move
                        File.Move(tempPath, finalPath);
                    }
                }
                catch (PlatformNotSupportedException) {
                    // Fallback for platforms that don't support File.Replace
                    if (File.Exists(finalPath))
                        File.Copy(finalPath, backupPath, true);

                    File.Copy(tempPath, finalPath, true);
                    DeleteFile(tempPath);
                }
                catch (Exception replaceEx) {
                    Debug.LogWarning($"Atomic replace failed ({replaceEx}). Falling back to copy/replace.");
                    // Fallback strategy
                    if (File.Exists(finalPath))
                        File.Copy(finalPath, backupPath, true);

                    File.Copy(tempPath, finalPath, true);
                    DeleteFile(tempPath);
                }

                // 6. Validate final file and clean up backup if successful
                if (ValidateFile<T>(finalPath, encrypt, logging: false)) {
                    // Success: Remove the backup to keep directory clean (except in Editor mode for debugging)
                    if (!ShouldPreserveBackups) {
                        DeleteFile(backupPath);
                    }
                    return true;
                }
                else {
                    // Validation failed: Keep backup and try to restore it
                    Debug.LogError("Final save validation failed! Attempting to restore from backup...");
                    if (File.Exists(backupPath)) {
                        try {
                            File.Copy(backupPath, finalPath, overwrite: true);
                            Debug.LogWarning("Restored save from backup after validation failure");
                        }
                        catch (Exception restoreEx) {
                            Debug.LogError($"Failed to restore from backup: {restoreEx}");
                        }
                    }
                    return false;
                }
            }
            catch (Exception e) {
                Debug.LogError($"WriteFileAtomic failed: {e}");
                DeleteFile(tempPath);
                return false;
            }
        }

        private static bool ValidateFile<T>(string path, bool encrypt, bool logging = true) where T : SaveFile {
            try {
                var fi = new FileInfo(path);
                if (fi.Length < 8) // Too small to be valid
                    return false;

                // Try to deserialize -- DeserializeData will throw on invalid crypto / format
                byte[] fileBytes = File.ReadAllBytes(path);
                var result = DeserializeData<T>(fileBytes, encrypt);
                return result.IsValid;
            }
            catch (Exception e) {
                if (logging) {
                    Debug.LogWarning($"ValidateFile failed for {path}: {e.Message}");
                }
                return false;
            }
        }

        private static void DeleteFile(string path) {
            try {
                if (File.Exists(path)) {
                    File.Delete(path);
                }
            }
            catch (Exception e) {
                Debug.LogWarning($"Failed to delete file {path}: {e}");
            }
        }

        // --- Compression ---
        private static byte[] Compress(byte[] data) {
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(mso, CompressionMode.Compress, leaveOpen: true)) {
                gs.Write(data, 0, data.Length);
            }
            return mso.ToArray();
        }

        private static byte[] Decompress(byte[] data) {
            using var msi = new MemoryStream(data);
            using var gs = new GZipStream(msi, CompressionMode.Decompress);
            using var msOut = new MemoryStream();
            gs.CopyTo(msOut);
            return msOut.ToArray();
        }

        // --- Encryption ---
        private static byte[] Encrypt(byte[] data, byte[] key, byte[] iv) {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock(); // Ensure all bytes are written
            }
            return ms.ToArray();
        }

        private static byte[] Decrypt(byte[] data, byte[] key, byte[] iv) {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write)) {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock(); // Ensure all bytes are written
            }
            return ms.ToArray();
        }

        // --- Key Obfuscation ---
        private static void GetKeyAndIV(out byte[] key, out byte[] iv) {
            // Expect the consumer to provide a combined key byte array via SecureSaveConfig
            byte[] keyBytes = SecureSaveConfig.CombinedKey();
            if (keyBytes == null || keyBytes.Length == 0)
                throw new InvalidOperationException("SecureSaveConfig.CombinedKey() returned null/empty — set the key before saving/loading.");

            using (var sha256 = SHA256.Create()) {
                byte[] hash = sha256.ComputeHash(keyBytes);
                key = new byte[16];
                iv = new byte[16];
                Array.Copy(hash, 0, key, 0, 16);
                Array.Copy(hash, 16, iv, 0, 16);
            }
        }

        private static void EnsureKeyIvLengths(byte[] key, byte[] iv) {
            if (key == null || iv == null || key.Length < 16 || iv.Length < 16)
                throw new InvalidOperationException("Derived key/iv are not of required lengths (16 bytes).");
        }
    }

}
