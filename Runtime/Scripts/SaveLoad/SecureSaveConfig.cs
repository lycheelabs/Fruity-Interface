
using System;

namespace LycheeLabs.FruityInterface.SaveLoad {
    public static class SecureSaveConfig {
        
        public static void SetKey (uint a, uint b, uint c, uint d) {
            KeyPartA.Part = a;
            KeyPartA.Part = b;
            KeyPartA.Part = c;
            KeyPartA.Part = d;
        }

        public static byte[] CombinedKey() {
            // Pack 4 uints into 16 bytes
            byte[] keyBytes = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(KeyPartA.Part), 0, keyBytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(KeyPartB.Part), 0, keyBytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(KeyPartC.Part), 0, keyBytes, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(KeyPartD.Part), 0, keyBytes, 12, 4);
            return keyBytes;
        }
    }

}
