using System;
using System.Text;

namespace Device.Fingerprint.Core.Encoders
{
    /// <summary>
    ///     RC4 class for encryption and decryption.
    /// </summar
    public static class Rc4
    {
        /// <summary>
        ///     Encrypt method.
        /// </summary>
        /// <param name="key">The key for encryption.</param>
        /// <param name="value">The value.</param>
        /// <returns>Encrypted value.</returns>
        public static string Encrypt(string key, string value)
        {
            var keyCharArray = key.ToCharArray();
            var valueCharArray = value.ToCharArray();
            var keyLength = key.Length;
            var valueLength = value.Length;
            var map = new int[256];

            for (var i = 0; i < 256; i++)
            {
                map[i] = i;
            }

            var j = 0;
            for (var i = 0; i < 256; i++)
            {
                j = (j + map[i] + keyCharArray[i % keyLength]) % 256;
                var x = map[i];
                map[i] = map[j];
                map[j] = x;
            }

            var encryptedValue = new StringBuilder();
            var encI = 0;
            var encJ = 0;
            for (var y = 0; y < valueLength; y++)
            {
                encI = (encI + 1) % 256;
                encJ = (encJ + map[encI]) % 256;
                var x = map[encI];
                map[encI] = map[encJ];
                map[encJ] = x;
                encryptedValue.Append(Convert.ToChar(valueCharArray[y] ^ map[(map[encI] + map[encJ]) % 256]));
            }

            return encryptedValue.ToString();
        }

        public static string Decrypt(string key, string value)
        {
            return Encrypt(key, value);
        }
    }
}