using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace CineApi
{
    public class Setting
    {
        public static string Secret = "fdere4232fd2432mfd0hs89dfhsdf0sd76dsbsdf9sd7f5sfdb08sdf83fgdgf2432fd";

        public static string GenerateHash(string password)
        {
            // cripto
            byte[] salt = Encoding.ASCII.GetBytes("123468973165");

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 32));

            return hashed;
        }
    }
}
