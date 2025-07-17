using System.Diagnostics;
using System.Security.Cryptography;

namespace GestionFacturas.Dominio.Seguridad
{
    internal static class PasswordHasher
    {
        private const int IterCount = 100_000;
        private static readonly RandomNumberGenerator _defaultRng = RandomNumberGenerator.Create();// secure PRNG
        public static string HashPassword(string password)
        {
            return Convert.ToBase64String(HashPasswordV3(password, _defaultRng));
        }

        private static byte[] HashPasswordV3(string password, RandomNumberGenerator rng)
        {
            return HashPasswordV3(password, rng,
                prf: KeyDerivationPrf.HMACSHA512,
                iterCount: IterCount,
                saltSize: 128 / 8,
                numBytesRequested: 256 / 8);
        }

        private static byte[] HashPasswordV3(string password, RandomNumberGenerator rng, KeyDerivationPrf prf, int iterCount, int saltSize, int numBytesRequested)
        {
            // Produce a version 3 (see comment above) text hash.
            byte[] salt = new byte[saltSize];
            rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
            WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }

        internal static PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {

            byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            // read the format marker from the hashed password
            if (decodedHashedPassword.Length == 0)
            {
                return PasswordVerificationResult.Failed;
            }
            switch (decodedHashedPassword[0])
            {
                case 0x00:
                    return PasswordVerificationResult.Failed;


                case 0x01:
                    if (VerifyHashedPasswordV3(decodedHashedPassword, providedPassword, out int embeddedIterCount, out KeyDerivationPrf prf))
                    {
                        // If this hasher was configured with a higher iteration count, change the entry now.
                        if (embeddedIterCount < IterCount)
                        {
                            return PasswordVerificationResult.SuccessRehashNeeded;
                        }

                        // V3 now requires SHA512. If the old PRF is SHA1 or SHA256, upgrade to SHA512 and rehash.
                        if (prf == KeyDerivationPrf.HMACSHA1 || prf == KeyDerivationPrf.HMACSHA256)
                        {
                            return PasswordVerificationResult.SuccessRehashNeeded;
                        }

                        return PasswordVerificationResult.Success;
                    }
                    else
                    {
                        return PasswordVerificationResult.Failed;
                    }

                default:
                    return PasswordVerificationResult.Failed; // unknown format marker
            }
        }


        private static bool VerifyHashedPasswordV3(byte[] hashedPassword, string password, out int iterCount, out KeyDerivationPrf prf)
        {
            iterCount = default;
            prf = default;

            try
            {
                // Read header information
                prf = (KeyDerivationPrf)ReadNetworkByteOrder(hashedPassword, 1);
                iterCount = (int)ReadNetworkByteOrder(hashedPassword, 5);
                int saltLength = (int)ReadNetworkByteOrder(hashedPassword, 9);

                // Read the salt: must be >= 128 bits
                if (saltLength < 128 / 8)
                {
                    return false;
                }
                byte[] salt = new byte[saltLength];
                Buffer.BlockCopy(hashedPassword, 13, salt, 0, salt.Length);

                // Read the subkey (the rest of the payload): must be >= 128 bits
                int subkeyLength = hashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }
                byte[] expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(hashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the incoming password and verify it
                byte[] actualSubkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, subkeyLength);
                return CryptographicOperations.FixedTimeEquals(actualSubkey, expectedSubkey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }


        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return (uint)buffer[offset + 0] << 24
                   | (uint)buffer[offset + 1] << 16
                   | (uint)buffer[offset + 2] << 8
                   | buffer[offset + 3];
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

    }

    public enum PasswordVerificationResult
    {
        /// <summary>
        /// Indicates password verification failed.
        /// </summary>
        Failed = 0,

        /// <summary>
        /// Indicates password verification was successful.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Indicates password verification was successful however the password was encoded using a deprecated algorithm
        /// and should be rehashed and updated.
        /// </summary>
        SuccessRehashNeeded = 2
    }

    internal enum KeyDerivationPrf
    {
        /// <summary>
        /// The HMAC algorithm (RFC 2104) using the SHA-1 hash function (FIPS 180-4).
        /// </summary>
        HMACSHA1,

        /// <summary>
        /// The HMAC algorithm (RFC 2104) using the SHA-256 hash function (FIPS 180-4).
        /// </summary>
        HMACSHA256,

        /// <summary>
        /// The HMAC algorithm (RFC 2104) using the SHA-512 hash function (FIPS 180-4).
        /// </summary>
        HMACSHA512,
    }

    /// <summary>
    /// Provides algorithms for performing key derivation.
    /// </summary>
    internal static class KeyDerivation
    {
        /// <summary>
        /// Performs key derivation using the PBKDF2 algorithm.
        /// </summary>
        /// <param name="password">The password from which to derive the key.</param>
        /// <param name="salt">The salt to be used during the key derivation process.</param>
        /// <param name="prf">The pseudo-random function to be used in the key derivation process.</param>
        /// <param name="iterationCount">The number of iterations of the pseudo-random function to apply
        /// during the key derivation process.</param>
        /// <param name="numBytesRequested">The desired length (in bytes) of the derived key.</param>
        /// <returns>The derived key.</returns>
        /// <remarks>
        /// The PBKDF2 algorithm is specified in RFC 2898.
        /// </remarks>
        public static byte[] Pbkdf2(string password, byte[] salt, KeyDerivationPrf prf, int iterationCount, int numBytesRequested)
        {

            // parameter checking
            if (prf < KeyDerivationPrf.HMACSHA1 || prf > KeyDerivationPrf.HMACSHA512)
            {
                throw new ArgumentOutOfRangeException(nameof(prf));
            }

            return Pbkdf2Util.Pbkdf2Provider.DeriveKey(password, salt, prf, iterationCount, numBytesRequested);
        }

        internal static class Pbkdf2Util
        {
            public static readonly IPbkdf2Provider Pbkdf2Provider = GetPbkdf2Provider();

            private static IPbkdf2Provider GetPbkdf2Provider()
            {
                // fastest implementation on .NET Core
                // Not supported on .NET Framework
                return new NetCorePbkdf2Provider();
            }
        }

        /// <summary>
        /// Implements Pbkdf2 using <see cref="Rfc2898DeriveBytes"/>.
        /// </summary>
        internal sealed class NetCorePbkdf2Provider : IPbkdf2Provider
        {
            public byte[] DeriveKey(string password, byte[] salt, KeyDerivationPrf prf, int iterationCount, int numBytesRequested)
            {
                Debug.Assert(password != null);
                Debug.Assert(salt != null);
                Debug.Assert(iterationCount > 0);
                Debug.Assert(numBytesRequested > 0);

                HashAlgorithmName algorithmName;
                switch (prf)
                {
                    case KeyDerivationPrf.HMACSHA1:
                        algorithmName = HashAlgorithmName.SHA1;
                        break;
                    case KeyDerivationPrf.HMACSHA256:
                        algorithmName = HashAlgorithmName.SHA256;
                        break;
                    case KeyDerivationPrf.HMACSHA512:
                        algorithmName = HashAlgorithmName.SHA512;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(prf));
                }

                return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterationCount, algorithmName, numBytesRequested);
            }
        }

        internal interface IPbkdf2Provider
        {
            byte[] DeriveKey(string password, byte[] salt, KeyDerivationPrf prf, int iterationCount, int numBytesRequested);
        }
    }
}
