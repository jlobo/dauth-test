using System;
using System.Linq;
using System.Security.Cryptography;

namespace dAuthMe.api.Tools
{
    public class ECDsaSerializer
    {
        const string _formatPublicPemP256 = "MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";
        public static ECDsa GetEcdsa(string key, bool isPrivate) => GetEcdsa(Convert.FromBase64String(key), isPrivate);

        public static ECDsa GetEcdsa(byte[] key, bool isPrivate) {
            if (key == null || !key.Any())
                throw new ArgumentException("Invalid key format");

            var parameters = new ECParameters()
            {
                Curve = ECCurve.NamedCurves.nistP256,
                Q = {
                    X = key.Skip(key.Length - 64).Take(32).ToArray(),
                    Y = key.Skip(key.Length - 32).Take(32).ToArray()
                }
            };
            
            if (!isPrivate) return ECDsa.Create(parameters);

            parameters.D = key.Take(32).ToArray();
            return ECDsa.Create(parameters);
        }

        public static string SerializeStr(ECDsa key, bool isPrivate) {
            return Convert.ToBase64String(Serialize(key, isPrivate));
        }

        public static Byte[] Serialize(ECDsa key, bool isPrivate)
        {
            var parametres = key.ExportExplicitParameters(isPrivate);
            if (isPrivate) return parametres.D.Concat(parametres.Q.X).Concat(parametres.Q.Y).ToArray();

            var buffer = Convert.FromBase64String(_formatPublicPemP256);
            Buffer.BlockCopy(parametres.Q.X, 0, buffer, 27, parametres.Q.X.Length);
            Buffer.BlockCopy(parametres.Q.Y, 0, buffer, 27 + parametres.Q.X.Length, parametres.Q.Y.Length);

            return buffer;
        }
    }
}