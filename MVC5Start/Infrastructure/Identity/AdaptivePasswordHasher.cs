using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.AspNet.Identity;

namespace MVC5Start.Infrastructure.Identity
{
    public class AdaptivePasswordHasher : IPasswordHasher
    {
        #region Fields

        public const char Separator = '.';

        private readonly int _noOfIterations;

        #endregion Fields

        #region Constructors

        public AdaptivePasswordHasher() : this(GetNoOfIterationsFromCurrentYear)
        {
            
        }

        public AdaptivePasswordHasher(int noOfIterations)
        {
            this._noOfIterations = (noOfIterations <= 0) 
                ? GetNoOfIterationsFromCurrentYear 
                : noOfIterations;       
        }

        #endregion Constructors

        #region IPasswordHasher Members

        public string HashPassword(string password)
        {
            var result = CryptoHashPassword(password, this._noOfIterations);
            return this._noOfIterations.ToString("X") + Separator + result;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword)) 
                return PasswordVerificationResult.Failed;

            if (hashedPassword.Contains(Separator))
            {
                var parts = hashedPassword.Split(Separator);
                if (parts.Length != 2) 
                    return PasswordVerificationResult.Failed;

                var count = DecodeIterations(parts[0]);
                if (count <= 0) 
                    return PasswordVerificationResult.Failed;

                hashedPassword = parts[1];

                if (CryptoVerifyHashedPassword(hashedPassword, providedPassword, count))
                    return this._noOfIterations != count 
                        ? PasswordVerificationResult.SuccessRehashNeeded 
                        : PasswordVerificationResult.Success;
            }
            else if (CryptoVerifyHashedPassword(hashedPassword, providedPassword))
                return PasswordVerificationResult.SuccessRehashNeeded;

            return PasswordVerificationResult.Failed;
        }

        #endregion IPasswordHasher Members

        #region Private Members

        private static int DecodeIterations(string prefix)
        {
            int value;
            return int.TryParse(prefix, NumberStyles.HexNumber, null, out value) ? value : -1;
        }
        
        /// <summary>
        /// From OWASP : https://www.owasp.org/index.php/Password_Storage_Cheat_Sheet 
        /// </summary>
        public static int GetNoOfIterationsFromYear(int year)
        {
            const int startYear = 2000;
            const int startCount = 1000;

            if (year <= startYear) 
                return startCount;

            var count = startCount * ((int) Math.Pow(2, (int)((year - startYear) / 2)));

            return count < 0 ? int.MaxValue : count;
        }

        private static int GetNoOfIterationsFromCurrentYear
        {
            get { return GetNoOfIterationsFromYear(DateTime.UtcNow.Year); }
        }

        /// <summary>
        /// Default for Rfc2898DeriveBytes
        /// </summary>
        private const int Pbkdf2IterCount = 1000; 

        /// <summary>
        /// 256 bits
        /// </summary>
        private const int Pbkdf2SubkeyLength = 32; 

        /// <summary>
        /// 128 bits
        /// </summary>
        private const int SaltSize = 16;

        /// <summary>
        /// Hashed password formats
        /// Version 0:
        /// PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.
        /// (See also: SDL crypto guidelines v5.1, Part III)
        /// Format: { 0x00, salt, subkey }
        /// </summary>
        private static string CryptoHashPassword(string password, int iterationCount = Pbkdf2IterCount)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            // Produce a version 0 (see comment above) password hash.
            byte[] salt;
            byte[] subkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, SaltSize, iterationCount))
            {
                salt = deriveBytes.Salt;
                subkey = deriveBytes.GetBytes(Pbkdf2SubkeyLength);
            }

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
            return Convert.ToBase64String(outputBytes);
        }

        /// <summary>
        /// HashedPassword must be of the format of HashWithPassword (salt + Hash(salt+input)
        /// </summary>
        private static bool CryptoVerifyHashedPassword(string hashedPassword, string password, int iterationCount = Pbkdf2IterCount)
        {
            if (hashedPassword == null)
                throw new ArgumentNullException("hashedPassword");

            if (password == null)
                throw new ArgumentNullException("password");

            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            // Verify a version 0 (see comment above) password hash.
            if (hashedPasswordBytes.Length != (1 + SaltSize + Pbkdf2SubkeyLength) || hashedPasswordBytes[0] != 0x00)
                return false; // Wrong length or version header.

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);
            var storedSubkey = new byte[Pbkdf2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, Pbkdf2SubkeyLength);

            byte[] generatedSubkey;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterationCount))
            {
                generatedSubkey = deriveBytes.GetBytes(Pbkdf2SubkeyLength);
            }

            return ByteArraysEqual(storedSubkey, generatedSubkey);
        }

        /// <summary>
        /// Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }

        #endregion Private Members
    }
}