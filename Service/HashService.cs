using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApiAutores.DTO;

namespace WebApiAutores.Service
{
    public class HashService
    {
        public ResultadoHash Hash(string text)
        {
            var salt = new byte[16];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(salt);
            return Hash(text, salt);

        }

        private ResultadoHash Hash(string text, byte[] salt)
        {
            var derivateKey = KeyDerivation.Pbkdf2(
                password: text,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32);
            var hash = Convert.ToBase64String(derivateKey);
            return new ResultadoHash() { Salt = salt, Hash = hash };
        }
    }
}
